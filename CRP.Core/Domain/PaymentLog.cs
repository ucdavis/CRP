using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


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
        [StringLength(200)]
        public virtual string Name { get; set; }
        /// <summary>
        /// Amount paid
        /// </summary>
        public virtual decimal Amount { get; set; }
        public virtual DateTime DatePayment { get; set; }
        [Required]
        public virtual Transaction Transaction { get; set; }

        public virtual int? CheckNumber { get; set; }
        /// <summary>
        /// Payment gateway transaction id
        /// </summary>
        [StringLength(16)]
        public virtual string GatewayTransactionId { get; set; }
        /// <summary>
        /// Card type that was used to pay
        /// </summary>
        [StringLength(20)]
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

        #region TouchNet Return Values

        public virtual string TnStatus { get; set; } //S=Success, C=Canceled, E=Error
        public virtual string TnPaymentDate { get; set; }
        public virtual string TnSysTrackingId { get; set; }
        public virtual string TnBillingAddress1 { get; set; }
        public virtual string TnBillingAddress2 { get; set; }
        public virtual string TnBillingCity { get; set; }
        public virtual string TnBillingState { get; set; }
        public virtual string TnBillingZip { get; set; }
        public virtual string TnUpaySiteId { get; set; }
        public virtual string TnErrorLink { get; set; }
        public virtual string TnSubmit { get; set; }
        public virtual string TnSuccessLink { get; set; }
        public virtual string TnCancelLink { get; set; }


        #endregion TouchNet Return Values

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
            CheckNumberRequired = false;
            NameRequired = false;
            AmountRequired = false;
            GatewayTransactionIdRequired = true;
            CardTypeRequired = true;
            CheckOrCredit = false;
            if (Check == Credit)
            {
                CheckOrCredit = true;
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
                        CheckNumberRequired = true;
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
                NameRequired = true;
            }
            if (Amount < 0.01m)
            {
                AmountRequired = true;
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
        [AssertFalse(ErrorMessage = "Check number required when credit card not used.")]
        public virtual bool CheckNumberRequired { get; set; }

        [AssertFalse(ErrorMessage = "Payee name required.")]
        public virtual bool NameRequired { get; set; }

        [AssertFalse(ErrorMessage = "Amount must be more than 1 cent.")]
        public virtual bool AmountRequired { get; set; }

        [AssertFalse(ErrorMessage = "Gateway Transaction Id Required.")]
        public virtual bool GatewayTransactionIdRequired { get; set; }

        [AssertFalse(ErrorMessage = "Card Type Required.")]
        public virtual bool CardTypeRequired { get; set; }

        [AssertFalse(ErrorMessage = "Check or Credit must be selected.")]
        public virtual bool CheckOrCredit { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
