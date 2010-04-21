using System;
using System.Collections.Generic;
using UCDArch.Core.DomainModel;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Check : DomainObject
    {
        public Check()
        {
            SetDefaults();
        }

        public Check(string payee, int checkNumber, decimal amount)
        {
            Payee = payee;
            CheckNumber = checkNumber;
            Amount = amount;

            SetDefaults();
        }

        private void SetDefaults()
        {
            DateReceived = DateTime.Now;

            Transactions = new List<Transaction>();
        }

        [Required]
        [Length(200)]
        public virtual string Payee { get; set; }
        public virtual int CheckNumber { get; set; }
        public virtual Decimal Amount { get; set; }
        public virtual DateTime DateReceived { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
