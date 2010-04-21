using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
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
    /// ItemManagement Controller Tests
    /// </summary>
    [TestClass]
    public class ItemManagementControllerTests : ControllerTestBase<ItemManagementController>
    {
        protected readonly IPrincipal Principal = new MockPrincipal();
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }
        protected List<ItemType> ItemTypes { get; set; }
        protected IRepository<ItemType> ItemTypeRepository { get; set; }

        #region Init
        public ItemManagementControllerTests()
        {
            Controller.ControllerContext.HttpContext.User = Principal;

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Editors = new List<Editor>();
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            ItemTypes = new List<ItemType>();
            ItemTypeRepository = FakeRepository<ItemType>();
            Controller.Repository.Expect(a => a.OfType<ItemType>()).Return(ItemTypeRepository).Repeat.Any();
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
            Controller = new TestControllerBuilder().CreateController<ItemManagementController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ItemManagement/Index".ShouldMapTo<ItemManagementController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/ItemManagement/List".ShouldMapTo<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create(new Item(),new ExtendedPropertyParameter[1],new string[1], string.Empty), true);
        }

        /// <summary>
        /// Tests the get extended properties mapping.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesMapping()
        {
            "~/ItemManagement/GetExtendedProperties/5".ShouldMapTo<ItemManagementController>(a => a.GetExtendedProperties(5));
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5, new Item(),new ExtendedPropertyParameter[1],new string[1], string.Empty), true);
        }

        /// <summary>
        /// Tests the remove editor with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestRemoveEditorWithParametersMapping()
        {
            "~/ItemManagement/RemoveEditor/5".ShouldMapTo<ItemManagementController>(a => a.RemoveEditor(5, 1), true);
        }

        /// <summary>
        /// Tests the add editor mapping.
        /// </summary>
        [TestMethod]
        public void TestAddEditorMapping()
        {
            "~/ItemManagement/AddEditor/5".ShouldMapTo<ItemManagementController>(a => a.AddEditor(5, null));
        }

        /// <summary>
        /// Tests the save template with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestSaveTemplateWithParametersMapping()
        {
            "~/ItemManagement/SaveTemplate/5".ShouldMapTo<ItemManagementController>(a => a.SaveTemplate(5, "Test"), true);
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/ItemManagement/Details/5".ShouldMapTo<ItemManagementController>(a => a.Details(5));
        }
        #endregion Route Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }
        

        #endregion Index Tests

        #region List Tests

        [TestMethod]
        public void TestListReturnsView()
        {
            FakeItems(2);
            FakeUsers(2);
            FakeEditors(1);
            Users[1].LoginID = "UserName";
            Editors[0].User = Users[1];
            Items[1].AddEditor(Editors[0]);

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();

            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.FirstOrDefault().Name);
        }
        

        #endregion List Tests

        #region Create Tests

        [TestMethod]
        public void TestCreateWithoutParameterReturnsItemViewModel()
        {
            FakeUsers(2);
            FakeItemTypes(1);
            Users[1].LoginID = "UserName";
            ItemTypes[0].IsActive = true;
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();

            Controller.Create()
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
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

        private void FakeUsers(int count)
        {
            var offSet = Users.Count;
            for (int i = 0; i < count; i++)
            {
                Users.Add(CreateValidEntities.User(i + 1 + offSet));
                Users[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        private void FakeEditors(int count)
        {
            var offSet = Editors.Count;
            for (int i = 0; i < count; i++)
            {
                Editors.Add(CreateValidEntities.Editor(i + 1 + offSet));
                Editors[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item types.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeItemTypes(int count)
        {
            var offSet = ItemTypes.Count;
            for (int i = 0; i < count; i++)
            {
                ItemTypes.Add(CreateValidEntities.ItemType(i + 1 + offSet));
                ItemTypes[i + offSet].SetIdTo(i + 1 + offSet);
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
