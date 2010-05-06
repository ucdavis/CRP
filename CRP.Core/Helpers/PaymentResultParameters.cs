
namespace CRP.Core.Helpers
{
    public class PaymentResultParameters
    {
        /*
            * Since, technically, anyone could post a result to your server to make it think the payment was successful, we recommend following these steps for processing the form post that returns to your server.
            * Verify the POSTING_KEY against your local copy
            * Verify UPAY_SITE_ID against expected value(s)
            * Check that the PMT_STATUS is "success”
            * Check that the TPG_TRANS_ID is not "DUMMY_TRANS_ID"
            * Retrieve the transaction from persistent storage using the EXT_TRANS_ID
            * Verify that the PMT_AMT matches the transaction amount as retrieved from your database
            * Save any of the returned information that you want to keep (especially TPG_TRANS_ID)
            * Mark the transaction as "paid” and perform any post-payment processing
        */ 
        // ReSharper disable InconsistentNaming
        public string EXT_TRANS_ID { get; set; }
        public string PMT_STATUS { get; set; }
        public string NAME_ON_ACCT { get; set; }
        public decimal? PMT_AMT { get; set; }
        public string TPG_TRANS_ID { get; set; }
        public string CARD_TYPE { get; set; }
        public string posting_key { get; set; }
        public string pmt_date { get; set; }

        public string sys_tracking_id { get; set; }
        public string acct_addr { get; set; }
        public string acct_addr2 { get; set; }
        public string acct_city { get; set; }
        public string acct_state { get; set; }
        public string acct_zip { get; set; }
        public string UPAY_SITE_ID { get; set; }
        public string ERROR_LINK { get; set; }
        public string Submit { get; set; }
        public string SUCCESS_LINK { get; set; }
        public string CANCEL_LINK { get; set; }

        // ReSharper restore InconsistentNaming
    }
}
