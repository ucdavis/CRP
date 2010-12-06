using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
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
        protected ISearchTermProvider SearchProvider { get; set; }
        private readonly Type _controllerClass = typeof(ItemController);

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
            SearchProvider = MockRepository.GenerateStub<ISearchTermProvider>();
            Controller = new TestControllerBuilder().CreateController<ItemController>(OpenIdUserRepository, SearchProvider);
        }

        #endregion Init

        #region Route Tests

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

        [TestMethod]
        public void TestMapMapping()
        {
            "~/Item/Map/5".ShouldMapTo<ItemController>(a => a.Map(5));
        }


        #endregion Route Tests


        #region Details Tests

        /// <summary>
        /// Tests the index of the details when id not found redirects to home.
        /// </summary>
        [TestMethod]
        public void TestDetailsWhenIdNotFoundRedirectsToHomeIndex()
        {
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            Controller.Details(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            Assert.AreEqual("Item not found.", Controller.Message);
        }

        /// <summary>
        /// Tests the index of the details when item found but not available redirects to home.
        /// </summary>
        [TestMethod]
        public void TestDetailsWhenItemFoundButNotAvailableRedirectsToHomeIndex()
        {
            #region Arrange
            FakeItems(1);
            Items[0].Available = false;
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();            
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());            
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the details when item found but has expired returns view.
        /// </summary>
        [TestMethod]
        public void TestDetailsWhenItemFoundButHasExpiredReturnsView()
        {
            #region Arrange
            FakeDisplayProfiles(4);
            FakeUnits(3);
            FakeSchools(2);
            FakeItems(1);
            Items[0].Available = true;
            Items[0].Quantity = 10;

            Items[0].Expiration = DateTime.Today.AddDays(-1); //Value being tested
            
            Items[0].Unit = Units[2];
            DisplayProfiles[2].Unit = Units[1];
            DisplayProfiles[3].School = Schools[1];
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Online registration for this event has passed.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the details when item found but has no items available returns view.
        /// </summary>
        [TestMethod]
        public void TestDetailsWhenItemFoundButHasNoItemsAvailableReturnsView()
        {
            #region Arrange
            FakeDisplayProfiles(4);
            FakeUnits(3);
            FakeSchools(2);
            FakeItems(1);
            Items[0].Available = true;

            Items[0].Quantity = 0;//Value being tested

            Items[0].Expiration = DateTime.Today.AddDays(10); 
            Items[0].Unit = Units[2];
            DisplayProfiles[2].Unit = Units[1];
            DisplayProfiles[3].School = Schools[1];
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Online registration for this event has passed.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the details when item found and is available returns view.
        /// </summary>
        [TestMethod]
        public void TestDetailsWhenItemFoundAndIsAvailableReturnsView()
        {
            #region Arrange
            FakeDisplayProfiles(4);
            FakeUnits(3);
            FakeSchools(2);
            FakeItems(1);
            Items[0].Available = true;
            Items[0].Quantity = 10;
            Items[0].Expiration = DateTime.Today.AddDays(10);
            Items[0].Unit = Units[2];
            DisplayProfiles[2].Unit = Units[1];
            DisplayProfiles[3].School = Schools[1];
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
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
            Assert.AreEqual("Online registration for this event has passed.", Controller.Message);
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
            Assert.AreEqual("Online registration for this event has passed.", Controller.Message);
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

        #region Map Tests

        [TestMethod]
        public void TestMapRedirectsToHomeIfItemNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            #endregion Arrange

            #region Act
            Controller.Map(4).AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestMapRedirectsToHomeIfItemNotAvailable()
        {
            #region Arrange
            Items = new List<Item>();
            Items.Add(CreateValidEntities.Item(1));
            Items[0].Available = false;
            ControllerRecordFakes.FakeItems(3, ItemRepository, Items);
            #endregion Arrange

            #region Act
            Controller.Map(1).AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestMapReturnsViewWhenItemAvailable()
        {
            #region Arrange
            Items = new List<Item>();
            Items.Add(CreateValidEntities.Item(1));
            Items[0].Available = true;
            ControllerRecordFakes.FakeItems(3, ItemRepository, Items);
            #endregion Arrange

            #region Act
            var result = Controller.Map(1)
                .AssertViewRendered()
                .WithViewData<Item>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.AreEqual("Name1", result.Name);
            #endregion Assert
        }
        #endregion Map Tests

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
        /// Tests the controller has only two attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyTwoAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, result.Count());
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
        /// Tests the controller method details contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method get image contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodGetImageContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("GetImage");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodMapContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Map");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
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
