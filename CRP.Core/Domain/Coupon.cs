using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
            Unlimited = false;
            Used = false;
            IsActive = true;
            DiscountAmountCostPerItem = false;
            UnlimitedAndEmail = false;
            Transactions = new List<Transaction>();
        }
        #endregion

        #region Mapped Fields
        [Required]
        [Length(Min = 10, Max = 10)]
        public virtual string Code { get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        /// <summary>
        /// The coupon can be used for an unlimited number of transactions
        /// </summary>
        public virtual bool Unlimited { get; set; }
        /// <summary>
        /// If not null, the date when the coupon has expired
        /// </summary>
        public virtual DateTime? Expiration { get; set; }
        /// <summary>
        /// If specified, the coupon can only be used for transactions with a matching contact info's email
        /// </summary>
        [Length(100)]
        public virtual string Email { get; set; }
        /// <summary>
        /// If the coupon has been used at least once
        /// </summary>
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

        /// <summary>
        /// If the coupon has not been deactivated
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// The maximum number of times it can be used
        /// </summary>
        [Min(0)]
        public virtual int? MaxUsage { get; set; }

        /// <summary>
        /// Transactions that have used the coupons
        /// </summary>
        [NotNull]
        public virtual IList<Transaction> Transactions { get; set; }
        #endregion

        #region Methods
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
            // coupon has been used but isn't unlimied or has max usage defined, is inactive or has passed expiration
            if (((Used && !Unlimited) && (Used && !MaxUsage.HasValue)) || (!IsActive) || (Expiration.HasValue && Expiration.Value < DateTime.Now))
            {
                return null;
            }

            // check against the max number of usages if other valid conditions pass
            // usages has already surpassed max usage
            if (MaxUsage.HasValue && CalculateUsage() > MaxUsage.Value)
            {
                return null;
            }

            // has an email restriction
            if (!string.IsNullOrEmpty(Email) && !ignoreEmail)
            {
                if (!string.IsNullOrEmpty(email) && Email.ToLower() == email.ToLower())
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
            // calculate availabe left, if a max usage is defined
            var quantityAvailable = int.MaxValue;
            if (MaxUsage.HasValue)
            {
                quantityAvailable = MaxUsage.Value - CalculateUsage();
            }

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
            if (!Unlimited && string.IsNullOrEmpty(Email != null ? Email.Trim() : string.Empty))
            {
                UnlimitedAndEmail = false;
            }

            DiscountAmountCostPerItem = true;
            if(Item != null && Item.CostPerItem < DiscountAmount)
            {
                DiscountAmountCostPerItem = false;
            }

            UnlimitedAndMaxUsage = true;
            if (Unlimited && MaxUsage != null)
            {
                UnlimitedAndMaxUsage = false;
            }
        }

        /// <summary>
        /// Calculates # of times the coupon has been used
        /// </summary>
        /// <returns></returns>
        public virtual int CalculateUsage()
        {
            // determine what the max quantitty per transaction is
            var maxq = MaxQuantity.HasValue ? MaxQuantity.Value : int.MaxValue;

            // count the number used for each transaction
            // if max quantity is higher than the transaction quantity, then return the transaction quantity
            // but if the quantity is greater than the max, only return the max becuase that is the most that should
            // have been allowed.
            return Transactions.Sum(a => maxq > a.Quantity ? a.Quantity : maxq);
        }
        #endregion

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "When not unlimited a coupon requires an email")]
        private bool UnlimitedAndEmail { get; set; }

        [AssertTrue(Message = "The discount amount must not be greater than the cost per item.")]
        private bool DiscountAmountCostPerItem { get; set; }

        [AssertTrue(Message="Cannot have unlimited and a max usage defined, one or the other.")]
        private bool UnlimitedAndMaxUsage { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
