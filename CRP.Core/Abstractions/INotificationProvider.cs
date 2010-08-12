using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq;


namespace CRP.Core.Abstractions
{
    public interface INotificationProvider
    {
        void SendConfirmation(IRepository repository, Transaction transaction, string emailAddress);
        void SendLowQuantityWarning(IRepository repository, Item item, int transactionQuantity);
        void SendPaymentResultErrors(string email, PaymentResultParameters touchNetValues, NameValueCollection requestAllParams, string extraBody, PaymentResultType paymentResultType);
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
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message);
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
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message);
        }

        public void SendPaymentResultErrors(string email, PaymentResultParameters touchNetValues, NameValueCollection requestAllParams, string extraBody, PaymentResultType paymentResultType)
        {
            var emailNotFound = false;
            if(string.IsNullOrEmpty(email))
            {
                email = "jSylvestre@ucdavis.edu";
                emailNotFound = true;
            }
            var client = new SmtpClient("smtp.ucdavis.edu"); //Need for errors/debugging
            var message = new MailMessage("automatedemail@caes.ucdavis.edu", email) {IsBodyHtml = true};

            var body = new StringBuilder("TouchNet Results<br/><br/>");
            body.Append(DateTime.Now + "<br/>");

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
            client.Send(message);
        }
    }

    public enum PaymentResultType
    {
        TransactionNotFound = 0,
        OverPaid,
        InValidPaymentLog
    }
}