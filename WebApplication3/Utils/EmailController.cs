using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;
using log4net;

namespace ReservationSystem.Utils
{
    public class EmailController
    {
        public EmailController()
        {
        }

        readonly ILog logger = LogManager.GetLogger(typeof(EmailController));

        public void SendReservationConfirmation(string emailTo, string table, string time, string date)
        {
            var body = string.Format(Resource.ReservationEmailConfirmation, date, time, table);
            var subject = Resource.ReservationEmailConfirmationSubject;

            SendEmail(emailTo, body, subject);
        }

        public void SendRegisterEmail(string url, string email)
        {
            var body = string.Format(Resource.RegistrationEmail, url);
            var subject = Resource.RegistrationEmailSubject;

            SendEmail(email, body, subject);
        }

        private void SendEmail(string mailTo, string body, string subject)
        {
            try
            {
                MailMessage mail = new MailMessage("sparta.rezervace@gmail.com", mailTo)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sparta.rezervace@gmail.com", "acsparta1234");
                client.Host = "smtp.gmail.com";
                client.Send(mail);
            }
            catch (Exception ex)
            {
                logger.Error("Email was not send due to error" + ex.ToString());
            }
 
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