using System.Collections.Generic;
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

        #region Init

        public CouponControllerRepositoryTests()
        {
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();
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

        #endregion Helper Methods
    }
}
