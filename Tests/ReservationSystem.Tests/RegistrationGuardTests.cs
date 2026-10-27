using System;
using System.Collections.Specialized;
using ReservationSystem.Utils;
using Xunit;

namespace ReservationSystem.Tests
{
    public class RegistrationGuardTests
    {
        private const string Ip = "203.0.113.7";

        // Deterministic stand-in for the machine-key protector: "tok:<value>" is valid, anything else is not.
        private class FakeTokenProtector : IFormTokenProtector
        {
            public string Protect(string value)
            {
                return "tok:" + value;
            }

            public string Unprotect(string token)
            {
                return token != null && token.StartsWith("tok:", StringComparison.Ordinal) ? token.Substring(4) : null;
            }
        }

        private DateTime now = new DateTime(2026, 10, 27, 12, 0, 0, DateTimeKind.Utc);

        private RegistrationGuard CreateGuard(RegistrationGuardOptions options = null)
        {
            return new RegistrationGuard(new FakeTokenProtector(), () => now, options ?? new RegistrationGuardOptions());
        }

        // A form that was rendered `shownSecondsAgo` seconds before "now".
        private string TokenShown(RegistrationGuard guard, double shownSecondsAgo)
        {
            var real = now;
            now = real.AddSeconds(-shownSecondsAgo);
            var token = guard.CreateFormToken();
            now = real;
            return token;
        }

        // ---- time trap -------------------------------------------------------------------------

