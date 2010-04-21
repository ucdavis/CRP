using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class Transaction : DomainObject
    {
        public Transaction()
        {
            SetDefaults();
        }

        public Transaction(Item item)
        {
            Item = item;

            SetDefaults();
        }

        private void SetDefaults()
        {
            TransactionDate = DateTime.Now;
            Credit = false;
            Check = false;
            Paid = false;
            Amount = 0.0m;
            Donation = false;

            Checks = new List<Check>();
        }

        [NotNull]
        public virtual Item Item { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        /// <summary>
        /// The confirmation number from touchnet.
        /// </summary>
        [Length(100)]
        public virtual string PaymentConfirmation { get; set; }
        public virtual bool Credit { get; set; }
        public virtual bool Check { get; set; }
        public virtual bool Paid { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual bool Donation { get; set; }

        public virtual ICollection<Check> Checks { get; set; }

        public virtual void AddCheck(Check check)
        {
            Checks.Add(check);
        }

        public virtual void RemoveCheck(Check check)
        {
            Checks.Remove(check);
        }
    }
}
