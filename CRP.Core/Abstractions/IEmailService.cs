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