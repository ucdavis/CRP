using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class PaymentLog : DomainObject
    {
        public PaymentLog() { SetDefaults(); }

        public PaymentLog(decimal amount)
        {
            Amount = amount;

            SetDefaults();
        }

        private void SetDefaults()
        {
            DatePayment = DateTime.Now;
            Accepted = false;
            Check = false;
            Credit = false;
        }

        /// <summary>
        /// Payee's name
        /// </summary>
        [Length(200)]
        public virtual string Name { get; set; }
        /// <summary>
        /// Amount paid
        /// </summary>
        public virtual decimal Amount { get; set; }
        public virtual DateTime DatePayment { get; set; }
        [NotNull]
        public virtual Transaction Transaction { get; set; }

        public virtual int? CheckNumber { get; set; }
        /// <summary>
        /// Payment gateway transaction id
        /// </summary>
        [Length(16)]
        public virtual string GatewayTransactionId { get; set; }
        /// <summary>
        /// Card type that was used to pay
        /// </summary>
        [Length(20)]
        public virtual string CardType { get; set; }
        /// <summary>
        /// Whether or not the payment has been accepted, example where this might be false is if a check bounces
        /// </summary>
        public virtual bool Accepted { get; set; }
        /// <summary>
        /// Signifies check payment
        /// </summary>
        public virtual bool Check { get; set; }
        /// <summary>
        /// Signifies credit card payment through gateway
        /// </summary>
        public virtual bool Credit { get; set; }
        public virtual string Notes { get; set; }
    }
}
