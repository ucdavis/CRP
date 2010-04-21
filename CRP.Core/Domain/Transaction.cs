using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
            TransactionDate = SystemTime.Now();
            Credit = false;
            Check = false;
            Amount = 0.0m;
            Donation = false;

            PaymentLogs = new List<PaymentLog>();
            TransactionAnswers = new List<TransactionAnswer>();
            QuantityAnswers = new List<QuantityAnswer>();
            ChildTransactions = new List<Transaction>();
            RegularAmount = false;
            CorrectionAmount = false;
            CorrectionTotalAmount = false;

            PaymentType = false;
        }

        [NotNull]
        public virtual Item Item { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual bool Credit { get; set; }
        public virtual bool Check { get; set; }
        //public virtual bool Paid { get; set; }
        //[RangeDouble(Min = 0.00, Message = "must be zero or more")]
        public virtual decimal Amount { get; set; }
        public virtual bool Donation { get; set; }
        public virtual int Quantity { get; set; } 
        /// <summary>
        /// The parent transaction object, this is only populated for donation fields.
        /// </summary>
        public virtual Transaction ParentTransaction { get; set; }
        /// <summary>
        /// Display transaction number
        /// Database calculated field
        /// </summary>
        public virtual string TransactionNumber { get; set; }
        public virtual OpenIdUser OpenIDUser { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string CorrectionReason { get; set; }

        [NotNull]
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; }
        [NotNull]
        public virtual ICollection<TransactionAnswer> TransactionAnswers { get; set; }
        [NotNull]
        public virtual ICollection<QuantityAnswer> QuantityAnswers { get; set; }
        [NotNull]
        public virtual ICollection<Transaction> ChildTransactions { get; set; }


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
        public virtual decimal DonationTotal 
        { 
            get
            {
                return ChildTransactions.Where(a => a.Donation).Sum(a => a.Amount);
            } 
        }

        /// <summary>
        /// Only Corrections
        /// </summary>
        public virtual decimal CorrectionTotal
        {
            get
            {
                return ChildTransactions.Where(a => !a.Donation).Sum(a => a.Amount);
            }
        }

        /// <summary>
        /// All non donation values
        /// </summary>
        public virtual decimal AmountTotal
        {
            //TODO: JCS - Get all non-negative non donation amounts?
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

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            PaymentType = true;
            if (Credit == Check)
            {
                PaymentType = false;
            }

            //We only allow amount to be negative for corrections (CreatedBy is Populated)
            RegularAmount = true;
            if(Amount < 0 && (CreatedBy == null || Donation))
            {
                RegularAmount = false;
            }
            CorrectionAmount = true;
            if(Amount >= 0 && CreatedBy != null && !Donation)
            {
                CorrectionAmount = false;
            }

            CorrectionTotalAmount = true;
            if(ChildTransactions != null && DonationTotal + CorrectionTotal <0)
            {
                CorrectionTotalAmount = false;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "Payment type was not selected.")]
        private bool PaymentType { get; set; }

        [AssertTrue(Message = "Amount must be zero or more.")]
        private bool RegularAmount { get; set; }

        [AssertTrue(Message = "Amount must be less than zero.")]
        private bool CorrectionAmount { get; set; }

        [AssertTrue(Message = "The total of all correction amounts must not exceed the donation amounts")]
        private bool CorrectionTotalAmount { get; set; }
        #endregion Fields ONLY used for complex validation, not in database

    }
}
