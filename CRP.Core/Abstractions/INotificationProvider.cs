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
using Serilog;


namespace CRP.Core.Abstractions
{
    public interface INotificationProvider
    {
        void SendConfirmation(IRepository repository, Transaction transaction, string emailAddress);
        void SendLowQuantityWarning(IRepository repository, Item item, int transactionQuantity);
        void SendPaymentResultErrors(string email, PaymentResultParameters touchNetValues, NameValueCollection requestAllParams, string extraBody, PaymentResultType paymentResultType);
        void SendRefundNotification(User user, Transaction refundTransaction, bool canceled);

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
        /// <param name="transactionQuantity">The transaction quantity.</param>
        public void SendLowQuantityWarning(IRepository repository, Item item, int transactionQuantity)
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
            bodyBuilder.AppendFormat("<td>{0}</td>", item.Quantity - (item.Sold + transactionQuantity));
            bodyBuilder.Append("</tr>");
            bodyBuilder.Append("</tfoot>");
            bodyBuilder.Append("<tbody>");
            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Quantity:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.Quantity);
            bodyBuilder.Append("</tr>");

            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Sold:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.Sold  + transactionQuantity);
            bodyBuilder.Append("</tr>");

            bodyBuilder.Append("<tr>");
            bodyBuilder.Append("<td>Total Sold and Paid for:</td>");
            bodyBuilder.AppendFormat("<td>{0}</td>", item.SoldAndPaidQuantity);
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

        public void SendPaymentResultErrors(string email, PaymentResultParameters touchNetValues, NameValueCollection requestAllParams, string extraBody, PaymentResultType paymentResultType)
        {
            var emailNotFound = false;
            if(string.IsNullOrEmpty(email))
            {
                email = "jSylvestre@ucdavis.edu";
                emailNotFound = true;
            }

            var message = new MailMessage("automatedemail@caes.ucdavis.edu", email) {IsBodyHtml = true};

            var body = new StringBuilder("TouchNet Results<br/><br/>");
            body.Append(DateTime.UtcNow.ToPacificTime() + "<br/>");

            switch (paymentResultType)
            {
                case PaymentResultType.TransactionNotFound:
                    message.Subject = "TouchNet Post Results -- Transaction not found";
                    break;
                case PaymentResultType.OverPaid:
                    message.Subject = "TouchNet Post Results -- Has Overpaid";
                    break;
                case PaymentResultType.InValidPaymentLog:
                    message.Subject = "TouchNet Post Results -- PaymentLog is Not Valid";
                    break;
                default:
                    message.Subject = "TouchNet Post Results -- Unknown Result Type";
                    break;
            }
            if (emailNotFound)
            {
                message.Subject = string.Format("email missing too {0}", message.Subject);
            }

            foreach (var k in requestAllParams.AllKeys)
            {
                if (k.ToLower() != "posting_key")
                {
                    body.Append(k + ":" + requestAllParams[k]);
                    body.Append("<br/>");
                }
            }
            message.Body = body.ToString();

            body.Append("<br/>Function parameters================<br/>");
            body.Append("acct_addr: " + touchNetValues.acct_addr + "<br/>");
            body.Append("acct_addr2: " + touchNetValues.acct_addr2 + "<br/>");
            body.Append("acct_city: " + touchNetValues.acct_city + "<br/>");
            body.Append("acct_state: " + touchNetValues.acct_state + "<br/>");
            body.Append("acct_zip: " + touchNetValues.acct_zip + "<br/>");
            body.Append("CANCEL_LINK: " + touchNetValues.CANCEL_LINK + "<br/>");
            body.Append("CARD_TYPE: " + touchNetValues.CARD_TYPE + "<br/>");
            body.Append("ERROR_LINK: " + touchNetValues.ERROR_LINK + "<br/>");
            body.Append("EXT_TRANS_ID: " + touchNetValues.EXT_TRANS_ID + "<br/>");
            body.Append("NAME_ON_ACCT: " + touchNetValues.NAME_ON_ACCT + "<br/>");
            body.Append("PMT_AMT: " + touchNetValues.PMT_AMT + "<br/>");
            body.Append("pmt_date: " + touchNetValues.pmt_date + "<br/>");
            body.Append("PMT_STATUS: " + touchNetValues.PMT_STATUS + "<br/>");
            body.Append("Submit: " + touchNetValues.Submit + "<br/>");
            body.Append("SUCCESS_LINK: " + touchNetValues.SUCCESS_LINK + "<br/>");
            body.Append("sys_tracking_id: " + touchNetValues.sys_tracking_id + "<br/>");
            body.Append("TPG_TRANS_ID: " + touchNetValues.TPG_TRANS_ID + "<br/>");
            body.Append("UPAY_SITE_ID: " + touchNetValues.UPAY_SITE_ID + "<br/>");

            message.Body = body.ToString();
            if(!string.IsNullOrEmpty(extraBody))
            {
                message.Body = message.Body + extraBody;
            }
            _emailService.SendEmail(message);
        }

        public void SendRefundNotification(User user, Transaction refundTransaction, bool canceled)
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

            var fid = string.Empty;
            if (refundTransaction.ParentTransaction.FidUsed != null)
            {
                fid = refundTransaction.ParentTransaction.FidUsed;
            }
            else
            {
                fid = refundTransaction.ParentTransaction.Item.TouchnetFID ?? string.Empty;
            }

            var body = new StringBuilder("Refund Information<br/><br/>");
            body.Append("<b>Refunder</b><br/>");
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Name", user.FullName));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Email", user.Email));
            body.Append(string.Format("  <b>{0} :</b> {1}<br/>", "Kerb", user.LoginID));
            body.Append("<br/>");
            body.Append("<b>Details</b><br/>");
            body.Append(string.Format("  <b>{0} :</b> {1} FID={2}<br/>", "Touchnet Id", refundTransaction.ParentTransaction.TransactionGuid, fid));
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
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error sending Refund Notification");
            }
            message.Body = body.ToString();

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
            body.Append(string.Format("Just sold {0} {1}<br/><br/>", transactionQuantity, item.QuantityName));
            body.Append("This is a notification only email. Do not reply.<br/><br/>");
            body.Append("You are getting this notification because you are an editor on an event that has a setting to inform you when people register for your event.");

            message.Body = body.ToString();
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