using ReservationSystem.Utils;
using Xunit;

namespace ReservationSystem.Tests
{
    public class ClientAddressTests
    {
        [Fact]
        public void Resolve_DirectPublicClient_IsUsedAndForwardedForIsIgnored()
        {
            // a client talking to us directly can write anything into X-Forwarded-For
            Assert.Equal("203.0.113.5", ClientAddress.Resolve("203.0.113.5", "198.51.100.9"));
        }

        [Theory]
        [InlineData("10.0.0.5", "198.51.100.9", "198.51.100.9")]
        [InlineData("169.254.130.1", "198.51.100.9:54321", "198.51.100.9")]
        [InlineData("172.16.4.4", "198.51.100.9", "198.51.100.9")]
        [InlineData("192.168.1.10", "198.51.100.9", "198.51.100.9")]
        [InlineData("100.64.0.8", "198.51.100.9", "198.51.100.9")]
        [InlineData("127.0.0.1", "198.51.100.9", "198.51.100.9")]
        [InlineData("::1", "198.51.100.9", "198.51.100.9")]
        public void Resolve_RequestFromAProxyHop_UsesTheForwardedClient(string remote, string forwardedFor, string expected)
        {
            Assert.Equal(expected, ClientAddress.Resolve(remote, forwardedFor));
        }

        [Fact]
        public void Resolve_ForgedLeadingEntries_AreIgnored()
        {
            // the client claims to be 6.6.6.6; our proxy appended the real address at the end
            Assert.Equal("198.51.100.9", ClientAddress.Resolve("169.254.130.1", "6.6.6.6, 198.51.100.9:443"));
        }

        [Fact]
        public void Resolve_PrivateEntriesInTheChain_AreSkipped()
        {
            Assert.Equal("198.51.100.9", ClientAddress.Resolve("10.0.0.5", "198.51.100.9, 10.1.1.1"));
        }

        [Theory]
        [InlineData("10.0.0.5", null, "10.0.0.5")]
        [InlineData("10.0.0.5", "", "10.0.0.5")]
        [InlineData("10.0.0.5", "unknown", "10.0.0.5")]
        [InlineData("10.0.0.5", "garbage, 10.9.9.9", "10.0.0.5")]
        public void Resolve_ProxyHopWithoutUsableForwardedFor_FallsBackToTheSocketAddress(string remote, string forwardedFor, string expected)
        {
            Assert.Equal(expected, ClientAddress.Resolve(remote, forwardedFor));
        }

        [Theory]
        [InlineData("2001:db8:aaaa:bbbb:1:2:3:4", "2001:db8:aaaa:bbbb::/64")]
        [InlineData("2001:db8:aaaa:bbbb:ffff:ffff:ffff:ffff", "2001:db8:aaaa:bbbb::/64")]
        public void Resolve_Ipv6Clients_AreCountedPerSlash64(string remote, string expected)
        {
            Assert.Equal(expected, ClientAddress.Resolve(remote, null));
        }

        [Fact]
        public void Resolve_Ipv6ClientBehindAProxy_IsReadFromBracketedForwardedFor()
        {
            Assert.Equal("2001:db8:1:2::/64", ClientAddress.Resolve("10.0.0.5", "[2001:db8:1:2:3:4:5:6]:4711"));
        }

        [Fact]
        public void Resolve_Ipv4MappedIpv6_IsTreatedAsIpv4()
        {
            Assert.Equal("203.0.113.5", ClientAddress.Resolve("::ffff:203.0.113.5", null));
            Assert.Equal("198.51.100.9", ClientAddress.Resolve("::ffff:10.0.0.5", "198.51.100.9"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not an address")]
        public void Resolve_UnusableSocketAddress_IsUnknown(string remote)
        {
            Assert.Equal(ClientAddress.Unknown, ClientAddress.Resolve(remote, "198.51.100.9"));
        }
    }
}
