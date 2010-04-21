using System.Net.Mail;
using CRP.Core.Domain;
using UCDArch.Core.Utils;

namespace CRP.Core.Abstractions
{
    public interface INotificationProvider
    {
        void SendConfirmation(Transaction transaction, string emailAddress);
    }

    public class NotificationProvider : INotificationProvider
    {
        public void SendConfirmation(Transaction transaction, string emailAddress)
        {
            Check.Require(transaction != null, "Transaction is required.");
            Check.Require(!string.IsNullOrEmpty(emailAddress), "Email address is required.");

            MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", emailAddress,
                                                  "payment confirmed",
                                                  "your payment of " + transaction.TotalPaid.ToString("C") +
                                                  " has been accepted.");
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message);
        }
    }
}