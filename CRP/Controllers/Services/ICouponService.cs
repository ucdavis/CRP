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
        string Create(Item item, string email, bool unlimited, DateTime? expiration, decimal discountAmount, string userId, int? maxUsage, int? maxQuantity, ModelStateDictionary modelState = null);
        string Create(Item item, Coupon coupon, string userId, ModelStateDictionary modelState = null);
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

        public string Create(Item item, Coupon coupon, string userId, ModelStateDictionary modelState = null)
        {
            Check.Require(item != null, "item is required.");
            Check.Require(coupon != null, "coupon is required.");

            // set the information that needs to be set for every coupon
            // do not trust what was passed
            coupon.Code = CouponGenerator.GenerateCouponCode();
            coupon.UserId = userId;
            coupon.Item = item;

            // validate the coupon
            modelState = modelState ?? new ModelStateDictionary();
            MvcValidationAdapter.TransferValidationMessagesTo(modelState, coupon.ValidationResults());

            // persist, if valid
            if (modelState.IsValid)
            {
                _couponRepository.EnsurePersistent(coupon);
            }

            // return the coupon code if it's valid, otherwise return an empty string
            return modelState.IsValid ? coupon.Code : string.Empty;
        }

        public string Create(Item item, string email, bool unlimited, DateTime? expiration, decimal discountAmount, string userId, int? maxUsage, int? maxQuantity, ModelStateDictionary modelState = null)
        {
            Check.Require(item != null, "item is required.");
            Check.Require(modelState != null, "modelState is required.");

            // create the coupon and set the fields
            var coupon = new Coupon();
            coupon.Unlimited = unlimited;
            coupon.Expiration = expiration;
            coupon.Email = email;
            coupon.DiscountAmount = discountAmount;
            coupon.MaxQuantity = maxQuantity;
            coupon.MaxUsage = maxUsage;

            return Create(item, coupon, userId, modelState);
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
            if (!coupon.IsActive || (coupon.Used && !coupon.Unlimited)) return false;

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
    }
}