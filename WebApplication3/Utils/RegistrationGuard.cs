using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace ReservationSystem.Utils
{
    public enum RegistrationCheckResult
    {
        Ok,

        // The hidden field was filled in - only an automated form filler does that.
        Honeypot,

        // The form token is missing, was tampered with, or was not issued by this site.
        InvalidToken,

        // Submitted faster than a person can fill in the form.
        TooFast,

        // The form was rendered too long ago (limits how long a scraped token can be replayed).
        Expired,

        // Too many registration attempts from the same address.
        RateLimited
    }

    public interface IFormTokenProtector
    {
        string Protect(string value);

        // Returns null when the token was not issued by this site or has been tampered with.
        string Unprotect(string token);
    }

    // Tokens are encrypted and signed with the site's machine key, so a bot cannot forge
    // a "rendered long enough ago" timestamp.
    public class MachineKeyFormTokenProtector : IFormTokenProtector
    {
        private const string Purpose = "ReservationSystem.RegisterForm.v1";

        public string Protect(string value)
        {
            return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(value), Purpose));
        }

        public string Unprotect(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var bytes = MachineKey.Unprotect(Convert.FromBase64String(token), Purpose);
                return bytes == null ? null : Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }

    public class RegistrationGuardOptions
    {
        // Throw-away / spam mail domains seen in real bot registrations. Add more without a deploy
        // through the "registrationBlockedEmailDomains" appSetting (comma separated).
        public static readonly string[] DefaultBlockedEmailDomains =
        {
            "acetylcholgh.ru", "testform.xyz", "testing-your-form.info", "checkyourform.xyz",
            "travel-e-store.com", "bubuk.site", "poshta.site", "polosmail.com", "vargosmail.com",
            "ronaldofmail.com", "belettersmail.com", "legenmail.com", "inboxgate.rest",
            "immenseignite.info", "laravelmail.com", "prohabits.es", "prrrvt.fun", "rockstarseo.net",
            "bornfrom.info", "perevozka24-7.com.ua", "ovout.com", "1ti.ru", "reebsd.com", "lumiomail.com"
        };

        public int MinSeconds { get; set; } = 3;

        public int MaxAgeMinutes { get; set; } = 120;

        public int MaxAttemptsPerHour { get; set; } = 10;

        public ISet<string> BlockedEmailDomains { get; set; } =
            new HashSet<string>(DefaultBlockedEmailDomains, StringComparer.OrdinalIgnoreCase);

        public static RegistrationGuardOptions FromAppSettings()
        {
            return FromAppSettings(ConfigurationManager.AppSettings);
        }

        public static RegistrationGuardOptions FromAppSettings(NameValueCollection settings)
        {
            var options = new RegistrationGuardOptions();
            options.MinSeconds = ReadInt(settings, "registrationMinSeconds", options.MinSeconds, 0);
            options.MaxAgeMinutes = ReadInt(settings, "registrationMaxAgeMinutes", options.MaxAgeMinutes, 1);
            options.MaxAttemptsPerHour = ReadInt(settings, "registrationMaxAttemptsPerHour", options.MaxAttemptsPerHour, 1);

            var extraDomains = settings["registrationBlockedEmailDomains"];
            if (!string.IsNullOrWhiteSpace(extraDomains))
            {
                foreach (var domain in extraDomains.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    options.BlockedEmailDomains.Add(domain.Trim().TrimStart('@').ToLowerInvariant());
            }

            return options;
        }

        private static int ReadInt(NameValueCollection settings, string key, int fallback, int minimum)
        {
            int value;
            return int.TryParse(settings[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value >= minimum
                ? value
                : fallback;
        }
    }

    // Basic bot protection for the public registration form. It is deliberately dependency-free:
    //   1. honeypot   - a hidden input that people never fill in;
    //   2. time trap  - a signed "form rendered at" token; bots post instantly or replay an old token;
    //   3. rate limit - a cap on attempts per client address (in memory, per server instance);
    //   4. blocklist  - throw-away e-mail domains.
    // Register as a singleton: the rate limiter keeps its state in this instance.
    public class RegistrationGuard
    {
        private static readonly TimeSpan AttemptWindow = TimeSpan.FromHours(1);
        private static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(1);
        private const int SweepThreshold = 1000;

        private readonly IFormTokenProtector protector;
        private readonly Func<DateTime> utcNow;
        private readonly RegistrationGuardOptions options;
        private readonly Dictionary<string, Queue<DateTime>> attempts = new Dictionary<string, Queue<DateTime>>();
        private readonly object attemptsLock = new object();
        private DateTime lastSweep = DateTime.MinValue;

        public RegistrationGuard()
            : this(new MachineKeyFormTokenProtector(), () => DateTime.UtcNow, RegistrationGuardOptions.FromAppSettings())
        {
        }

        public RegistrationGuard(IFormTokenProtector protector, Func<DateTime> utcNow, RegistrationGuardOptions options)
        {
            if (protector == null)
                throw new ArgumentNullException(nameof(protector));
            if (utcNow == null)
                throw new ArgumentNullException(nameof(utcNow));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.protector = protector;
            this.utcNow = utcNow;
            this.options = options;
        }

        // Value for the hidden "form rendered at" field. Create a fresh one every time the form is shown.
        public string CreateFormToken()
        {
            return protector.Protect(utcNow().Ticks.ToString(CultureInfo.InvariantCulture));
        }

        public RegistrationCheckResult Check(string honeypotValue, string formToken, string clientIp)
        {
            var now = utcNow();

            if (!string.IsNullOrWhiteSpace(honeypotValue))
                return RegistrationCheckResult.Honeypot;

            if (!TryRecordAttempt(clientIp, now))
                return RegistrationCheckResult.RateLimited;

            DateTime issuedAt;
            if (!TryReadToken(formToken, out issuedAt))
                return RegistrationCheckResult.InvalidToken;

            var age = now - issuedAt;
            if (age < TimeSpan.FromSeconds(options.MinSeconds))
                return RegistrationCheckResult.TooFast;
            if (age > TimeSpan.FromMinutes(options.MaxAgeMinutes))
                return RegistrationCheckResult.Expired;

            return RegistrationCheckResult.Ok;
        }

        public bool IsBlockedEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var at = email.LastIndexOf('@');
            if (at < 0 || at == email.Length - 1)
                return false;

            var domain = email.Substring(at + 1).Trim().TrimEnd('.');
            foreach (var blocked in options.BlockedEmailDomains)
            {
                if (string.Equals(domain, blocked, StringComparison.OrdinalIgnoreCase)
                    || domain.EndsWith("." + blocked, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private bool TryReadToken(string token, out DateTime issuedAt)
        {
            issuedAt = default(DateTime);

            var payload = protector.Unprotect(token);
            long ticks;
            if (payload == null || !long.TryParse(payload, NumberStyles.None, CultureInfo.InvariantCulture, out ticks))
                return false;
            if (ticks > DateTime.MaxValue.Ticks)
                return false;

            issuedAt = new DateTime(ticks, DateTimeKind.Utc);
            return true;
        }

        // Only allowed attempts are recorded, so a blocked client regains its quota as soon as the
        // oldest attempt leaves the one-hour window - hammering the form does not extend the block.
        private bool TryRecordAttempt(string clientIp, DateTime now)
        {
            var key = string.IsNullOrWhiteSpace(clientIp) ? "unknown" : clientIp.Trim();

            lock (attemptsLock)
            {
                if (attempts.Count >= SweepThreshold && now - lastSweep >= SweepInterval)
                    SweepExpired(now);

                Queue<DateTime> queue;
                if (!attempts.TryGetValue(key, out queue))
                {
                    queue = new Queue<DateTime>();
                    attempts[key] = queue;
                }

                while (queue.Count > 0 && now - queue.Peek() >= AttemptWindow)
                    queue.Dequeue();

                if (queue.Count >= options.MaxAttemptsPerHour)
                    return false;

                queue.Enqueue(now);
                return true;
            }
        }

        private void SweepExpired(DateTime now)
        {
            lastSweep = now;

            var idle = new List<string>();
            foreach (var pair in attempts)
            {
                var queue = pair.Value;
                while (queue.Count > 0 && now - queue.Peek() >= AttemptWindow)
                    queue.Dequeue();
                if (queue.Count == 0)
                    idle.Add(pair.Key);
            }

            foreach (var key in idle)
                attempts.Remove(key);
        }
    }
}
