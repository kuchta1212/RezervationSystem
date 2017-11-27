using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ReservationSystem.Utils
{
    public class EmailController
    {
        public EmailController()
        {
        }

        public void SendReservationConfirmation(string emailTo)
        {
            var body = "<p>Potrvzujeme vaši rezervaci</p>";
            var subject = "Potvrzení rezervace";

            var task = SendEmail(emailTo, body, subject);
            task.ContinueWith(t =>
            {  
               if(!t.Result)
                Logger.Instance.WriteToLog("Email was not send due to error", "EmailController", LogType.ERROR);
            })
            ;
               
        }

        private async Task<bool> SendEmail(string mailTo, string body, string subject)
        {
            MailMessage mail = new MailMessage("sparta.rezervace@gmail.com", mailTo);
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("sparta.rezervace@gmail.com", "acsparta1234");
            client.Host = "smtp.gmail.com";
            mail.Subject = "this is a test of reservation number #.";
            mail.Body = "this is my test email body";
            await client.SendMailAsync(mail);
            return true;

            //var message = new MailMessage("jan.kuchar.89@gmail.com", mailTo)
            //{
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //};

            //using (var smtp = new SmtpClient())
            //{
            //    var credential = new NetworkCredential
            //    {
            //        UserName = "jan.kuchar.89@gmail.com",
            //        Password = "Kuchta12"
            //    };

            //    smtp.Credentials = credential;
            //    smtp.Host = "smtp.gmail.com";
            //    smtp.Port = 587;
            //    smtp.EnableSsl = true;
            //    smtp.Timeout = 1000;
            //    smtp.UseDefaultCredentials = false;
            //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    await smtp.SendMailAsync(message);
            //    return true;
            //}
        }
    }
}