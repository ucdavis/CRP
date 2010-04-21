using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace CRP.Controllers.Helpers
{
    public class PaymentResultParameters
    {
        // ReSharper disable InconsistentNaming
        public int? EXT_TRANS_ID { get; set; }
        public string PMT_STATUS { get; set; }
        public string NAME_ON_ACCT { get; set; }
        public decimal? PMT_AMT { get; set; }
        public string TPG_TRANS_ID { get; set; }
        public string CARD_TYPE { get; set; }
        // ReSharper restore InconsistentNaming
    }
}
