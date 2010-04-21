using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : ControllerTestBase<HomeController>
    {
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }
        private readonly Type _controllerClass = typeof(HomeController);

        #region Init
        public HomeControllerTest()
        {
            Tags = new List<Tag>();
            TagRepository = FakeRepository<Tag>();
            Controller.Repository.Expect(a => a.OfType<Tag>()).Return(TagRepository).Repeat.Any();

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();
        }

        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<HomeController>();
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }
        #endregion Init

        #region Mapping Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Home/Index".ShouldMapTo<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Tests the admin home mapping.
        /// </summary>
        [TestMethod]
        public void TestAdminHomeMapping()
        {
            "~/Home/AdminHome".ShouldMapTo<HomeController>(a => a.AdminHome());
        }

        /// <summary>
        /// Tests the about mapping.
        /// </summary>
        [TestMethod]
        public void TestAboutMapping()
        {
            "~/Home/About".ShouldMapTo<HomeController>(a => a.About());
        }
        #endregion Mapping Tests

        /// <summary>
        /// Tests the index returns view of items.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsViewOfItems()
        {
            #region Arrange
            FakeItems(3);
            foreach (var item in Items)
            {
                item.Expiration = DateTime.Now.AddDays(3);
            }
            FakeTags(1);
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();
            TagRepository.Expect(a => a.Queryable).Return(Tags.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<BrowseItemsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Items.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the about returns view.
        /// </summary>
        [TestMethod]
        public void TestAboutReturnsView()
        {
            #region Act
            var result = Controller.About()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the admin home returns view.
        /// </summary>
        [TestMethod]
        public void TestAdminHomeReturnsView()
        {
            #region Act
            var result = Controller.AdminHome()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert
        }

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from super controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromSuperController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has handle transactions manually attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasHandleTransactionsManuallyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "HandleTransactionsManuallyAttribute not found.");
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        /// <summary>
        /// Tests the controller contains expected number of public methods.
        /// </summary>
        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method Index contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method admin home contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAdminHomeContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("AdminHome");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AnyoneWithRoleAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method about contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAboutContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("About");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests

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
        /// Fakes the tags.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeTags(int count)
        {
            var offSet = Tags.Count;
            for (int i = 0; i < count; i++)
            {
                Tags.Add(CreateValidEntities.Tag(i + 1 + offSet));
                Tags[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        #endregion Helper Methods
    }
}
