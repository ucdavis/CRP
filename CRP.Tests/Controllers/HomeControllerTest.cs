using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : ControllerTestBase<HomeController>
    {
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }

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
        /// Tests the index.
        /// </summary>
        [TestMethod]
        public void TestIndex()
        {
            FakeItems(1);
            FakeTags(1);
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();
            TagRepository.Expect(a => a.Queryable).Return(Tags.AsQueryable()).Repeat.Any();
   
            // Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<BrowseItemsViewModel>();

            // Assert
            //ViewDataDictionary viewData = result.ViewData;
            //Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
            Assert.IsNotNull(result);

        }

        /// <summary>
        /// Tests the about.
        /// </summary>
        [TestMethod]
        public void TestAbout()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

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
    }
}
