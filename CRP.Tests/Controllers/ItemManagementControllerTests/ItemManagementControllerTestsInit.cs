using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    /// <summary>
    /// ItemManagement Controller Tests
    /// </summary>
    [TestClass]
    public partial class ItemManagementControllerTests : ControllerTestBase<ItemManagementController>
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
        protected List<ExtendedProperty> ExtendedProperties { get; set; }
        protected IRepository<ExtendedProperty> ExtendedPropertyRepository { get; set; }
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<ItemTypeQuestionSet> ItemTypeQuestionSets { get; set; }
        protected IRepository<ItemTypeQuestionSet> ItemTypeQuestionSetRepository { get; set; }
        protected List<ItemReport> ItemReports { get; set; }
        protected IRepository<ItemReport> ItemReportRepository { get; set; }
        protected Type _controllerClass = typeof(ItemManagementController);

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

            ExtendedProperties = new List<ExtendedProperty>();
            ExtendedPropertyRepository = FakeRepository<ExtendedProperty>();
            Controller.Repository.Expect(a => a.OfType<ExtendedProperty>()).Return(ExtendedPropertyRepository).Repeat.Any();

            Tags = new List<Tag>();
            TagRepository = FakeRepository<Tag>();
            Controller.Repository.Expect(a => a.OfType<Tag>()).Return(TagRepository).Repeat.Any();

            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            ItemTypeQuestionSets = new List<ItemTypeQuestionSet>();
            ItemTypeQuestionSetRepository = FakeRepository<ItemTypeQuestionSet>();
            Controller.Repository.Expect(a => a.OfType<ItemTypeQuestionSet>()).Return(ItemTypeQuestionSetRepository).Repeat.Any();

            ItemReports = new List<ItemReport>();
            ItemReportRepository = FakeRepository<ItemReport>();
            Controller.Repository.Expect(a => a.OfType<ItemReport>()).Return(ItemReportRepository).Repeat.Any();
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

        #region Helper Methods

        /// <summary>
        /// Setups the data for create tests.
        /// </summary>
        private void SetupDataForCreateTests()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);
            //Fakes
            FakeExtendedProperties(4);
            FakeTags(2);
            FakeItems(1);
            FakeUsers(2);
            FakeQuestionSets(1);
            FakeUnits(2);
            FakeItemTypes(3);
            FakeItemTypeQuestionSets(4);

            TagRepository.Expect(a => a.GetAll()).Return(Tags.ToList()).Repeat.Any();
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionSetRepository.Expect(a => a.Queryable).Return(QuestionSets.AsQueryable()).Repeat.Any();

            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;

            Users[1].LoginID = "UserName";
            Users[1].Units.Add(Units[0]);
            Users[1].Units.Add(Units[1]);

            ItemTypeQuestionSets[0].TransactionLevel = true;
            ItemTypeQuestionSets[0].QuantityLevel = false;
            ItemTypeQuestionSets[1].TransactionLevel = false;
            ItemTypeQuestionSets[1].QuantityLevel = true;

            ItemTypes[0].QuestionSets.Add(ItemTypeQuestionSets[0]);
            ItemTypes[0].QuestionSets.Add(ItemTypeQuestionSets[1]);
            ItemTypes[2].QuestionSets.Add(ItemTypeQuestionSets[3]);

            Items[0].ItemType = ItemTypes[0];
        }


        #region Fakes

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

        private void FakeItemReports(int count)
        {
            var offSet = ItemReports.Count;
            for (int i = 0; i < count; i++)
            {
                ItemReports.Add(CreateValidEntities.ItemReport(i + 1 + offSet));
                ItemReports[i + offSet].SetIdTo(i + 1 + offSet);
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

        /// <summary>
        /// Fakes the ExtendedProperties.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of ExtendedProperties to add.</param>
        private void FakeExtendedProperties(int count)
        {
            var offSet = ExtendedProperties.Count;
            for (int i = 0; i < count; i++)
            {
                ExtendedProperties.Add(CreateValidEntities.ExtendedProperty(i + 1 + offSet));
                ExtendedProperties[i + offSet].SetIdTo(i + 1 + offSet);
                var i1 = i;
                ExtendedPropertyRepository.Expect(a => a.GetNullableByID(i1 + 1 + offSet))
                    .Return(ExtendedProperties[i1 + offSet]).Repeat.Any();
            }
        }

        /// <summary>
        /// Fakes the Tags.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of Tags to add.</param>
        private void FakeTags(int count)
        {
            var offSet = Tags.Count;
            for (int i = 0; i < count; i++)
            {
                Tags.Add(CreateValidEntities.Tag(i + 1 + offSet));
                Tags[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the QuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of QuestionSets to add.</param>
        private void FakeQuestionSets(int count)
        {
            var offSet = QuestionSets.Count;
            for (int i = 0; i < count; i++)
            {
                QuestionSets.Add(CreateValidEntities.QuestionSet(i + 1 + offSet));
                QuestionSets[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the Units.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of Units to add.</param>
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
        /// Fakes the ItemTypeQuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of ItemTypeQuestionSets to add.</param>
        private void FakeItemTypeQuestionSets(int count)
        {
            var offSet = ItemTypeQuestionSets.Count;
            for (int i = 0; i < count; i++)
            {
                ItemTypeQuestionSets.Add(CreateValidEntities.ItemTypeQuestionSet(i + 1 + offSet));
                ItemTypeQuestionSets[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #endregion Fakes

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
            private int _count;
            public MockHttpContext(int count)
            {
                _count = count;
            }

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

            public override HttpRequestBase Request
            {
                get
                {
                    return new MockHttpRequest(_count);
                }
            }
        }

        public class MockHttpRequest : HttpRequestBase
        {
            MockHttpFileCollectionBase Mocked { get; set; }

            public MockHttpRequest(int count)
            {
                Mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return Mocked;
                }
            }
        }

        public class MockHttpFileCollectionBase : HttpFileCollectionBase
        {
            public int Counter { get; set; }

            public MockHttpFileCollectionBase(int count)
            {
                Counter = count;
                for (int i = 0; i < count; i++)
                {
                    BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
                }

            }

            public override int Count
            {
                get
                {
                    return Counter;
                }
            }
            public override HttpPostedFileBase Get(string name)
            {
                return new MockHttpPostedFileBase();
            }
            public override HttpPostedFileBase this[string name]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
            public override HttpPostedFileBase this[int index]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase
        {
            public override int ContentLength
            {
                get
                {
                    return 5;
                }
            }
            public override string FileName
            {
                get
                {
                    return "Mocked File Name";
                }
            }
            public override Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                    return memStream;
                }
            }
        }

        #endregion

        #endregion Helper Methods
    }
}