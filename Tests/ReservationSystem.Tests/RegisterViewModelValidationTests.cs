using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ReservationSystem.Models;
using ReservationSystem.Utils;
using Xunit;

namespace ReservationSystem.Tests
{
    public class RegisterViewModelValidationTests
    {
        private static RegisterViewModel ValidModel()
        {
            return new RegisterViewModel
            {
                Name = "Jan Novak",
                PhoneNumber = "777 123 456",
                Email = "jan.novak@seznam.cz",
                Password = "Heslo123",
                ConfirmPassword = "Heslo123"
            };
        }

        private static List<ValidationResult> Validate(RegisterViewModel model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), results, true);
            return results;
        }

        private static bool HasErrorFor(IEnumerable<ValidationResult> results, string member)
        {
            return results.Any(r => r.MemberNames.Contains(member));
        }

        [Fact]
        public void ValidModel_HasNoErrors()
        {
            Assert.Empty(Validate(ValidModel()));
        }

        [Fact]
        public void BotProtectionFields_AreNotValidatedByDataAnnotations()
        {
            var model = ValidModel();
            model.Website = "anything";
            model.FormToken = null;

            Assert.Empty(Validate(model));
        }

        // ---- phone -----------------------------------------------------------------------------

        [Theory]
        [InlineData("777123456")]
        [InlineData("777 123 456")]
        [InlineData("777-123-456")]
        [InlineData("777.123.456")]
        [InlineData("+420 777 123 456")]
        [InlineData("+420777123456")]
        [InlineData("(+420) 777 123 456")]
        [InlineData("00420777123456")]
        [InlineData("+421905123456")]
        [InlineData("+380 50 123 45 67")]
        [InlineData("  777123456  ")]
        // Slovak national format keeps the trunk prefix 0
        [InlineData("0905123456")]
        [InlineData("0905 123 456")]
        public void Phone_ValidCzechSlovakOrInternationalNumbers_AreAccepted(string phone)
        {
            var model = ValidModel();
            model.PhoneNumber = phone;

            Assert.False(HasErrorFor(Validate(model), "PhoneNumber"));
        }

        [Theory]
        // bare 10-11 digit numbers without a + / 00 prefix - the shape registration bots typed
        [InlineData("81234567890")]
        [InlineData("81112223344")]
        [InlineData("2125550123")]
        [InlineData("6505550123")]
        // not phone numbers at all
        [InlineData("abc")]
        [InlineData("12345")]
        [InlineData("123456789")]
        [InlineData("777 123 45")]
        [InlineData("777 123 4567")]
        [InlineData("+1234")]
        [InlineData("<script>")]
        [InlineData("0123456789")]
        [InlineData("08274447597")]
        public void Phone_BotStyleOrMalformedNumbers_AreRejected(string phone)
        {
            var model = ValidModel();
            model.PhoneNumber = phone;

            Assert.True(HasErrorFor(Validate(model), "PhoneNumber"));
        }

        [Fact]
        public void Phone_Missing_IsRejected()
        {
            var model = ValidModel();
            model.PhoneNumber = null;

            Assert.True(HasErrorFor(Validate(model), "PhoneNumber"));
        }

        // ---- name ------------------------------------------------------------------------------

        [Theory]
        [InlineData("Jan Novak")]
        [InlineData("Honza")]
        [InlineData("Ann O'Neil")]
        [InlineData("Ing. Jan Novak")]
        [InlineData("J.Novak")]
        [InlineData("Mgr.Petr")]
        [InlineData("Anna Marie Novakova-Svobodova")]
        public void Name_OrdinaryNames_AreAccepted(string name)
        {
            var model = ValidModel();
            model.Name = name;

            Assert.False(HasErrorFor(Validate(model), "Name"));
        }

        [Theory]
        [InlineData("Rufetuk cujiv pocadag u111 example.azurewebsites.net Yh")]
        [InlineData("visit www.example.com now")]
        [InlineData("http://spam.example")]
        [InlineData("https://spam.example/x")]
        [InlineData("go to spam.xyz/x")]
        [InlineData("best-deals.shop")]
        [InlineData("jan@seznam.cz")]
        [InlineData("<b>bold</b>")]
        [InlineData("<script>alert(1)</script>")]
        public void Name_LinksEmailsAndMarkup_AreRejected(string name)
        {
            var model = ValidModel();
            model.Name = name;

            Assert.True(HasErrorFor(Validate(model), "Name"));
        }

        [Fact]
        public void Name_LongerThan80Characters_IsRejected()
        {
            var model = ValidModel();
            model.Name = new string('a', 81);

            Assert.True(HasErrorFor(Validate(model), "Name"));
        }

        [Fact]
        public void Name_Exactly80Characters_IsAccepted()
        {
            var model = ValidModel();
            model.Name = new string('a', 80);

            Assert.False(HasErrorFor(Validate(model), "Name"));
        }

        [Theory]
        [InlineData("-spam.com")]
        [InlineData("--spam.com")]
        [InlineData("Capek-spam.com")]
        [InlineData("mail.example.com")]
        [InlineData("FOO.COM")]
        public void Name_DomainNamesAreRejected_EvenWithLeadingHyphensOrUpperCase(string name)
        {
            var model = ValidModel();
            model.Name = name;

            Assert.True(HasErrorFor(Validate(model), "Name"));
        }

        // Validators run during model binding - before the bot protection - so they must stay cheap
        // however large or adversarial the posted value is (a quadratic regex took minutes on ~100 KB).
        [Theory]
        [InlineData(50)]
        [InlineData(500)]
        [InlineData(50000)]
        [InlineData(1000000)]
        public void Name_HugeHyphenatedValue_IsRejectedQuickly(int repeat)
        {
            var model = ValidModel();
            model.Name = string.Concat(Enumerable.Repeat("a-", repeat));

            var timer = System.Diagnostics.Stopwatch.StartNew();
            var results = Validate(model);
            timer.Stop();

            Assert.True(timer.ElapsedMilliseconds < 1000, "validation took " + timer.ElapsedMilliseconds + " ms");
            Assert.Equal(repeat * 2 > 80, HasErrorFor(results, "Name"));
        }

        [Fact]
        public void NoLinks_ValueAtTheScanLimit_IsScanned_AndLongerValuesAreSuspicious()
        {
            Assert.False(NoLinksAttribute.ContainsLink(new string('a', 1000)));
            Assert.True(NoLinksAttribute.ContainsLink(new string('a', 1001)));
        }

        [Fact]
        public void Name_Missing_IsRejected()
        {
            var model = ValidModel();
            model.Name = null;

            Assert.True(HasErrorFor(Validate(model), "Name"));
        }

        [Theory]
        [InlineData("http://x.cz", true)]
        [InlineData("Jan Novak", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void NoLinks_ContainsLink(string value, bool expected)
        {
            Assert.Equal(expected, NoLinksAttribute.ContainsLink(value));
        }

        // ---- e-mail ----------------------------------------------------------------------------

        [Fact]
        public void Email_LongerThan100Characters_IsRejected()
        {
            var model = ValidModel();
            model.Email = new string('a', 96) + "@x.cz";

            Assert.True(HasErrorFor(Validate(model), "Email"));
        }

        [Fact]
        public void Email_Exactly100Characters_IsAccepted()
        {
            var model = ValidModel();
            model.Email = new string('a', 95) + "@x.cz";

            Assert.False(HasErrorFor(Validate(model), "Email"));
        }

        [Fact]
        public void Email_Malformed_IsRejected()
        {
            var model = ValidModel();
            model.Email = "not-an-email";

            Assert.True(HasErrorFor(Validate(model), "Email"));
        }
    }
}
