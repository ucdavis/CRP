using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq;
using System.Net;
using Microsoft.Azure;
using Newtonsoft.Json;
using Serilog;



namespace CRP.Core.Abstractions
{
    public interface INotificationProvider
    {
        void SendConfirmation(IRepository repository, Transaction transaction, string emailAddress);

        void SendLowQuantityWarning(IRepository repository, Item item, int soldAndPaid);

        void SendRefundNotification(User user, Transaction refundTransaction, bool canceled, string link, PaymentLog paymentLog);

        void TestEmailFromService(MailMessage message);

        void SendPurchaseToOwners(IRepository repository, Item item, int transactionQuantity);
    }

    public class NotificationProvider : INotificationProvider
    {
        private readonly IEmailService _emailService;

        public NotificationProvider(IEmailService emailService)
        {
            _emailService = emailService;
        }

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

            if (body.Contains(StaticValues.ConfirmationTemplateDelimiter))
            {
                var delimiter = new string[] { StaticValues.ConfirmationTemplateDelimiter };
                var parse = body.Split(delimiter, StringSplitOptions.None);
                //if(transaction.Paid && transaction.TotalPaid > 0)
                if (transaction.TotalPaid > 0)
                {
                    body = parse[0];
                }
                else
                {
                    body = parse[1];
                }
            }
            string extraSubject;
            if(transaction.TotalPaid > 0)
            {
                if (transaction.Paid)
                {
                    extraSubject = "Payment confirmed";
                }
                else
                {
                    extraSubject = "Payment pending";
                }
            }
            else
            {
                extraSubject = "Order confirmed";
            }

            var subject = extraSubject;
            if (transaction.Item != null && transaction.Item.Name != null)
            {
                subject = transaction.Item.Name.Trim() + ": " + extraSubject;
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
            if (transaction.DonationTotal > 0 && transaction.Paid)
            {
                donationThanks = string.Format(ScreenText.STR_DonationText, transaction.DonationTotal.ToString("C"));
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


            try
            {
                _emailService.SendEmail(message);
                transaction.Notified = true;
                transaction.NotifiedDate = SystemTime.Now();
                repository.OfType<Transaction>().EnsurePersistent(transaction);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending confirmation");
            }
        }


        /// <summary>
        /// Sends the low quantity warning.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="item">The item.</param>
        public void SendLowQuantityWarning(IRepository repository, Item item, int soldAndPaid)
        {
            Check.Require(item != null);
            var email = item.Editors.Where(a => a.Owner).First().User.Email;
            Check.Require(!string.IsNullOrEmpty(email));

            var subject = string.Format("On-line Registrations. Quantity Low Warning for \"{0}'s\" {1}", item.Name, item.QuantityName);
            var bodyBuilder = new StringBuilder(string.Format("Quantity low for \"{0}\"<br/><br/>", item.Name));
            bodyBuilder.Append("<head>");
            bodyBuilder.Append("<style type=\"text/css\">");
            bodyBuilder.Append("tbody {color:green;height:50px}");
            bodyBuilder.Append("tfoot {color:red}");
            bodyBuilder.Append("</style>");
            bodyBuilder.Append("</head>");
            bodyBuilder.Append("<body>");
            bodyBuilder.Append("<table border=\"1\"  width=\"20%\">");
            bodyBuilder.Append("<tfoot>");
            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Quantity Remaining:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.Quantity - item.SoldCount);
            bodyBuilder.Append("</tr>");
            bodyBuilder.Append("</tfoot>");
            bodyBuilder.Append("<tbody>");
            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Quantity:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.Quantity);
            bodyBuilder.Append("</tr>");

            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Sold:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.SoldCount);
            bodyBuilder.Append("</tr>");

            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Sold and Paid for:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", soldAndPaid);
            bodyBuilder.Append("</tr>");

            bodyBuilder.Append("</tbody>");
            bodyBuilder.Append("</table>");
            bodyBuilder.Append("</body>");


            var body = bodyBuilder.ToString();

            MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", email,
                                                  subject,
                                                  body);
            message.IsBodyHtml = true;
            _emailService.SendEmail(message);
        }

        public void SendRefundNotification(User user, Transaction refundTransaction, bool canceled, string link, PaymentLog paymentLog)
        {
            var email = CloudConfigurationManager.GetSetting("EmailForRefunds");
            if(string.IsNullOrWhiteSpace(email))
            {
                email = "jsylvestre@ucdavis.edu";
            }

          
            var message = new MailMessage("automatedemail@caes.ucdavis.edu", email) { IsBodyHtml = true };
            if(canceled)
            {
                message.Subject = "Registration Refund CANCELED";
            }
            else
            {
                message.Subject = "Registration Refund";
            }

            string accountId;
            try
            {
                if (refundTransaction.ParentTransaction.FinancialAccount != null)
                {
                    accountId = refundTransaction.ParentTransaction.FinancialAccount.GetAccountString();
                }
                else
                {
                    accountId = refundTransaction.ParentTransaction.Item.FinancialAccount.GetAccountString() ?? string.Empty;
                }
            }
            catch (Exception)
            {
                accountId = "Not Found";
            }


            var processor = "???";
            var processorId = string.Empty;
            var cleared = string.Empty;
            string creditCardEmail = string.Empty;
            if (paymentLog != null && paymentLog.TnStatus == "A")
            {
                processor = "CyberSource";
                processorId = paymentLog.GatewayTransactionId;
                cleared = paymentLog.Cleared ? "Cleared" : "Not cleared at time of refund. Check Link.";
                try
                {
                    dynamic data = JsonConvert.DeserializeObject(paymentLog.ReturnedResults);
                    creditCardEmail = data.req_bill_to_email;
                }
                catch (Exception)
                {
                    creditCardEmail = "???";
                }

            }
            if (paymentLog != null && paymentLog.TnStatus == "S")
            {
                processor = "Touchnet";
            }

            var body = new StringBuilder("Refund Information<br/><br/>");
            if (canceled)
            {
                body.Append($"<br/<br/<b>This is a CANCEL of a refund!</b><br/<br/<b>");
            }
            body.Append("<b>Refunder</b><br/>");
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Name", user.FullName));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Email", user.Email));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Kerb", user.LoginID));
            body.Append("<br/>");
            body.Append("<b>Details</b><br/>");
            body.Append($"  <b>Transaction ID :</b> {refundTransaction.ParentTransaction.TransactionGuid}<br/>");
            body.Append($"  <b>Processor :</b> {processor}<br/>");
            if (!string.IsNullOrWhiteSpace(processorId))
            {
                body.Append($"  <b>Processor ID :</b> {processorId}<br/>");
            }
            if (!string.IsNullOrWhiteSpace(cleared))
            {
                body.Append($"  <b>Cleared? :</b> {cleared}<br/>");
            }
            body.Append($"  <b>Account ID :</b> {accountId}<br/>");
            body.Append($"  <b>Link to Payment Details :</b> {link}<br/>");
            body.Append(string.Format("  <b>{0} :</b> ${1}<br/>", "Refund Amount", refundTransaction.Amount));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Date", refundTransaction.TransactionDate));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Event Name", refundTransaction.Item.Name));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Event Id", refundTransaction.Item.Id));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Transaction", refundTransaction.ParentTransaction.TransactionNumber));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Reason For Refund", refundTransaction.CorrectionReason));

            try
            {
                var contactFirstName = refundTransaction.ParentTransaction.TransactionAnswers.FirstOrDefault(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                                                                                                                  a.Question.Name == StaticValues.Question_FirstName).Answer;
                var contactLastName = refundTransaction.ParentTransaction.TransactionAnswers.FirstOrDefault(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                                                                                                                 a.Question.Name == StaticValues.Question_LastName).Answer;
                var contactEmail = refundTransaction.ParentTransaction.TransactionAnswers.FirstOrDefault(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                                                                                                              a.Question.Name == StaticValues.Question_Email).Answer;
                body.Append("<br/>");
                body.Append("<b>Billing Contact</b><br/>");
                body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "First Name", contactFirstName));
                body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Last Name", contactLastName));
                body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Email", contactEmail));
                if (!string.IsNullOrWhiteSpace(creditCardEmail))
                {
                    body.Append($"  <b>Credit Card Email :</b> {creditCardEmail}<br/>");
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error sending Refund Notification");
            }
            message.Body = body.ToString();
            message.IsBodyHtml = true;

            _emailService.SendEmail(message);
        }

        public void TestEmailFromService(MailMessage message)
        {
            _emailService.SendEmail(message);
        }

        public void SendPurchaseToOwners(IRepository repository, Item item, int transactionQuantity)
        {
            var message = new MailMessage("automatedemail@caes.ucdavis.edu", item.Editors.Single(a => a.Owner).User.Email) { IsBodyHtml = true };
            foreach (var editor in item.Editors.Where(a => !a.Owner))
            {
                message.To.Add(editor.User.Email);
            }

            message.Subject = "Online Registration Notification";
            var body = new StringBuilder(string.Format("Your event: {0}<br/><br/>", item.Name));
            body.Append(string.Format("Just sold {0} {1}(s)<br/><br/>", transactionQuantity, item.QuantityName));
            body.Append("This is a notification only email. Do not reply.<br/><br/>");
            body.Append("You are getting this notification because you are an editor on an event that has a setting to inform you when people register for your event.");

            message.Body = body.ToString();

            message.IsBodyHtml = true;
            _emailService.SendEmail(message);
        }
    }

    public enum PaymentResultType
    {
        TransactionNotFound = 0,
        OverPaid,
        InValidPaymentLog
    }
}