using System;
using System.Collections.Generic;
using System.Linq;
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
            Amount = 0.0m;
            Donation = false;

            //Checks = new List<Check>();
            PaymentLogs = new List<PaymentLog>();
            TransactionAnswers = new List<TransactionAnswer>();
            QuantityAnswers = new List<QuantityAnswer>();
            ChildTransactions = new List<Transaction>();
        }

        [NotNull]
        public virtual Item Item { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual bool Credit { get; set; }
        public virtual bool Check { get; set; }
        //public virtual bool Paid { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual bool Donation { get; set; }
        public virtual int Quantity { get; set; } //TODO: Write a test to make sure this quantity can't exceed the amount that is available.
        /// <summary>
        /// The parent transaction object, this is only populated for donation fields.
        /// </summary>
        public virtual Transaction ParentTransaction { get; set; }
        /// <summary>
        /// Display transaction number
        /// </summary>
        public virtual string TransactionNumber { get; set; }
        public virtual OpenIdUser OpenIDUser { get; set; }

        //TODO: get rid of this field
        //public virtual string PaymentConfirmation { get; set; }

        //TODO: get rid of this field
        /// <summary>
        /// uPay reference number
        /// </summary>
        //public virtual int? ReferenceNumber { get; set; }
        //TODO: get rid of this field
        /// <summary>
        /// uPay tracking id
        /// </summary>
        //public virtual int? TrackingId { get; set; }

        //TODO: get rid of this field
        //public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; }
        
        public virtual ICollection<TransactionAnswer> TransactionAnswers { get; set; }
        public virtual ICollection<QuantityAnswer> QuantityAnswers { get; set; }
        public virtual ICollection<Transaction> ChildTransactions { get; set; }

        ////TODO: get rid of this method
        //public virtual void AddCheck(Check check)
        //{
        //    check.Transaction = this;
        //    Checks.Add(check);
        //}

        ////TODO: get rid of this method
        //public virtual void RemoveCheck(Check check)
        //{
        //    Checks.Remove(check);
        //}

        public virtual void AddPaymentLog(PaymentLog paymentLog)
        {
            paymentLog.Transaction = this;
            PaymentLogs.Add(paymentLog);
        }

        public virtual void RemovePaymentLog(PaymentLog paymentLog)
        {
            PaymentLogs.Remove(paymentLog);
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

        /// <summary>
        /// Only donations
        /// </summary>
        public virtual decimal DonationTotal { 
            get
            {
                return ChildTransactions.Where(a => a.Donation).Sum(a => a.Amount);
            } 
        }

        /// <summary>
        /// All non donation values
        /// </summary>
        public virtual decimal AmountTotal
        {
            get
            {
                return Amount + ChildTransactions.Where(a => !a.Donation).Sum(a => a.Amount);
            }
        }

        /// <summary>
        /// Donation and Non-Donation values.
        /// </summary>
        public virtual decimal Total
        {
            get
            {
                return DonationTotal + AmountTotal;
            }
        }

        /// <summary>
        /// Returns the total amount of money paid by the shopper
        /// </summary>
        public virtual decimal TotalPaid
        {
            get
            {
                return PaymentLogs.Where(a => a.Accepted).Sum(a => a.Amount);
            }
        }

        /// <summary>
        /// Returns the total amount of money paid by check
        /// </summary>
        public virtual decimal TotalPaidByCheck
        {
            get
            {
                return PaymentLogs.Where(a => a.Accepted && a.Check).Sum(a => a.Amount);
            }
        }

        /// <summary>
        /// Returns the total amount of money paid by credit card
        /// </summary>
        public virtual decimal TotalPaidByCredit
        {
            get
            {
                return PaymentLogs.Where(a => a.Accepted && a.Credit).Sum(a => a.Amount);
            }
        }

        public virtual bool Paid
        {
            get
            {
                return TotalPaid == Total;
            }
        }
    }
}
