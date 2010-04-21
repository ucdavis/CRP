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
        }

        private void SetDefaults()
        {
            Unlimited = false;
            Used = false;   
        }

        [Required]
        [Length(10)]
        public virtual string Code { get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        public virtual bool Unlimited { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Used { get; set; }
        public virtual decimal DiscountAmount { get; set; }
        [Required]
        [Length(50)]
        public virtual string UserId { get; set; }
    }
}
