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
            TransactionAnswers = new List<TransactionAnswer>();
            QuantityAnswers = new List<QuantityAnswer>();
            ChildTransactions = new List<Transaction>();
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
        public virtual int Quantity { get; set; }
        /// <summary>
        /// The parent transaction object, this is only populated for donation fields.
        /// </summary>
        public virtual Transaction ParentTransaction { get; set; }

        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<TransactionAnswer> TransactionAnswers { get; set; }
        public virtual ICollection<QuantityAnswer> QuantityAnswers { get; set; }
        public virtual ICollection<Transaction> ChildTransactions { get; set; }

        public virtual void AddCheck(Check check)
        {
            Checks.Add(check);
        }

        public virtual void RemoveCheck(Check check)
        {
            Checks.Remove(check);
        }

        public virtual void AddTransactionAnswer(TransactionAnswer transactionAnswer)
        {
            transactionAnswer.Transaction = this;

            TransactionAnswers.Add(transactionAnswer);
        }

        public virtual void AddQuantityAnswer(QuantityAnswer quantityAnswer)
        {
            quantityAnswer.Transaction = this;

            QuantityAnswers.Add(quantityAnswer);
        }

        public virtual void AddChildTransaction(Transaction transaction)
        {
            transaction.ParentTransaction = this;
            transaction.Check = Check;
            transaction.Credit = Credit;
            ChildTransactions.Add(transaction);
        }
    }
}
