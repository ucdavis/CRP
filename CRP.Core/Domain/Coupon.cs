using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Coupon : DomainObject
    {
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
            Unlimited = false;
            Used = false;
            IsActive = true;
            DiscountAmountCostPerItem = false;
            UnlimitedAndEmail = false;
        }

        [Required]
        [Length(Min = 10, Max = 10)]
        public virtual string Code { get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        public virtual bool Unlimited { get; set; }
        public virtual DateTime? Expiration { get; set; }
        [Length(100)]
        public virtual string Email { get; set; }
        public virtual bool Used { get; set; }
        [RangeDouble(Min = 0.01, Max = 922337203685477.00, Message = "must be more than $0.00")]
        public virtual decimal DiscountAmount { get; set; }
        /// <summary>
        /// User login id of the user creating the coupon
        /// </summary>
        [Required]
        [Length(50)]
        public virtual string UserId { get; set; }

        /// <summary>
        /// The maximum number of quantity use per transaction
        /// </summary>
        public virtual int? MaxQuantity { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual decimal UseCoupon(string email, int quantity)
        {
            // call the validate coupon
            var discount = ValidateCoupon(email, quantity, false);

            // coupon is valid and usable
            if (discount.HasValue)
            {
                // set the coupon as used
                Used = true;

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
            // coupon has been used but isn't unlimied, is inactive or has passed expiration
            if ((Used && !Unlimited) || (!IsActive) || (Expiration.HasValue && Expiration.Value > DateTime.Now))
            {
                return null;
            }

            // has an email restriction
            if (!string.IsNullOrEmpty(Email) && !ignoreEmail)
            {
                if (Email == email)
                {
                    return CalculateDiscount(quantity);
                }
                else
                {
                    return null;
                }
            }

            // no email restriction and it's valid
            return CalculateDiscount(quantity);
        }

        private decimal CalculateDiscount(int quantity)
        {
            // we need to consider a max quantity
            if (MaxQuantity.HasValue)
            {
                // quantity is less than max, so take discount on all quantity
                if (MaxQuantity > quantity)
                {
                    return quantity*DiscountAmount;
                }
                // quantity if greater than max, discount only max number
                else
                {
                    return MaxQuantity.Value*DiscountAmount;
                }
            }
            
            // just take the discount off all attendees
            return quantity*DiscountAmount;   
        }

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
            UnlimitedAndEmail = true;
            if (Unlimited && string.IsNullOrEmpty(Email != null ? Email.Trim() : string.Empty))
            {
                UnlimitedAndEmail = false;
            }

            DiscountAmountCostPerItem = true;
            if(Item != null && Item.CostPerItem < DiscountAmount)
            {
                DiscountAmountCostPerItem = false;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "An unlimited coupon requires an email")]
        private bool UnlimitedAndEmail { get; set; }

        [AssertTrue(Message = "The discount amount must not be greater than the cost per item.")]
        private bool DiscountAmountCostPerItem { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
