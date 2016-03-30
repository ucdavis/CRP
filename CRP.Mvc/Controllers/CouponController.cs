using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.Services;
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
    public class CouponController : ApplicationController
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

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
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
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
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Create(int itemId, [Bind(Exclude="Id")]Coupon coupon, string couponType)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            // validate and create the coupon
            _couponService.Create(item, coupon, CurrentUser.Identity.Name, couponType, ModelState);

            if (ModelState.IsValid)
            {
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Coupon");
                return Redirect(Url.EditItemUrl(item.Id, StaticValues.Tab_Coupons));
            }

            var viewModel = CouponViewModel.Create(Repository, item, couponType);
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
        /// <param name="quantity">Quantity to calculate</param>
        /// <returns></returns>
        public JsonNetResult Validate(int itemId, string couponCode)
        {            
            if(string.IsNullOrEmpty(couponCode))
            {
                return new JsonNetResult(new { discountAmount = 0, maxQuantity = 0, message = "" });
            }
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            var coupon = Repository.OfType<Coupon>().Queryable.Where(a => a.Code == couponCode && a.Item == item && a.IsActive).FirstOrDefault();

            if (item == null || coupon == null)
            {
                return new JsonNetResult(new { discountAmount = 0, maxQuantity = 0, message = "Invalid code." });
            }

            decimal discountAmt = 0m;
            int maxQty = 0;
            string msg = string.Empty;

            // validate the coupon, if true, then give the information so it can be displayed
            if (_couponService.Validate(item, coupon, ref discountAmt, ref maxQty, ref msg))
                return new JsonNetResult(new {discountAmount = discountAmt, maxQuantity = maxQty, totalDiscount = discountAmt});

            // there was a problem, return the values with the message
            return new JsonNetResult(new {discountAmount = discountAmt, maxQuantity = maxQty, message = msg });
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
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Deactivate(int couponId)
        {            
            var coupon = Repository.OfType<Coupon>().GetNullableById(couponId);

            if (coupon == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            _couponService.Deactivate(coupon, ModelState);

            if (ModelState.IsValid)
            {
                Message = NotificationMessages.STR_Deactivated.Replace(NotificationMessages.ObjectType, "Coupon");
            }
            else
            {
                Message = NotificationMessages.STR_UnableToUpdate.Replace(NotificationMessages.ObjectType, "Coupon");
            }

            // redirect to edit with the anchor to coupon
            return Redirect(Url.EditItemUrl(coupon.Item.Id, StaticValues.Tab_Coupons));
        }
    }
}