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


namespace CRP.Tests.Controllers
{
    [TestClass]
    public class CouponControllerRepositoryTests : ControllerTestBase<CouponController>
    {
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected readonly IPrincipal Principal = new MockPrincipal();
        protected IRepository<Coupon> CouponRepository { get; set; }

        #region Init

        public CouponControllerRepositoryTests()
        {
            Controller.ControllerContext.HttpContext.User = Principal;
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();
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
