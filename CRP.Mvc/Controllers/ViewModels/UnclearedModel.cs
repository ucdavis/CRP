using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Controllers.ViewModels
{
    public class UnclearedModel 
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime DatePayment { get; set; }
        public virtual Core.Domain.Transaction Transaction { get; set; }
        public virtual string GatewayTransactionId { get; set; }
        public virtual string TnPaymentDate { get; set; }
        public virtual bool IsOld { get; set; }
    }
}