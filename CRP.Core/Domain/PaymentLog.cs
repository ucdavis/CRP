using System;
using System.Collections.Generic;
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
            CheckNumberRequired = false;
            NameRequired = false;
            AmountRequired = false;
            GatewayTransactionIdRequired = false;
            CardTypeRequired = false;
            CheckOrCredit = false;
            DisplayCheckInvalidMessage = false;
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
            CheckNumberRequired = true;
            NameRequired = true;
            AmountRequired = true;
            GatewayTransactionIdRequired = true;
            CardTypeRequired = true;
            CheckOrCredit = true;
            if (Check == Credit)
            {
                CheckOrCredit = false;
            }
            else
            {
                if (Credit)
                {
                    if (Accepted)
                    {
                        CommonChecksForComplexLogicFields();
                        if (GatewayTransactionId == null || string.IsNullOrEmpty(GatewayTransactionId.Trim()))
                        {
                            GatewayTransactionIdRequired = false;
                        }
                        if (CardType == null || string.IsNullOrEmpty(CardType.Trim()))
                        {
                            CardTypeRequired = false;
                        }
                    }
                }
                if (Check)
                {
                    CommonChecksForComplexLogicFields();
                    if (CheckNumber == null || CheckNumber <= 0)
                    {
                        CheckNumberRequired = false;
                    }
                }
            }
        }

        /// <summary>
        /// Common checks for complex logic fields.
        /// Name and amount
        /// </summary>
        private void CommonChecksForComplexLogicFields()
        {
            if (Name == null || string.IsNullOrEmpty(Name.Trim()))
            {
                NameRequired = false;
            }
            if (Amount < 0.01m)
            {
                AmountRequired = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [display check invalid message].
        /// This is not is the database, it is used in the view model 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [display check invalid message]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool DisplayCheckInvalidMessage { get; set; }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "Check number required when credit card not used.")]
        private bool CheckNumberRequired { get; set; }

        [AssertTrue(Message = "Payee name required.")]
        private bool NameRequired { get; set; }

        [AssertTrue(Message = "Amount must be more than 1 cent.")]
        private bool AmountRequired { get; set; }

        [AssertTrue(Message = "Gateway Transaction Id Required.")]
        private bool GatewayTransactionIdRequired { get; set; }

        [AssertTrue(Message = "Card Type Required.")]
        private bool CardTypeRequired { get; set; }

        [AssertTrue(Message = "Check or Credit must be selected.")]
        private bool CheckOrCredit { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
