using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Tag Controller Tests
    /// </summary>
    [TestClass]
    public class TagControllerTests : ControllerTestBase<TagController>
    {
        private readonly Type _controllerClass = typeof(TagController);
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }    

        #region Init
        public TagControllerTests()
        {
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Tags = new List<Tag>();
            TagRepository = FakeRepository<Tag>();
            Controller.Repository.Expect(a => a.OfType<Tag>()).Return(TagRepository).Repeat.Any();
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
            Controller = new TestControllerBuilder().CreateController<TagController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping ignore parameter.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingIgnoreParameter()
        {
            "~/Tag/Index/Test".ShouldMapTo<TagController>(a => a.Index("Test"), true);
        }

        /// <summary>
        /// Tests the just tag mapping.
        /// </summary>
        [TestMethod]
        public void TestJustTagMapping()
        {
            "~/Tag/Snail".ShouldMapTo<TagController>(a => a.Index("Snail"));	
        }

        #endregion Route Tests


        /// <summary>
        /// Tests the tag index returns items with tags found.
        /// </summary>
        [TestMethod]
        public void TestTagIndexReturnsItemsWithTagsFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(Items, 6);
            ControllerRecordFakes.FakeTags(Tags, 2);
            foreach (var item in Items)
            {
                item.Expiration = DateTime.Now.AddDays(2);
                item.Available = true;
                item.Private = false;
            }
            Items[0].AddTag(Tags[0]);
            Items[1].AddTag(Tags[1]);
            Items[2].AddTag(Tags[0]);
            Items[2].AddTag(Tags[1]);
            Items[3].AddTag(Tags[0]);
            Items[3].Expiration = null;
            Items[4].AddTag(Tags[0]);
            Items[4].Available = false;
            Items[5].Private = true;
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();
            TagRepository.Expect(a => a.Queryable).Return(Tags.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Index(Tags[0].Name)
                .AssertViewRendered()
                .WithViewData<BrowseItemsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Items.Count());
            Assert.IsTrue(result.Items.Contains(Items[0]));
            Assert.IsFalse(result.Items.Contains(Items[1]));
            Assert.IsTrue(result.Items.Contains(Items[2]));
            #endregion Assert		
        }


        /// <summary>
        /// Tests the tag index returns message if no matching tags are found add all current items.
        /// </summary>
        [TestMethod]
        public void TestTagIndexReturnsMessageIfNoMatchingTagsAreFoundAddAllCurrentItems()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(Items, 6);
            ControllerRecordFakes.FakeTags(Tags, 2);
            foreach (var item in Items)
            {
                item.Expiration = DateTime.Now.AddDays(2);
                item.Available = true;
                item.Private = false;
            }
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();
            TagRepository.Expect(a => a.Queryable).Return(Tags.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Index("NotATag")
                .AssertViewRendered()
                .WithViewData<BrowseItemsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Items.Count());
            Assert.AreEqual("No items match that tag.", Controller.Message);
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
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has 3 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasThreeAttributes()
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

        [TestMethod]
        public void TestControllerHasLocServiceMessageAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<LocServiceMessageAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "LocServiceMessageAttribute not found.");
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
            Assert.AreEqual(1, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method viewReport  contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodViewReportContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
