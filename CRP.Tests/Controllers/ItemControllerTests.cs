using System.Collections.Generic;
using System.Linq;
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
    
    /// <summary>
    /// Item Controller Tests
    /// </summary>
    [TestClass]
    public class ItemControllerTests : ControllerTestBase<ItemController>
    {
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }
        protected List<DisplayProfile> DisplayProfiles { get; set; }
        protected IRepository<DisplayProfile> DisplayProfileRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected List<School> Schools { get; set; }

        protected IPrincipal Principal = new MockPrincipal();

        #region Init

        public ItemControllerTests()
        {
            Controller.ControllerContext.HttpContext.User = Principal;

            Tags = new List<Tag>();
            TagRepository = FakeRepository<Tag>();
            Controller.Repository.Expect(a => a.OfType<Tag>()).Return(TagRepository).Repeat.Any();

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            DisplayProfiles = new List<DisplayProfile>();
            DisplayProfileRepository = FakeRepository<DisplayProfile>();
            Controller.Repository.Expect(a => a.OfType<DisplayProfile>()).Return(DisplayProfileRepository).Repeat.Any();

            Units = new List<Unit>();

            Schools = new List<School>();
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
            OpenIdUserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OpenIdUser, string>>();
            Controller = new TestControllerBuilder().CreateController<ItemController>(OpenIdUserRepository);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Item/Index".ShouldMapTo<ItemController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/Item/List".ShouldMapTo<ItemController>(a => a.List());
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Item/Details/5".ShouldMapTo<ItemController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the get image mapping.
        /// </summary>
        [TestMethod]
        public void TestGetImageMapping()
        {
            "~/Item/GetImage/5".ShouldMapTo<ItemController>(a => a.GetImage(5));
        }
        #endregion Route Tests

        #region Misc Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }

        [TestMethod]
        public void TestListReturnsBrowseItemsViewModel()
        {
            FakeItems(1);
            FakeTags(1);
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();
            TagRepository.Expect(a => a.Queryable).Return(Tags.AsQueryable()).Repeat.Any();
            Controller.List()
                .AssertViewRendered()
                .WithViewData<BrowseItemsViewModel>();
        }

        #endregion Misc Tests

        #region Details Tests

        [TestMethod]
        public void TestDetailsWhenIdNotFoundRedirectsToList()
        {
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            Controller.Details(2)
                .AssertActionRedirect()
                .ToAction<ItemController>(a => a.List());
        }

        [TestMethod]
        public void TestDetailsWhenIdFoundReturnsView()
        {
            FakeDisplayProfiles(4);
            FakeUnits(3);
            FakeItems(2);
            FakeSchools(2);
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            Items[1].Available = true;
            Items[1].Unit = Units[1];
            Items[0].Unit = Units[2];
            DisplayProfiles[2].Unit = Units[1];
            DisplayProfiles[3].School = Schools[1];
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            
            
            var result = Controller.Details(2)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(Items[1], result.Item);
            Assert.AreSame(DisplayProfiles[2], result.DisplayProfile);
            Assert.AreNotSame(DisplayProfiles[3], result.DisplayProfile);
        }
        [TestMethod]
        public void TestDetailsWhenIdFoundReturnsView2()
        {
            FakeDisplayProfiles(4);
            FakeUnits(3);
            FakeItems(2);
            FakeSchools(2);
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            Items[1].Available = true;
            Items[1].Unit = Units[1];
            Items[0].Unit = Units[2];
            DisplayProfiles[2].Unit = Units[2]; //Different From Above
            DisplayProfiles[3].School = Schools[1];
            DisplayProfiles[3].SchoolMaster = true;
            Units[1].School = Schools[1];
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();

            var result = Controller.Details(2)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(Items[1], result.Item);
            Assert.AreNotSame(DisplayProfiles[2], result.DisplayProfile);
            Assert.AreSame(DisplayProfiles[3], result.DisplayProfile);
        }

        #endregion Details Tests

        #region GetImage Tests


        [TestMethod]
        public void TestGetImageWhenIdNotFoundDoesNotThrowException()
        {
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            var result = Controller.GetImage(2)
                .AssertResultIs<FileContentResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.FileContents.ByteArrayToString());
            Assert.AreEqual("image/jpg", result.ContentType);
        }

        [TestMethod]
        public void TestGetImageWhenIdFoundButImageIsNullReturnsEmptyFileContents()
        {
            FakeItems(1);
            Items[0].Image = null;
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            var result = Controller.GetImage(1)
                .AssertResultIs<FileContentResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.FileContents.ByteArrayToString());
            Assert.AreEqual("image/jpg", result.ContentType);
        }

        [TestMethod]
        public void TestGetImageWhenIdFoundAndImageIsNotNullReturnsFileContents()
        {
            FakeItems(1);
            Items[0].Image = new byte[]{4,5,6,7};
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            var result = Controller.GetImage(1)
                .AssertResultIs<FileContentResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("4567", result.FileContents.ByteArrayToString());
            Assert.AreEqual("image/jpg", result.ContentType);
        }
        #endregion GetImage Tests

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

        private void FakeDisplayProfiles(int count)
        {
            var offSet = DisplayProfiles.Count;
            for (int i = 0; i < count; i++)
            {
                DisplayProfiles.Add(CreateValidEntities.DisplayProfile(i + 1 + offSet));
                DisplayProfiles[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        private void FakeUnits(int count)
        {
            var offSet = Units.Count;
            for (int i = 0; i < count; i++)
            {
                Units.Add(CreateValidEntities.Unit(i + 1 + offSet));
                Units[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the schools.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeSchools(int count)
        {
            var offSet = Schools.Count;
            for (int i = 0; i < count; i++)
            {
                Schools.Add(CreateValidEntities.School(i + 1 + offSet));
                Schools[i + offSet].SetIdTo((i + 1 + offSet).ToString());
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
                    return "httpUserName";
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
        #endregion mocks

        #endregion Helper Methods
    }
}
