using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ReservationSystem.Utils
{
    // Rejects values that contain a URL, a domain name, an e-mail address or HTML markup. Real names
    // never do, while spam bots put their links into every free-text field they can find.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class NoLinksAttribute : ValidationAttribute
    {
        // Anything longer than this is not a name. It is rejected without being scanned, so a huge value
        // cannot cost CPU - validators run during model binding, before any bot protection.
        private const int MaxScannedLength = 1000;

        // The domain alternative may only start at the beginning of a run of letters/digits/hyphens
        // ((?<![a-z0-9-])). Without that, a long "a-a-a-..." value is re-scanned from every position and
        // the cost grows quadratically. The match timeout is a second line of defence.
        private static readonly Regex LinkPattern = new Regex(
            @"https?:|://|www\.|[@<>]|(?<![a-z0-9-])[a-z0-9-]+\.(?:com|net|org|info|biz|xyz|site|online|top|club|shop|store|link|click|fun|rest|ru|ua|cz|sk|pl|de|es|it|uk|eu)\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(250));

        public static bool ContainsLink(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (value.Length > MaxScannedLength)
                return true;

            try
            {
                return LinkPattern.IsMatch(value);
            }
            catch (RegexMatchTimeoutException)
            {
                return true;
            }
        }

        public override bool IsValid(object value)
        {
            return !ContainsLink(value as string);
        }
    }
}
