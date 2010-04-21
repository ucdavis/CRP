using System.Linq;
using System.Web.Mvc;
using CRP.App_GlobalResources;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Controller;
using MvcContrib;
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
        public ActionResult Create(int itemId, [Bind(Exclude="Id")]Coupon coupon)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            coupon.Code = CouponGenerator.GenerateCouponCode();
            coupon.UserId = CurrentUser.Identity.Name;

            item.AddCoupon(coupon);

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, coupon.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Coupon");
                return this.RedirectToAction<ItemManagementController>(a => a.Edit(item.Id));
            }

            var viewModel = CouponViewModel.Create(Repository, item);
            viewModel.Coupon = coupon;
            return View(viewModel);
        }

        /// <summary>
        /// GET: /Coupon/Validate/{couponCode}
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public JsonNetResult Validate(int itemId, string couponCode)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            var coupon = Repository.OfType<Coupon>().Queryable.Where(a => a.Code == couponCode).FirstOrDefault();

            if (item == null || coupon == null)
            {
                return new JsonNetResult("Invalid code.");
            }

            if (coupon.Item != item)
            {
                return new JsonNetResult("Invalid code.");
            }

            return new JsonNetResult(coupon.DiscountAmount);
        }
    }
}
