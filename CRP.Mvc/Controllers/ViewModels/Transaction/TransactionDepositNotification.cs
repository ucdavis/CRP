using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Controllers.ViewModels.Transaction
{
    public class TransactionDepositNotification
    {
        /// <summary>
        /// The tracking number that has been assigned to this deposit
        /// </summary>
        public string KfsTrackingNumber { get; set; }

        /// <summary>
        /// The tracking number assigned by the merchant
        /// </summary>
        public string MerchantTrackingNumber { get; set; }

        /// <summary>
        /// The tracking number assigned by the payment processor
        /// </summary>
        public string ProcessorTrackingNumber { get; set; }

        /// <summary>
        /// The date the transaction took place
        /// </summary>
        public DateTime TransactionDate { get; set; }
    }
}