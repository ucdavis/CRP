using System;
using CRP.Core.Domain;

namespace CRP.Mvc.Controllers.ViewModels.Payment
{
    public class PaymentReceiptViewModel
    {
        public Item Item { get; set; }

        public Transaction Transaction { get; set; }

        public string Amount { get; set; }

        public string CardNumber { get; set; }

        public DateTime? CardExp { get; set; }


        public DateTime AuthDateTime { get; set; }

        public string AuthCode { get; set; }
    }
}