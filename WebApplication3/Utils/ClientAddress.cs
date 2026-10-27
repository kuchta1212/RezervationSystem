using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ReservationSystem.Utils
{
    // Works out which address a request really came from, for per-client limits.
    //
    // A request that reaches the server directly from a public address is taken at face value and
    // X-Forwarded-For is ignored (a client can write anything it likes into that header). Behind a
    // reverse proxy - e.g. the Azure App Service front end - the socket address is the proxy's private
    // or link-local address instead; only then is the real client read from X-Forwarded-For, and it is
    // the LAST public entry that counts, because that is the one our own proxy appended.
    public static class ClientAddress
    {
        public const string Unknown = "unknown";

        public static string Resolve(string remoteAddress, string forwardedFor)
        {
            IPAddress remote;
            if (!TryParse(remoteAddress, out remote))
                return Unknown;

            if (IsPublic(remote))
                return Key(remote);

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                var entries = forwardedFor.Split(',');
                for (var i = entries.Length - 1; i >= 0; i--)
                {
                    IPAddress candidate;
                    if (TryParse(entries[i], out candidate) && IsPublic(candidate))
                        return Key(candidate);
                }
            }

            return Key(remote);
        }

        // Accepts "1.2.3.4", "1.2.3.4:5678", "2001:db8::1" and "[2001:db8::1]:5678".
        private static bool TryParse(string value, out IPAddress address)
        {
            address = null;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var text = value.Trim();
            if (text.StartsWith("[", StringComparison.Ordinal))
            {
                var end = text.IndexOf(']');
                if (end < 0)
                    return false;
                text = text.Substring(1, end - 1);
            }
            else if (text.IndexOf(':') > 0 && text.IndexOf(':') == text.LastIndexOf(':'))
            {
                text = text.Substring(0, text.IndexOf(':'));
            }

            if (!IPAddress.TryParse(text, out address))
                return false;

            if (address.IsIPv4MappedToIPv6)
                address = address.MapToIPv4();

            return true;
        }

        private static bool IsPublic(IPAddress address)
        {
            if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
                return false;

            var b = address.GetAddressBytes();
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return !(b[0] == 0
                    || b[0] == 10
                    || b[0] == 127
                    || (b[0] == 100 && b[1] >= 64 && b[1] <= 127)   // shared / carrier-grade NAT space
                    || (b[0] == 169 && b[1] == 254)                 // link-local
                    || (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
                    || (b[0] == 192 && b[1] == 168)
                    || b[0] >= 224);                                // multicast and reserved
            }

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return !(address.IsIPv6LinkLocal
                    || address.IsIPv6SiteLocal
                    || address.IsIPv6Multicast
                    || (b[0] & 0xFE) == 0xFC);                      // unique local fc00::/7
            }

            return false;
        }

        // One household / mobile subscriber normally owns a whole IPv6 /64, so that is the unit to count.
        private static string Key(IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetworkV6)
                return address.ToString();

            var prefix = address.GetAddressBytes().Take(8).Concat(new byte[8]).ToArray();
            return new IPAddress(prefix) + "/64";
        }
    }
}
