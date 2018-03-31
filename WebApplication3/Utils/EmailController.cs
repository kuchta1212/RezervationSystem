using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Text;
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

        public void SendReservationConfirmation(string emailTo, List<KeyValuePair<string, TimeSpan>> data, string date)
        {
            var tables = new Dictionary<string,KeyValuePair<TimeSpan, TimeSpan>>();
            foreach (var d in data)
            {
                if(!tables.ContainsKey(d.Key))
                    tables.Add(d.Key, new KeyValuePair<TimeSpan, TimeSpan>(d.Value, new TimeSpan(0,0,0,0)));
                else
                {
                    var times = tables[d.Key];
                    if (times.Key > d.Value)
                    {
                        var finishTime = times.Key < times.Value ? times.Value : times.Key;

                        tables[d.Key] = new KeyValuePair<TimeSpan, TimeSpan>(d.Value, finishTime);
                    }
                    else if(times.Value < d.Value)
                    {
                        tables[d.Key] = new KeyValuePair<TimeSpan, TimeSpan>(times.Key, d.Value);
                    }
                }
               
            }

            var sb = new StringBuilder();
            foreach (var table in tables)
            {
              sb.Append(string.Format(Resource.ReservationEmailConfirmation, date, table.Value.Key.ToString() + "-" + table.Value.Value.Add(new TimeSpan(0,0,30,0)).ToString(), table.Key));
            }

            var body = sb.ToString();
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