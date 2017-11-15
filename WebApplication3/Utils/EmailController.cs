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

            var task = SendEmail("kuchar_jan@live.com", body, subject);
            task.ContinueWith(t =>
            {  
               if(!t.Result)
                Logger.Instance.WriteToLog("Email was not send due to error", "EmailController", LogType.ERROR);
            })
            ;
               
        }

        private async Task<bool> SendEmail(string mailTo, string body, string subject)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(mailTo));
            message.From = new MailAddress("jku@prodata.cz");
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "jku@prodata.cz",
                    Password = "Kuchta12"
                };

                smtp.Credentials = credential;
                smtp.Host = "smtp.prodata.cz";
                smtp.Port = 25;
                smtp.EnableSsl = false;
                await smtp.SendMailAsync(message);
                return true;
            }
        }
    }
}