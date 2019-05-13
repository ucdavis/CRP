using System;
using System.Collections.Generic;
using CRP.Core.Domain;

namespace CRP.Controllers.ViewModels
{
    public class PaymentConfirmationViewModel
    {
        public Transaction Transaction { get; set; }

        public string PostUrl { get; set; }

        public Dictionary<string, string> PaymentDictionary { get; set; }

        public string Signature { get; set; }
    }
}
