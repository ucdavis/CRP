using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CRP.Core.Helpers;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Coupon : DomainObject
    {
        #region Constructors
        public Coupon()
        {
            SetDefaults();
        }

        public Coupon(string code, Item item, string userId)
        {
            Code = code;
            Item = item;
            UserId = userId;

            SetDefaults();
        }

        private void SetDefaults()
        {
            IsActive = true;
            Transactions = new List<Transaction>();
        }
        #endregion

        #region Mapped Fields
        [Required]
        [StringLength(10, MinimumLength = 10)]
        public virtual string Code { get; set; }
        [Required]
        public virtual Item Item { get; set; }

        /// <summary>
        /// The maximum number of quantity use per transaction
        /// </summary>
        public virtual int? MaxQuantity { get; set; }
        /// <summary>
        /// The maximum number of times it can be used
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public virtual int MaxUsage { get; set; }

        /// <summary>
        /// If not null, the date when the coupon has expired
        /// </summary>
        public virtual DateTime? Expiration { get; set; }
        /// <summary>
        /// If specified, the coupon can only be used for transactions with a matching contact info's email
        /// </summary>
        [StringLength(100)]
        public virtual string Email { get; set; }

        [Range(0.01, 922337203685477.00, ErrorMessage = "must be more than $0.00")]
        public virtual decimal DiscountAmount { get; set; }
        /// <summary>
        /// User login id of the user creating the coupon
        /// </summary>
        [Required]
        [StringLength(50)]
        public virtual string UserId { get; set; }
        /// <summary>
        /// If the coupon has not been deactivated
        /// </summary>
        public virtual bool IsActive { get; set; }



        /// <summary>
        /// Transactions that have used the coupons
        /// </summary>
        [Required]
        public virtual IList<Transaction> Transactions { get; set; }


        ///// <summary>
        ///// The coupon can be used for an unlimited number of transactions
        ///// 
        ///// phasing this field out
        ///// </summary>
        //public virtual bool Unlimited { get; set; }
        ///// <summary>
        ///// If the coupon has been used at least once
        ///// 
        ///// phasing this field out, switching to calculated field
        ///// </summary>
        //public virtual bool Used { get; set; }
        #endregion

        #region Methods
        public virtual decimal UseCoupon(string email, int quantity)
        {
            // call the validate coupon
            var discount = ValidateCoupon(email, quantity, false);

            // coupon is valid and usable
            if (discount.HasValue)
            {
                return discount.Value;
            }

            // coupon is not valid discount is 0.
            return 0.0m;
        }

        /// <summary>
        /// Validates to tell you if the coupon is valid for use or not.  Will check against email, usage and date.
        /// If you are actually using the coupon, do not use this function directly, call UseCoupon(email, quantity)
        /// </summary>
        /// <param name="email">Pass null if you just want to know if it's valid or not based on usage and date.</param>
        /// <param name="quantity"></param>
        /// <param name="ignoreEmail">Set to true just for checkout validation.  Always use false when using the coupon.</param>
        /// <returns></returns>
        public virtual decimal? ValidateCoupon(string email, int quantity, bool ignoreEmail)
        {
            // check against isactive or expiration
            if (!IsAvailabeForUsage()) return null;

            // check against the max number of usages if other valid conditions pass
            // usages has already surpassed max usage
            if (CalculateUsage() > MaxUsage) return null;

            // check for email restriction
            if (!string.IsNullOrEmpty(Email) && !ignoreEmail)
            {
                // email matches, do check
                if (!string.IsNullOrEmpty(email) && Email.ToLower() == email.ToLower())
                {
                    return CalculateDiscount(quantity);
                }
                // no match on email
                return null;
            }

            // no email restriction and it's valid
            return CalculateDiscount(quantity);
        }

        /// <summary>
        /// Returns if this coupon can even be used based on rules for coupon
        /// Does not take into account availablility of quantity
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAvailabeForUsage()
        {
            // coupon is not active
            if (!IsActive || (Expiration.HasValue && Expiration.Value < DateTime.UtcNow.ToPacificTime()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates # of times the coupon has been used
        /// </summary>
        /// <returns></returns>
        private int CalculateUsage()
        {
            // determine what the max quantitty per transaction is
            var maxq = MaxQuantity.HasValue ? MaxQuantity.Value : int.MaxValue;

            // count the number used for each transaction
            // if max quantity is higher than the transaction quantity, then return the transaction quantity
            // but if the quantity is greater than the max, only return the max becuase that is the most that should
            // have been allowed.
            return Transactions.Sum(a => maxq > a.Quantity ? a.Quantity : maxq);
        }

        /// <summary>
        /// Calculates the discount amount based on quantity available and # attempted to use
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private decimal CalculateDiscount(int quantity)
        {
            // calculate availabe left, if a max usage is defined
            var quantityAvailable = MaxUsage - CalculateUsage();

            // we need to consider a max quantity
            if (MaxQuantity.HasValue)
            {
                // take the lower of the two values, to restrict
                var maxDiscountQuantity = quantityAvailable < MaxQuantity ? quantityAvailable : MaxQuantity.Value;

                // quantity is less than max, so take discount on all quantity
                if (maxDiscountQuantity > quantity)
                {
                    return quantity*DiscountAmount;
                }
                // quantity if greater than max, discount only max number
                else
                {
                    return maxDiscountQuantity*DiscountAmount;
                }
            }
            
            // just take the discount off all attendees
            return quantityAvailable > quantity ? quantity*DiscountAmount : quantityAvailable*DiscountAmount;   
        }

        /// <summary>
        /// Calculates the most that this coupon can apply to.
        /// </summary>
        /// <remarks>
        /// Takes the lowest of max quantity or the max overall usage.
        /// </remarks>
        /// <returns></returns>
        public virtual int MaxAvailableForUsage()
        {
            var quantityAvailable = MaxUsage - CalculateUsage();

            // take into acct max quantity
            if (MaxQuantity.HasValue)
            {
                return MaxQuantity.Value > quantityAvailable ? quantityAvailable : MaxQuantity.Value;
            }

            // no max quantity, just return available
            return quantityAvailable;
        }
        #endregion

        #region Fields ONLY used for complex validation, not in database
        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override System.Collections.Generic.ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        private void PopulateComplexLogicFields()
        {
            DiscountAmountCostPerItem = false;
            if(Item != null && Item.CostPerItem < DiscountAmount)
            {
                DiscountAmountCostPerItem = true;
            }
        }

        [AssertFalse(ErrorMessage = "The discount amount must not be greater than the cost per item.")]
        public virtual bool DiscountAmountCostPerItem { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }


}
