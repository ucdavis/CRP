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
        private static readonly string UserName = CloudConfigurationManager.GetSetting("CrpEmail");
        private static readonly string Password = CloudConfigurationManager.GetSetting("EmailToken");

        public void SendEmail(MailMessage mailMessage)
        {
            if (mailMessage.IsBodyHtml)
            {
                mailMessage.IsBodyHtml = false;
                var mimeType = new System.Net.Mime.ContentType("text/html");
                var alternate = AlternateView.CreateAlternateViewFromString(mailMessage.Body, mimeType);

                mailMessage.AlternateViews.Add(alternate);
            }
            var client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, Password),
                Port = 587,
                Host = "smtp.ucdavis.edu",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            client.Send(mailMessage);
        }
    }
}