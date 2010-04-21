using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using System.Linq.Expressions;
using System.Linq;
using UCDArch.Web.ActionResults;


namespace CRP.Tests.Controllers
{
    [TestClass]
    public class CouponControllerRepositoryTests : ControllerTestBase<CouponController>
    {
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected readonly IPrincipal Principal = new MockPrincipal();
        protected IRepository<Coupon> CouponRepository { get; set; }
        protected List<Coupon> Coupons { get; set; }



        #region Init

        public CouponControllerRepositoryTests()
        {
            Controller.ControllerContext.HttpContext.User = Principal;

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Coupons = new List<Coupon>();
            CouponRepository = FakeRepository<Coupon>();
            Controller.Repository.Expect(a => a.OfType<Coupon>()).Return(CouponRepository).Repeat.Any();
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<CouponController>();
        }

        #endregion Init

        #region Route Tests

        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Coupon/Index".ShouldMapTo<CouponController>(a => a.Index());
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters1()
        {
            "~/Coupon/Create/5".ShouldMapTo<CouponController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters2()
        {
            "~/Coupon/Create/5".ShouldMapTo<CouponController>(a => a.Create(5, new Coupon()), true);
        }

        /// <summary>
        /// Test validates the ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestValidateIgnoreParameters()
        {
            "~/Coupon/Validate/5".ShouldMapTo<CouponController>(a => a.Validate(5, "test"), true);    
        }

        /// <summary>
        /// Tests the deactivate mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestDeactivateMappingIgnoreParameters()
        {
            "~/Coupon/Deactivate/5".ShouldMapTo<CouponController>(a => a.Deactivate(5), true);    
        }

        #endregion Route Tests

        #region Index Tests

        /// <summary>
        /// Tests the index returns view.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }

        #endregion Index Tests

        #region Create Tests

        /// <summary>
        /// Tests the create with one parameter redirected to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestCreateWithOneParameterRedirectedToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableByID(5)).Return(null).Repeat.Any();
            Controller.Create(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the create with one parameter returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestCreateWithOneParameterReturnsViewWhenIdFound()
        {
            FakeItems(3);
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            var result = Controller.Create(2)
                .AssertViewRendered()
                .WithViewData<CouponViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(Items[1], result.Item);
        }

        /// <summary>
        /// Tests the create with two parameters redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestCreateWithTwoParametersRedirectsToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            Controller.Create(2, new Coupon())
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the create with two parameters and valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithTwoParametersAndValidDataSaves()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeItems(3);
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            var newCoupon = CreateValidEntities.Coupon(1);
            newCoupon.Code = null;
            newCoupon.UserId = null;

            var result = Controller.Create(2, newCoupon)
                .AssertHttpRedirect();
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/2#Coupons", result.Url);
            CouponRepository.AssertWasCalled(a => a.EnsurePersistent(newCoupon));
            Assert.AreEqual("Coupon has been created successfully.", Controller.Message);
            Assert.IsNotNull(newCoupon.Code);
            Assert.IsNotNull(newCoupon.UserId);
        }

        /// <summary>
        /// Creates the with two parameters and invalid data does not save.
        /// </summary>
        [TestMethod]
        public void CreateWithTwoParametersAndInvalidDataDoesNotSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeItems(3);
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            var newCoupon = CreateValidEntities.Coupon(1);
            newCoupon.Code = null;
            newCoupon.UserId = null;
            newCoupon.DiscountAmount = 0;

            var result = Controller.Create(2, newCoupon)
                .AssertViewRendered()
                .WithViewData<CouponViewModel>();
            CouponRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Coupon>.Is.Anything));
            Assert.AreNotEqual("Coupon has been created successfully.", Controller.Message);
            Assert.AreSame(newCoupon, result.Coupon);
            Controller.ModelState.AssertErrorsAre("DiscountAmount: must be more than 0.00");
        }

        #endregion Create Tests

        #region Validate Tests

        /// <summary>
        /// Tests the validate when item id not found returns json result with error message.
        /// </summary>
        [TestMethod]
        public void TestValidateWhenItemIdNotFoundReturnsJsonResultWithErrorMessage()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODES";
            }
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODES")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid code.", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate when coupon code not found returns json result with error message.
        /// </summary>
        [TestMethod]
        public void TestValidateWhenCouponCodeNotFoundReturnsJsonResultWithErrorMessage()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODES";
            }
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "NOTFOUND")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid code.", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate when item and coupon code do not match returns json result with error message.
        /// </summary>
        [TestMethod]
        public void TestValidateWhenItemAndCouponCodeDoNotMatchReturnsJsonResultWithErrorMessage()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODE"+ (i+1);
            }
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODE1")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid code.", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate when item and coupon code do match but coupon is not active returns json result with error message.
        /// </summary>
        [TestMethod]
        public void TestValidateWhenItemAndCouponCodeDoMatchButCouponIsNotActiveReturnsJsonResultWithErrorMessage()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODE" + (i + 1);                
            }
            Coupons[1].IsActive = false;
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODE2")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid code.", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate when item and coupon code do match but coupon is used returns json result with error message.
        /// </summary>
        [TestMethod]
        public void TestValidateWhenItemAndCouponCodeDoMatchButCouponIsUsedReturnsJsonResultWithErrorMessage()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODE" + (i + 1);
            }
            Coupons[1].IsActive = true;
            Coupons[1].Used = true;
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODE2")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Coupon has already been redeemed.", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate with valid data returns coupon discount amount.
        /// </summary>
        [TestMethod]
        public void TestValidateWithValidDataReturnsCouponDiscountAmount()
        {
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODE" + (i + 1);
            }
            Coupons[1].DiscountAmount = 10.77m;
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODE2")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("10.77", result.Data.ToString());
        }

        /// <summary>
        /// Tests the validate with unlimited coupon that has been used returns coupon discount amount.
        /// </summary>
        [TestMethod]
        public void TestValidateWithUnlimitedCouponThatHasBeenUsedReturnsCouponDiscountAmount()
        {
            //Fix in controller. See suggested commented out code
            FakeCoupons(3);
            FakeItems(3);
            for (int i = 0; i < 3; i++)
            {
                Coupons[i].Item = Items[i];
                Coupons[i].Code = "FAKECCODE" + (i + 1);
            }
            Coupons[1].DiscountAmount = 10.77m;
            Coupons[1].Used = true;
            Coupons[1].Unlimited = true;
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();

            var result = Controller.Validate(2, "FAKECCODE2")
                .AssertResultIs<JsonNetResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("10.77", result.Data.ToString(), "Controller should check unlimited coupons.");
        }

        #endregion Validate Tests

        #region Deactivate Tests

        /// <summary>
        /// Tests the deactivate when coupon id not found does not save.
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenCouponIdNotFoundDoesNotSave()
        {
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.Deactivate(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            CouponRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Coupon>.Is.Anything));
        }

        /// <summary>
        /// Tests the deactivate when is not active does not save.
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenIsNotActiveDoesNotSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/1").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeCoupons(1);
            Coupons[0].IsActive = false;
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(Coupons[0]).Repeat.Any();
            var result = Controller.Deactivate(1)
                .AssertHttpRedirect();
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/1#Coupons", result.Url);
            CouponRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Coupon>.Is.Anything));
        }

        /// <summary>
        /// Tests the deactivate when is used does not save.
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenIsUsedDoesNotSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/1").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeCoupons(1);
            Coupons[0].Used = true;
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(Coupons[0]).Repeat.Any();
            var result = Controller.Deactivate(1)
                .AssertHttpRedirect();
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/1#Coupons", result.Url);
            CouponRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Coupon>.Is.Anything));
        }

        /// <summary>
        /// Tests the deactivate when is used and unlimited does save.
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenIsUsedAndUnlimitedDoesSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/1").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeCoupons(1);
            Coupons[0].Used = true;
            Coupons[0].Unlimited = true;
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(Coupons[0]).Repeat.Any();
            var result = Controller.Deactivate(1)
                .AssertHttpRedirect();
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/1#Coupons", result.Url);
            Assert.AreEqual("Coupon has been deactivated.", Controller.Message, "Controller should check unlimited.");
            CouponRepository.AssertWasCalled(a => a.EnsurePersistent(Coupons[0]));
            Assert.IsFalse(Coupons[0].IsActive);            
        }

        /// <summary>
        /// Tests the deactivate when valid data does save.
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenValidDataDoesSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/1").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeCoupons(1);            
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(Coupons[0]).Repeat.Any();
            var result = Controller.Deactivate(1)
                .AssertHttpRedirect();
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/1#Coupons", result.Url);
            Assert.AreEqual("Coupon has been deactivated.", Controller.Message);
            CouponRepository.AssertWasCalled(a => a.EnsurePersistent(Coupons[0]));
            Assert.IsFalse(Coupons[0].IsActive);
        }

        /// <summary>
        /// Tests the deactivate when invalid data does not save.
        /// This probably can't happen
        /// </summary>
        [TestMethod]
        public void TestDeactivateWhenInvalidDataDoesNotSave()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/1").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            FakeCoupons(1);
            Coupons[0].Code = " "; //Invalid.
            CouponRepository.Expect(a => a.GetNullableByID(1)).Return(Coupons[0]).Repeat.Any();
            var result = Controller.Deactivate(1)
                .AssertHttpRedirect();
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Edit/1#Coupons", result.Url);
            Assert.AreEqual("Coupon was unable to update.", Controller.Message);
            CouponRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Coupon>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Code: may not be null or empty");
        }

        #endregion Deactivate Tests

        #region Helper Methods

        /// <summary>
        /// Fakes the items.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeItems(int count)
        {
            var offSet = Items.Count;
            for (int i = 0; i < count; i++)
            {
                Items.Add(CreateValidEntities.Item(i + 1 + offSet));
                Items[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the coupons.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeCoupons(int count)
        {
            var offSet = Coupons.Count;
            for (int i = 0; i < count; i++)
            {
                Coupons.Add(CreateValidEntities.Coupon(i + 1 + offSet));
                Coupons[i + offSet].SetIdTo(i + 1 + offSet);
                Coupons[i + offSet].IsActive = true;
                Coupons[i + offSet].Used = false;
            }
        }

        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }
        }
        #endregion

        #endregion Helper Methods
    }
}
