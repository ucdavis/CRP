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
        [RangeDouble(Min = 0.01, Message = "must be more than $0.00")]
        public virtual decimal DiscountAmount { get; set; }
        /// <summary>
        /// User login id of the user creating the coupon
        /// </summary>
        [Required]
        [Length(50)]
        public virtual string UserId { get; set; }

        public virtual bool IsActive { get; set; }

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
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "An unlimited coupon requires an email")]
        public virtual bool UnlimitedAndEmail { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
