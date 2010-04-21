using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class CouponController : SuperController
    {
        //
        // GET: /Coupon/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Coupon/Create/{itemId}
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [UserOnlyAttribute]
        public ActionResult Create(int itemId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            var viewModel = CouponViewModel.Create(Repository, item);

            return View(viewModel);
        }

        /// <summary>
        /// POST: /Coupon/Create/{itemId}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Generates a new coupon for the specified item
        /// PreCondition:
        ///     Item is valid
        /// PostCondition:
        ///     Coupon is created
        ///     Random coupon code is assigned
        ///     Coupon is associated with item
        ///     User is marked in the coupon
        /// </remarks>
        /// <param name="itemId"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        [AcceptPost]
        [Authorize(Roles = "User")]
        public ActionResult Create(int itemId, [Bind(Exclude="Id")]Coupon coupon)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            coupon.Code = CouponGenerator.GenerateCouponCode();
            coupon.UserId = CurrentUser.Identity.Name;
            coupon.Item = item;

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, coupon.ValidationResults());


            if (ModelState.IsValid)
            {
                Repository.OfType<Coupon>().EnsurePersistent(coupon);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Coupon");
                //return Redirect(ReturnUrlGenerator.EditItemUrl(item.Id, StaticValues.Tab_Coupons));
                return Redirect(Url.EditItemUrl(item.Id, StaticValues.Tab_Coupons));
            }

            var viewModel = CouponViewModel.Create(Repository, item);
            viewModel.Coupon = coupon;
            return View(viewModel);
        }

        /// <summary>
        /// GET: /Coupon/Validate/{couponCode}
        /// 
        /// Validate the coupon code to make sure it can still be used.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="couponCode">The coupon code.</param>
        /// <returns></returns>
        public JsonNetResult Validate(int itemId, string couponCode)
        {            
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            var coupon = Repository.OfType<Coupon>().Queryable.Where(a => a.Code == couponCode && a.Item == item && a.IsActive).FirstOrDefault();

            if (item == null || coupon == null)
            {
                return new JsonNetResult(new {message = "Invalid code."});
            }

            var discountAmount = coupon.ValidateCoupon(null, 1, true);

            //Done: This needs to work with Coupons that are unlimited.
            if (!discountAmount.HasValue) //Suggestion to fix failing test            
            {
                return new JsonNetResult(new { message = "Coupon has already been redeemed." });
            }

            return new JsonNetResult(new {discountAmount = discountAmount.Value, maxQuantity = coupon.MaxQuantity.HasValue ? coupon.MaxQuantity.Value : -1});
        }

        /// <summary>
        /// POST: /Coupon/Deactivate/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Deactivates an active coupon.
        /// PreCondition:
        ///     Coupon is active
        ///     Coupon is not used
        /// PostCondition:
        ///     Coupon's "IsActive" flag is false
        /// </remarks>
        /// <param name="couponId"></param>
        /// <returns></returns>
        [AcceptPost]
        [Authorize(Roles = "User")]
        public ActionResult Deactivate(int couponId)
        {            
            var coupon = Repository.OfType<Coupon>().GetNullableByID(couponId);

            if (coupon == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            //Done: This needs to work with Coupons that are unlimited.
            if (!coupon.IsActive || (coupon.Used && !coupon.Unlimited)) 
            {
                //return Redirect(ReturnUrlGenerator.EditItemUrl(coupon.Item.Id, StaticValues.Tab_Coupons));
                return Redirect(Url.EditItemUrl(coupon.Item.Id, StaticValues.Tab_Coupons));
            }

            coupon.IsActive = false;

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, coupon.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Coupon>().EnsurePersistent(coupon);
                Message = NotificationMessages.STR_Deactivated.Replace(NotificationMessages.ObjectType, "Coupon");
            }
            else
            {
                Message = NotificationMessages.STR_UnableToUpdate.Replace(NotificationMessages.ObjectType, "Coupon");
            }

            // redirect to edit with the anchor to coupon
            //return Redirect(ReturnUrlGenerator.EditItemUrl(coupon.Item.Id, StaticValues.Tab_Coupons));
            return Redirect(Url.EditItemUrl(coupon.Item.Id, StaticValues.Tab_Coupons));
        }
    }
}
