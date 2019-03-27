using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class Transaction : DomainObject
    {
        #region Constructors
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
            IsActive = true;
            Refunded = false;
            Notified = false;

            PaymentLogs = new List<PaymentLog>();
            TransactionAnswers = new List<TransactionAnswer>();
            QuantityAnswers = new List<QuantityAnswer>();
            ChildTransactions = new List<Transaction>();
            RegularAmount = false;
            CorrectionAmount = false;
            CorrectionTotalAmount = false;
            //CorrectionTotalAmountPaid = false;

            PaymentType = false;

            TransactionGuid = Guid.NewGuid();

            if (Item != null && FinancialAccount == null)
            {
                FinancialAccount = Item.FinancialAccount;
            }
        }
        #endregion

        #region Mapped Fields
        [Required]
        public virtual Item Item { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool Credit { get; set; }
        public virtual bool Check { get; set; }
        public virtual Coupon Coupon { get; set; }

        public virtual bool Refunded { get; set; }
        public virtual bool Notified { get; set; }
        public virtual DateTime? NotifiedDate { get; set; }

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

        //CreatedBy has a max size of 50 characters in the database, but we don't really care as it should only use the Kerbors Id.
        public virtual string CreatedBy { get; set; }
        public virtual string CorrectionReason { get; set; }

        public virtual Guid TransactionGuid { get; set; }
        
        public virtual FinancialAccount FinancialAccount { get; set; }

        [Required]
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; }
        [Required]
        public virtual ICollection<TransactionAnswer> TransactionAnswers { get; set; }
        [Required]
        public virtual ICollection<QuantityAnswer> QuantityAnswers { get; set; }
        [Required]
        public virtual ICollection<Transaction> ChildTransactions { get; set; }
        #endregion

        #region Methods
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
        /// Only donations (Less corrections)
        /// </summary>
        public virtual decimal DonationTotal 
        { 
            get
            {
                return ChildTransactions.Where(a => a.Donation).Sum(a => a.Amount) + CorrectionTotal;
            } 
        }

        public virtual decimal UncorrectedDonationTotal
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
                return ChildTransactions.Where(a => !a.Donation && a.CreatedBy != null && !a.Refunded).Sum(a => a.Amount);
            }
        }

        public virtual decimal RefundAmount
        {
            get
            {
                return ChildTransactions.Where(a => !a.Donation && a.Refunded && a.IsActive).Sum(a => a.Amount);
            }
        }

        public virtual bool RefundIssued
        {
            get
            {
                return ChildTransactions.Where(a => a.Refunded && a.IsActive).Any();
            }
        }

        /// <summary>
        /// All non donation values
        /// </summary>
        public virtual decimal AmountTotal
        {
            //JCS Filter out corrections
            get
            {
                return Amount + ChildTransactions.Where(a => !a.Donation && !a.Refunded && a.Amount > 0).Sum(a => a.Amount);
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
                return PaymentLogs.Where(a => a.Accepted).Sum(a => a.Amount) - RefundAmount;
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="Transaction"/> is paid.
        /// Modified so if they paid more, we still say it is paid - JCS
        /// </summary>
        /// <value><c>true</c> if paid; otherwise, <c>false</c>.</value>
        public virtual bool Paid
        {
            get
            {
                return TotalPaid >= Total;
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
            PaymentType = false;
            if (Credit == Check)
            {
                PaymentType = true;
            }

            //We only allow amount to be negative for corrections (CreatedBy is Populated)
            RegularAmount = false;
            if(Amount < 0 && (CreatedBy == null || Donation))
            {
                RegularAmount = true;
            }
            //We don't want corrections to have a positive value
            CorrectionAmount = false;
            if(Amount >= 0 && CreatedBy != null && !Donation && !Refunded)
            {
                CorrectionAmount = true;
            }
            //We only want corrections to be ale to reduce the donation amount
            CorrectionTotalAmount = false;
            if(ChildTransactions != null && UncorrectedDonationTotal + CorrectionTotal < 0)
            {
                CorrectionTotalAmount = true;
            }
            ////We don't want a "refund" situation.
            //CorrectionTotalAmountPaid = true;
            //if(ChildTransactions != null && TotalPaid > Total)
            //{
            //    CorrectionTotalAmountPaid = false;
            //}
        }
        #endregion

        #region Fields ONLY used for complex validation, not in database
        [AssertFalse(ErrorMessage = "Payment type was not selected.")]
        public virtual bool PaymentType { get; set; }

        [AssertFalse(ErrorMessage = "Amount must be zero or more.")]
        public virtual bool RegularAmount { get; set; }

        [AssertFalse(ErrorMessage = "Amount must be less than zero.")]
        public virtual bool CorrectionAmount { get; set; }

        [AssertFalse(ErrorMessage = "The total of all correction amounts must not exceed the donation amounts")]
        public virtual bool CorrectionTotalAmount { get; set; }

        //[AssertTrue(Message = "The total of all correction amounts must not exceed the amount already paid")]
        //private bool CorrectionTotalAmountPaid { get; set; }
        #endregion Fields ONLY used for complex validation, not in database


        public virtual Dictionary<string, string> GetPaymentDictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"transaction_type"       , "sale"},
                {"reference_number"       , Id.ToString()},             // use the actual id so we can find it easily
                {"amount"                 , Total.ToString("F2")},
                {"currency"               , "USD"},
                {"transaction_uuid"       , Guid.NewGuid().ToString()},
                {"signed_date_time"       , DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")},
                {"unsigned_field_names"   , string.Empty},
                {"locale"                 , "en"},
                //{"bill_to_email"          , CustomerEmail},
                //{"bill_to_forename"       , CustomerName},
                {"bill_to_address_country", "US"},
                {"bill_to_address_state"  , "CA"}
            };

            //dictionary.Add("line_item_count", Items.Count.ToString("D"));
            //for (var i = 0; i < Items.Count; i++)
            //{
            //    dictionary.Add($"item_{i}_name", Items[i].Description);
            //    //dictionary.Add($"item_{i}_quantity", Items[i].Quantity.ToString());
            //    dictionary.Add($"item_{i}_unit_price", Items[i].Total.ToString("F2"));
            //}

            return dictionary;
        }
    }
}
