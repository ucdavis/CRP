using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Azure;

namespace CRP.Core.Abstractions
{
    public interface IEmailService
    {
        void SendEmail(MailMessage mailMessage);
    }

    public class EmailService : IEmailService
    {
        //private static readonly string UserName = CloudConfigurationManager.GetSetting("CrpEmail");
        private static readonly string Password = CloudConfigurationManager.GetSetting("EmailToken");
        private static readonly string Host = CloudConfigurationManager.GetSetting("EmailHost");
        private static readonly string Port = CloudConfigurationManager.GetSetting("EmailPort");
        private static readonly string UserName = CloudConfigurationManager.GetSetting("EmailUserName");
        private static readonly string EmailFrom = CloudConfigurationManager.GetSetting("EmailFrom");

        public void SendEmail(MailMessage mailMessage)
        {
            mailMessage.Body = $"{mailMessage.Body} <br/><br/><hr> This email is not monitored.<br/>Please do not reply to it.";
            mailMessage.IsBodyHtml = true;


            //mailMessage.From = new MailAddress("registration-notify@ucdavis.edu");
            mailMessage.From = new MailAddress(EmailFrom);
            if (mailMessage.IsBodyHtml)
            {
                mailMessage.IsBodyHtml = false;
                var mimeType = new System.Net.Mime.ContentType("text/html");
                var alternate = AlternateView.CreateAlternateViewFromString(mailMessage.Body, mimeType);

                mailMessage.AlternateViews.Add(alternate);
            }

            using (var client = new SmtpClient(Host))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(UserName, Password);
                client.Port = int.Parse(Port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(mailMessage);
            }
        }
    }
}