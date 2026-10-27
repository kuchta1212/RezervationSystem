using ReservationSystem.Utils;
using Xunit;

namespace ReservationSystem.Tests
{
    public class NewRegistrationNotificationTests
    {
        [Fact]
        public void Body_HtmlEncodesTheValuesSuppliedByTheRegistrant()
        {
            var body = EmailController.BuildNewRegistrationNotificationBody(
                "<a href=\"http://evil.example\">Click</a>",
                "x<b>y</b>@example.cz",
                "https://site.example/Admin/Settings?a=1&b=2");

            Assert.DoesNotContain("<a href=\"http://evil.example\">", body);
            Assert.Contains("&lt;a href=&quot;http://evil.example&quot;&gt;Click&lt;/a&gt;", body);
            Assert.Contains("x&lt;b&gt;y&lt;/b&gt;@example.cz", body);
            Assert.Contains("a=1&amp;b=2", body);
        }

        [Fact]
        public void Body_ContainsTheNameEmailAndAdminLink()
        {
            var body = EmailController.BuildNewRegistrationNotificationBody("Jan Novak", "jan@seznam.cz", "https://site.example/Admin/Settings");

            Assert.Contains("Jan Novak", body);
            Assert.Contains("jan@seznam.cz", body);
            Assert.Contains("<a href=\"https://site.example/Admin/Settings\">https://site.example/Admin/Settings</a>", body);
        }
    }
}
