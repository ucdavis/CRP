//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using CRP.Core.Domain;
//using CRP.Core.Resources;
//using UCDArch.Core.PersistanceSupport;

//namespace CRP.Core.Abstractions
//{
//    public interface IPaymentProvider
//    {
//        void ProcessPayment(decimal amount, int transactionId, string name, string email, string address1, string address2, string city, string state, string zip, string country);
//        void CompletePayment(string status, int reference, int transactionId, decimal paymentAmount, int trackingId);
//    }

//    public class TouchnetPaymentProvider : IPaymentProvider
//    {
//        /////////////////////////////////////////////////////////
//        // Parameters to be passed to the TouchnetPayment site.
//        //
//        //  UPAY_SITE_ID - the id for the uPay site
//        //  BILL_NAME - (OPTIONAL) billing name
//        //  BILL_EMAIL_ADDRESS - (OPTIONAL) billing e-mail addrss
//        //  BILL_STREET1 - (OPTIONAL) billing street address 1
//        //  BILL_STREET2 - (OPTIONAL) billing street address 2
//        //  BILL_CITY - (OPTIONAL) billing city
//        //  BILL_STATE - (OPTIONAL) billing state
//        //  BILL_POSTAL_CODE - (OPTIONAL) billing zip code
//        //  BILL_COUNTRY - (OPTIONAL) billing country
//        //  EXT_TRANS_ID - the unique id to be used to cross reference with the uPay
//        //  AMT - amount to be charged
//        //  VALIDATION_KEY - validation key
//        /////////////////////////////////////////////////////////

//        /////////////////////////////////////////////////////////
//        // Parameters that get passed back from Touchnet
//        //
//        //  tpg_trans_id - reference number assigned by Payment gateway
//        //  pmt_amt - amount of transaction processed by gateway
//        //  sys_tracking_id - internal marketplace identifier, displayed to customer on the uPay receipt
//        //  pmt_status - the status of the payment, we want "success"
//        /////////////////////////////////////////////////////////

//        private readonly IRepository<Transaction> _transactionRepository;

//        public TouchnetPaymentProvider(IRepository<Transaction> transactionRepository)
//        {
//            _transactionRepository = transactionRepository;
//        }

//        public void ProcessPayment(decimal amount, int transactionId, string name, string email, string address1, string address2, string city, string state, string zip, string country)
//        {
//            // do nothing, we don't have a page to redirect hte user to for now to test payments

//            WebClient client = new WebClient();

//            return;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="reference"></param>
//        /// <param name="transactionId">CRP transaction id</param>
//        /// <param name="paymentAmount"></param>
//        /// <param name="trackingId"></param>
//        public void CompletePayment(string status, int reference, int transactionId, decimal paymentAmount, int trackingId)
//        {
//            var transaction = _transactionRepository.GetNullableById(transactionId);

//            // not sure what to do if this is null, because that means touchnet returned something we shouldn't have gotten back
//            if (transaction == null || transaction.Paid)
//            {
//                return;
//            }

//            if (status == "success")
//            {
//                // process the save
//                transaction.ReferenceNumber = reference;
//                transaction.TrackingId = trackingId;
//                transaction.Paid = true;
//                //TODO: Do something about the payment amount

//                // save the object
//                _transactionRepository.EnsurePersistent(transaction);

//                // trigger an email to the user
//                //TODO: write some emailing construct
//                var email =
//                    transaction.TransactionAnswers.Where(a => a.Question.Name == StaticValues.Question_Email).
//                        FirstOrDefault();

//                if (email != null)
//                {
//                    MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", email.Answer,
//                                                          "payment confirmed",
//                                                          "your payment of " + paymentAmount.ToString() +
//                                                          " has been accepted.");
//                    SmtpClient client = new SmtpClient("smtp.ucdavis.edu"); //TODO, Fix if ever implemented
//                    client.Send(message);
//                }
//                else
//                {
//                    MailMessage message = new MailMessage("automatedemail@caes.ucdavis.edu", "anlai@ucdavis.edu",
//                                      "payment confirmed",
//                                      "your payment of " + paymentAmount.ToString() +
//                                      " has been accepted.");
//                    SmtpClient client = new SmtpClient("smtp.ucdavis.edu"); //TODO, Fix if ever implemented
//                    client.Send(message);
//                }
//            }
//            //TODO: figure out something to do on payment failure

//            return;
//        }
//    }
//}