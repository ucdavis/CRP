using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Validator;

namespace CRP.Controllers.Services
{
    public interface ICouponService
    {
        string Create(Item item, string email, DateTime? expiration, decimal discountAmount, string userId, int? maxUsage, int? maxQuantity, string couponType, ModelStateDictionary modelState = null);
        string Create(Item item, Coupon coupon, string userId, string couponType, ModelStateDictionary modelState = null);
        bool Deactivate(Coupon coupon, ModelStateDictionary modelState = null);
        bool Validate(Item item, Coupon coupon, ref decimal discountAmount, ref int maxQuantity, ref string message);
    }

    public class CouponService : ICouponService
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<Coupon> _couponRepository;

        public CouponService(IRepository<Item> itemRepository, IRepository<Coupon> couponRepository)
        {
            _itemRepository = itemRepository;
            _couponRepository = couponRepository;
        }

        public string Create(Item item, Coupon coupon, string userId, string couponType, ModelStateDictionary modelState = null)
        {
            Check.Require(item != null, "item is required.");
            Check.Require(coupon != null, "coupon is required.");

            // set the information that needs to be set for every coupon
            // do not trust what was passed
            coupon.Code = CouponGenerator.GenerateCouponCode();
            coupon.UserId = userId;
            coupon.Item = item;
            coupon.MaxUsage = CalculateMaxUsage(couponType, coupon);

            // validate the coupon
            modelState = modelState ?? new ModelStateDictionary();
            MvcValidationAdapter.TransferValidationMessagesTo(modelState, coupon.ValidationResults());
            if (coupon.MaxUsage == 0)
            {
                modelState.AddModelError("Coupon.MaxUsage", "When Limited Usage, maximum must be at least 1");
            }

            // persist, if valid
            if (modelState.IsValid)
            {
                _couponRepository.EnsurePersistent(coupon);
            }

            // return the coupon code if it's valid, otherwise return an empty string
            return modelState.IsValid ? coupon.Code : string.Empty;
        }

        public string Create(Item item, string email, DateTime? expiration, decimal discountAmount, string userId, int? maxUsage, int? maxQuantity, string couponType, ModelStateDictionary modelState = null)
        {
            Check.Require(item != null, "item is required.");

            modelState = modelState ?? new ModelStateDictionary();

            // create the coupon and set the fields
            var coupon = new Coupon();
            coupon.Expiration = expiration;
            coupon.Email = email;
            coupon.DiscountAmount = discountAmount;
            coupon.MaxQuantity = maxQuantity;
            coupon.MaxUsage = maxUsage.HasValue ? maxUsage.Value : -1;

            coupon.MaxUsage = CalculateMaxUsage(couponType, coupon);

            return Create(item, coupon, userId, couponType, modelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns>True if coupon was deactivated, False if something was wrong</returns>
        public bool Deactivate(Coupon coupon, ModelStateDictionary modelState = null)
        {
            Check.Require(coupon != null, "coupon is required.");

            // check if coupon is available for deactivation
            //if (!coupon.IsActive || (coupon.Used && !coupon.Unlimited)) return false;
            if (!coupon.IsAvailabeForUsage() && coupon.MaxAvailableForUsage() <= 0) return false;

            // deactivate the coupon
            coupon.IsActive = false;

            // validate the coupon
            modelState = modelState ?? new ModelStateDictionary();
            MvcValidationAdapter.TransferValidationMessagesTo(modelState, coupon.ValidationResults());

            // persist if valid
            if (modelState.IsValid)
            {
                _couponRepository.EnsurePersistent(coupon);
            }

            // returns the validity of the object, if it's valid it should have been deactivated
            return modelState.IsValid;
        }

        public bool Validate(Item item, Coupon coupon, ref decimal discountAmount, ref int maxQuantity,  ref string message)
        {
            // get item and coupon and validate we actually have something
            Check.Require(item != null, "item is required.");
            Check.Require(coupon != null, "coupon is required.");

            var discountAmt = coupon.ValidateCoupon(null, 1, true);

            if (!discountAmt.HasValue)
            {
                discountAmount = 0;
                maxQuantity = 0;
                message = "Coupon has already been redeemed.";
                return false;
            }

            maxQuantity = coupon.MaxAvailableForUsage();
            discountAmount = discountAmt.Value;
            message = string.Empty;

            return true;
        }

        public string Unlimited { get { return "Unlimited"; } }
        public string LimitedUsage { get { return "LimitedUsage"; } }
        public string SingleUsage { get { return "SingleUsage"; } }

        public int CalculateMaxUsage(string couponType, Coupon coupon)
        {
            if (couponType == Unlimited) return int.MaxValue;
            if (couponType == LimitedUsage) return coupon.MaxUsage;
            if (couponType == SingleUsage) return 1;

            return -1;
        }
    }
}