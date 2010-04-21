using System.Net.Mail;
using System.Text;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq;

namespace CRP.Core.Abstractions
{
    public interface INotificationProvider
    {
        void SendConfirmation(IRepository repository, Transaction transaction, string emailAddress);
        void SendLowQuantityWarning(IRepository repository, Item item);
    }

    public class NotificationProvider : INotificationProvider
    {
        /// <summary>
        /// Sends the confirmation.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="emailAddress">The email address.</param>
        public void SendConfirmation(IRepository repository, Transaction transaction, string emailAddress)
        {
            Check.Require(transaction != null, "Transaction is required.");
            Check.Require(!string.IsNullOrEmpty(emailAddress), "Email address is required.");

            var body = string.Empty;

            if(transaction.Item != null && transaction.Item.Template != null)
            {
                body = transaction.Item.Template.Text;
            }
            else
            {
                body = repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault().Text ?? string.Empty;
            }
            var subject = "Payment confirmed";
            if (transaction.Item != null && transaction.Item.Name != null)
            {
                subject = transaction.Item.Name.Trim() + ": Payment confirmed";
            }
            //Things to replace in the body:
            //Contact Info:
            //  First Name
            //  Last Name
            //TotalPaid
            //Quantity
            //QuantityName
            

            var firstName = transaction.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer ??
                            string.Empty;
            var lastName = transaction.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer ??
                            string.Empty;
            var totalPaid = transaction.TotalPaid.ToString("C");
            var quantity = transaction.Quantity.ToString();
            var quantityName = string.Empty;
            if (transaction.Item != null)
            {
                quantityName = transaction.Item.QuantityName ?? string.Empty;
            }
         
            var paymentLog = transaction.PaymentLogs.Where(a => a.Accepted).FirstOrDefault();
            var paymentMethod = string.Empty;
            if(paymentLog != null)
            {
                if(paymentLog.Check)
                {
                    paymentMethod = "Check";
                }
                if (paymentLog.Credit)
                {
                    paymentMethod = "Credit Card";
                }
            }
            var donationThanks = string.Empty;
            if (transaction.DonationTotal > 0)
            {
                donationThanks = string.Format("Thank you for your donation of {0}.", transaction.DonationTotal.ToString("C"));
            }

            body = body.Replace("{FirstName}", firstName);
            body = body.Replace("{LastName}", lastName);
            body = body.Replace("{TotalPaid}", totalPaid);
            body = body.Replace("{Quantity}", quantity);
            body = body.Replace("{QuantityName}", quantityName);
            body = body.Replace("{TransactionNumber}", transaction.TransactionNumber);
            body = body.Replace("{PaymentMethod}", paymentMethod);
            body = body.Replace("{DonationThanks}", donationThanks);
            /*
Thank you {FirstName} {LastName} for your payment of {TotalPaid}.
{DonationThanks}
Your payment by {PaymentMethod} has been accepted.
You have purchased {Quantity} {QuantityName}.
Your Transaction number is: {TransactionNumber}
             */



            //MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", emailAddress,
            //                                     "payment confirmed",
            //                                     "your payment of " + transaction.TotalPaid.ToString("C") +
            //                                     " has been accepted.");


            MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", emailAddress,
                                                  subject,
                                                  body);
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message);
        }

        /// <summary>
        /// Sends the low quantity warning.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="item">The item.</param>
        public void SendLowQuantityWarning(IRepository repository, Item item)
        {
            Check.Require(item != null);
            var email = item.Editors.Where(a => a.Owner).First().User.Email;
            Check.Require(!string.IsNullOrEmpty(email));

            var subject = string.Format("On-line Registrations. Quantity Low Warning for {0}'s {1}", item.Name, item.QuantityName);
            var bodyBuilder = new StringBuilder(string.Format("Quantity low for {0}<br/><br/>", item.Name));
            bodyBuilder.AppendFormat("Total Quantity           : {0}<br/>", item.Quantity);
            bodyBuilder.AppendFormat("Total Sold               : {0}<br/>", item.Sold);
            bodyBuilder.AppendFormat("Total Sold and Paid for  : {0}<br/>", item.SoldAndPaidQuantity);
            bodyBuilder.      Append("====================================<br/>");
            bodyBuilder.AppendFormat("Quantity Remaining       : {0}<br/>", item.Quantity - item.Sold);
            var body = bodyBuilder.ToString();

            MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", email,
                                                  subject,
                                                  body);
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message);
        }
    }
}