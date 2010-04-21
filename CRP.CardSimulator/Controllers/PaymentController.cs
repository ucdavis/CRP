using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using MvcContrib.Attributes;

namespace CRP.CardSimulator.Controllers
{
    public class PaymentController : Controller
    {
        /////////////////////////////////////////////////////////
        // Parameters to be passed to the TouchnetPayment site.
        //
        //  UPAY_SITE_ID - the id for the uPay site
        //  BILL_NAME - (OPTIONAL) billing name
        //  BILL_EMAIL_ADDRESS - (OPTIONAL) billing e-mail addrss
        //  BILL_STREET1 - (OPTIONAL) billing street address 1
        //  BILL_STREET2 - (OPTIONAL) billing street address 2
        //  BILL_CITY - (OPTIONAL) billing city
        //  BILL_STATE - (OPTIONAL) billing state
        //  BILL_POSTAL_CODE - (OPTIONAL) billing zip code
        //  BILL_COUNTRY - (OPTIONAL) billing country
        //  EXT_TRANS_ID - the unique id to be used to cross reference with the uPay
        //  AMT - amount to be charged
        //  VALIDATION_KEY - validation key
        /////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////
        // Parameters that get passed back from Touchnet
        //
        //  tpg_trans_id - reference number assigned by Payment gateway
        //  pmt_amt - amount of transaction processed by gateway
        //  sys_tracking_id - internal marketplace identifier, displayed to customer on the uPay receipt
        //  pmt_status - the status of the payment, we want "success"
        /////////////////////////////////////////////////////////


        [AcceptPost]
        public ActionResult Payment(int UPAY_SITE_ID, string EXT_TRANS_ID, decimal AMT, string BILL_NAME, string BILL_EMAIL_ADDRESS
            , string BILL_STREET1, string BILL_STREET2, string BILL_CITY, string BILL_STATE, string BILL_POSTAL_CODE, string BILL_COUNTRY
            , string VALIDATION_KEY)
        {
            var viewModel = new PaymentViewModel()
                                {
                                    UpaySiteId = UPAY_SITE_ID,
                                    TransactionId = EXT_TRANS_ID,
                                    Amount = AMT,
                                    BillName = BILL_NAME,
                                    BillEmailAddress = BILL_EMAIL_ADDRESS,
                                    BillStreet1 = BILL_STREET1,
                                    BillStreet2 = BILL_STREET2,
                                    BillCity = BILL_CITY,
                                    BillState = BILL_STATE,
                                    BillPostalCode = BILL_POSTAL_CODE,
                                    BillCountry = BILL_COUNTRY,
                                    ValidationKey = VALIDATION_KEY
                                };

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult ProcessPayment(bool approved, int transactionId, decimal amount)
        {
            //// create a post call to deal with telling CRP whether or not payment has been approved.
            //string postData = "EXT_TRANS_ID=" + transactionId.ToString();
            //postData += "&PMT_STATUS=" + (approved ? "success" : "cancelled");
            //postData += "&PMT_AMOUNT=" + amount.ToString();
            //postData += "&TPG_TRANS_ID=" + "123456";
            //byte[] data = Encoding.UTF8.GetBytes(postData);


            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ReturnUrl"]);
            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;
            
            //Stream stream = request.GetRequestStream();
            //stream.Write(data, 0, data.Length);
            //stream.Flush();
            //stream.Close();

            string url = ConfigurationManager.AppSettings["ReturnUrl"];
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

            string data = String.Format("EXT_TRANS_ID={0}&PMT_STATUS={1}&PMT_AMOUNT={2}&TPG_TRANS_ID={3}", transactionId,
                                        approved ? "success" : "cancelled", amount, "123456");
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buffer.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Close();

            var res = (HttpWebResponse) request.GetResponse();

            var resStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(resStream);
            string response = sr.ReadToEnd();

            return View();
        }

    }

    public class PaymentViewModel
    {
        public int UpaySiteId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string BillName { get; set; }
        public string BillEmailAddress { get; set; }
        public string BillStreet1 { get; set; }
        public string BillStreet2 { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillPostalCode { get; set; }
        public string BillCountry { get; set; }
        public string ValidationKey { get; set; }
    }
}