        [Fact]
        public void Check_FormSubmittedAfterTheMinimumDelay_IsOk()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_FormSubmittedInstantly_IsTooFast()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.TooFast, guard.Check(null, TokenShown(guard, 0), Ip));
        }

        [Fact]
        public void Check_FormSubmittedJustBeforeTheMinimumDelay_IsTooFast()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.TooFast, guard.Check(null, TokenShown(guard, 2.9), Ip));
        }

        [Fact]
        public void Check_FormSubmittedExactlyAtTheMinimumDelay_IsOk()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 3), Ip));
        }

        [Fact]
        public void Check_TokenOlderThanTheMaximumAge_IsExpired()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Expired, guard.Check(null, TokenShown(guard, 121 * 60), Ip));
        }

        [Fact]
        public void Check_TokenExactlyAtTheMaximumAge_IsStillOk()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 120 * 60), Ip));
        }

        [Fact]
        public void Check_TokenIssuedInTheFuture_IsTreatedAsTooFast()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.TooFast, guard.Check(null, TokenShown(guard, -30), Ip));
        }

        [Fact]
        public void Check_TheMinimumDelayCanBeConfigured()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MinSeconds = 10 });

            Assert.Equal(RegistrationCheckResult.TooFast, guard.Check(null, TokenShown(guard, 9), Ip));
            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("garbage")]
        [InlineData("tok:not-a-number")]
        [InlineData("tok:-5")]
        [InlineData("tok:9223372036854775807")]
        [InlineData("tok:99999999999999999999")]
        public void Check_MissingOrForgedToken_IsInvalid(string token)
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.InvalidToken, guard.Check(null, token, Ip));
        }

        // ---- honeypot --------------------------------------------------------------------------

        [Fact]
        public void Check_FilledHoneypot_IsRejectedEvenWithAValidToken()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Honeypot, guard.Check("http://spam.example", TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_BlankHoneypot_IsIgnored()
        {
            var guard = CreateGuard();

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check("   ", TokenShown(guard, 10), Ip));
        }

        // ---- rate limit ------------------------------------------------------------------------

        [Fact]
        public void Check_MoreAttemptsThanAllowedPerHour_AreRateLimited()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 3 });

            for (var i = 0; i < 3; i++)
                Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), Ip));

            Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_RateLimitIsTrackedPerAddress()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 1 });

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), "198.51.100.1"));
            Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), "198.51.100.1"));
            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), "198.51.100.2"));
        }

        [Fact]
        public void Check_RateLimitRecoversOnceTheHourHasPassed()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 2 });
            guard.Check(null, TokenShown(guard, 10), Ip);
            guard.Check(null, TokenShown(guard, 10), Ip);
            Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), Ip));

            now = now.AddMinutes(61);

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_BlockedAttemptsDoNotExtendTheBlock()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 2 });
            guard.Check(null, TokenShown(guard, 10), Ip);
            guard.Check(null, TokenShown(guard, 10), Ip);

            // a bot keeps hammering for half an hour...
            for (var i = 0; i < 5; i++)
            {
                now = now.AddMinutes(6);
                Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), Ip));
            }

            // ...but the quota returns one hour after the last *allowed* attempt
            now = now.AddMinutes(31);
            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_InvalidTokenAttemptsCountTowardsTheRateLimit()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 3 });

            for (var i = 0; i < 3; i++)
                Assert.Equal(RegistrationCheckResult.InvalidToken, guard.Check(null, "forged", Ip));

            Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), Ip));
        }

        [Fact]
        public void Check_UnknownAddress_ShareOneBucket()
        {
            var guard = CreateGuard(new RegistrationGuardOptions { MaxAttemptsPerHour = 1 });

            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, TokenShown(guard, 10), null));
            Assert.Equal(RegistrationCheckResult.RateLimited, guard.Check(null, TokenShown(guard, 10), ""));
        }

        // ---- blocked e-mail domains ------------------------------------------------------------

        [Theory]
        [InlineData("bot@testform.xyz")]
        [InlineData("BOT@TESTFORM.XYZ")]
        [InlineData("bot@mail.testform.xyz")]
        [InlineData("bot@testform.xyz.")]
        [InlineData("someone@reebsd.com")]
        [InlineData("a@b@lumiomail.com")]
        public void IsBlockedEmailDomain_KnownThrowAwayDomains_AreBlocked(string email)
        {
            Assert.True(CreateGuard().IsBlockedEmailDomain(email));
        }

        [Theory]
        [InlineData("jan.novak@gmail.com")]
        [InlineData("jan@seznam.cz")]
        [InlineData("bot@nottestform.xyz")]
        [InlineData("bot@testform.xyz.cz")]
        [InlineData("testform.xyz")]
        [InlineData("bot@")]
        [InlineData("")]
        [InlineData(null)]
        public void IsBlockedEmailDomain_OrdinaryOrMalformedAddresses_AreNotBlocked(string email)
        {
            Assert.False(CreateGuard().IsBlockedEmailDomain(email));
        }

        [Fact]
        public void IsBlockedEmailDomain_UsesTheConfiguredExtraDomains()
        {
            var options = new RegistrationGuardOptions();
            options.BlockedEmailDomains.Add("spam.example");
            var guard = CreateGuard(options);

            Assert.True(guard.IsBlockedEmailDomain("x@spam.example"));
            Assert.True(guard.IsBlockedEmailDomain("x@testform.xyz"));
        }

        [Fact]
        public void IsBlockedEmailDomain_ConfiguredDomainsMatchRegardlessOfCase()
        {
            var options = new RegistrationGuardOptions();
            options.BlockedEmailDomains.Add("Spam.Example");
            var guard = CreateGuard(options);

            Assert.True(guard.IsBlockedEmailDomain("x@spam.example"));
            Assert.True(guard.IsBlockedEmailDomain("X@SPAM.EXAMPLE"));
            Assert.True(guard.IsBlockedEmailDomain("x@mail.Spam.example"));
        }

        // ---- configuration ---------------------------------------------------------------------

        [Fact]
        public void Options_WithoutSettings_UseTheDefaults()
        {
            var options = RegistrationGuardOptions.FromAppSettings(new NameValueCollection());

            Assert.Equal(3, options.MinSeconds);
            Assert.Equal(120, options.MaxAgeMinutes);
            Assert.Equal(10, options.MaxAttemptsPerHour);
            Assert.Contains("testform.xyz", options.BlockedEmailDomains);
        }

        [Fact]
        public void Options_ReadTheAppSettings()
        {
            var options = RegistrationGuardOptions.FromAppSettings(new NameValueCollection
            {
                { "registrationMinSeconds", "7" },
                { "registrationMaxAgeMinutes", "30" },
                { "registrationMaxAttemptsPerHour", "4" },
                { "registrationBlockedEmailDomains", "Spam.example, @junk.test;other.test" }
            });

            Assert.Equal(7, options.MinSeconds);
            Assert.Equal(30, options.MaxAgeMinutes);
            Assert.Equal(4, options.MaxAttemptsPerHour);
            Assert.Contains("spam.example", options.BlockedEmailDomains);
            Assert.Contains("junk.test", options.BlockedEmailDomains);
            Assert.Contains("other.test", options.BlockedEmailDomains);
            Assert.Contains("testform.xyz", options.BlockedEmailDomains);
        }

        [Fact]
        public void Options_InvalidSettings_FallBackToTheDefaults()
        {
            var options = RegistrationGuardOptions.FromAppSettings(new NameValueCollection
            {
                { "registrationMinSeconds", "abc" },
                { "registrationMaxAgeMinutes", "0" },
                { "registrationMaxAttemptsPerHour", "-3" }
            });

            Assert.Equal(3, options.MinSeconds);
            Assert.Equal(120, options.MaxAgeMinutes);
            Assert.Equal(10, options.MaxAttemptsPerHour);
        }

        [Fact]
        public void Options_ZeroMinimumDelay_DisablesTheTimeTrap()
        {
            var options = RegistrationGuardOptions.FromAppSettings(new NameValueCollection { { "registrationMinSeconds", "0" } });

            Assert.Equal(0, options.MinSeconds);
        }

        // ---- the real machine-key token --------------------------------------------------------

        [Fact]
        public void MachineKeyProtector_RoundTripsAValue()
        {
            var protector = new MachineKeyFormTokenProtector();

            Assert.Equal("638000000000000000", protector.Unprotect(protector.Protect("638000000000000000")));
        }

        [Fact]
        public void MachineKeyProtector_RejectsTamperedAndForeignTokens()
        {
            var protector = new MachineKeyFormTokenProtector();
            var token = protector.Protect("638000000000000000");
            var chars = token.ToCharArray();
            chars[10] = chars[10] == 'A' ? 'B' : 'A';
            var tampered = new string(chars);

            Assert.Null(protector.Unprotect(tampered));
            Assert.Null(protector.Unprotect("not base64 !!"));
            Assert.Null(protector.Unprotect("AAAA"));
            Assert.Null(protector.Unprotect(""));
            Assert.Null(protector.Unprotect(null));
        }

        [Fact]
        public void Guard_WorksEndToEndWithTheRealMachineKeyToken()
        {
            var guard = new RegistrationGuard(new MachineKeyFormTokenProtector(), () => now, new RegistrationGuardOptions());
            var token = guard.CreateFormToken();

            Assert.Equal(RegistrationCheckResult.TooFast, guard.Check(null, token, Ip));

            now = now.AddSeconds(10);
            Assert.Equal(RegistrationCheckResult.Ok, guard.Check(null, token, "198.51.100.9"));
            Assert.Equal(RegistrationCheckResult.InvalidToken, guard.Check(null, token + "x", "198.51.100.10"));
        }

        [Fact]
        public void Constructor_RejectsNullDependencies()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistrationGuard(null, () => now, new RegistrationGuardOptions()));
            Assert.Throws<ArgumentNullException>(() => new RegistrationGuard(new FakeTokenProtector(), null, new RegistrationGuardOptions()));
            Assert.Throws<ArgumentNullException>(() => new RegistrationGuard(new FakeTokenProtector(), () => now, null));
        }
    }
}
