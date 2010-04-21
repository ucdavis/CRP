using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    /// <summary>
    /// <c>QuestionSet</c> Controller Tests
    /// </summary>
    [TestClass]
    public partial class QuestionSetControllerTests : ControllerTestBase<QuestionSetController>
    {
        protected readonly Type ControllerClass = typeof(QuestionSetController);
        protected readonly IPrincipal Principal = new MockPrincipal(new[] { RoleNames.User });
        protected IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }        
        protected List<School> Schools { get; set; }

        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<ItemType> ItemTypes { get; set; }
        protected IRepository<ItemType> ItemTypeRepository { get; set; }
        protected IRepository<ItemQuestionSet> ItemQuestionSetRepository { get; set; }
        protected List<TransactionAnswer> TransactionAnswers { get; set; }
        protected IRepository<TransactionAnswer> TransactionAnswerRepository { get; set; }
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }

   

        
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionSetControllerTests"/> class.
        /// </summary>
        public QuestionSetControllerTests()
        {
            QuestionTypes = new List<QuestionType>();
            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();
        
            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();
      
            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            ItemTypes = new List<ItemType>();
            ItemTypeRepository = FakeRepository<ItemType>();
            Controller.Repository.Expect(a => a.OfType<ItemType>()).Return(ItemTypeRepository).Repeat.Any();

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            TransactionAnswers = new List<TransactionAnswer>();
            TransactionAnswerRepository = FakeRepository<TransactionAnswer>();
            Controller.Repository.Expect(a => a.OfType<TransactionAnswer>()).Return(TransactionAnswerRepository).Repeat.Any();
            
            Editors = new List<Editor>();
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();
        
            ItemQuestionSetRepository = FakeRepository<ItemQuestionSet>();
            Controller.Repository.Expect(a => a.OfType<ItemQuestionSet>()).Return(ItemQuestionSetRepository).Repeat.Any();

            Schools = new List<School>();
        }
        //Controller.ControllerContext.HttpContext = new MockHttpContext(1, new [] {RoleNames.Admin});
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
            SchoolRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<School, string>>();
            Controller = new TestControllerBuilder().CreateController<QuestionSetController>(SchoolRepository);
        }

        #endregion Init

        #region Helper Methods
        /// <summary>
        /// Setups the data for unlink from tests.
        /// </summary>
        private void SetupDataForUnlinkFromTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeTransactionAnswers(TransactionAnswers, 3);
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 5);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 2);
            Editors[0].User = Users[0];
            Editors[1].User = Users[1]; //Current user, but don't add to the item here.
            Items[1].AddEditor(Editors[0]);

            Users[1].LoginID = "UserName";
            Items[1].AddTransactionQuestionSet(QuestionSets[0]);
            Items[1].AddTransactionQuestionSet(QuestionSets[1]);
            Items[1].AddQuantityQuestionSet(QuestionSets[1]);
            Items[1].AddQuantityQuestionSet(QuestionSets[3]);

            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(TransactionAnswers.AsQueryable()).Repeat.Any();
        }
        /// <summary>
        /// Setups the data for link to tests.
        /// </summary>
        private void SetupDataForLinkToTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeItemTypes(ItemTypes, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeUnits(Units, 5);
            ControllerRecordFakes.FakeSchools(Schools, 5);
            for (int i = 0; i < 5; i++)
            {
                Units[i].School = Schools[i];
            }

            Users[1].Units.Add(Units[1]);
            Users[1].Units.Add(Units[3]);
            Users[1].Units.Add(Units[4]);
            Users[1].LoginID = "UserName";

            ControllerRecordFakes.FakeEditors(Editors, 2);
            Editors[0].User = Users[0];
            Editors[1].User = Users[1]; //Current user, but don't add to the item here.
            Items[1].AddEditor(Editors[0]);

            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 10);
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[1]; //Has
            QuestionSets[1].UserReusable = true;
            QuestionSets[1].User = Users[0]; //Doesn't have
            QuestionSets[2].UserReusable = true;
            QuestionSets[2].User = Users[1]; //Has
            QuestionSets[3].CollegeReusable = true;
            QuestionSets[3].School = Schools[1]; //Has
            QuestionSets[4].CollegeReusable = true;
            QuestionSets[4].School = Schools[2]; //Doesn't have
            QuestionSets[5].CollegeReusable = true;
            QuestionSets[5].School = Schools[4]; //Has
            QuestionSets[6].SystemReusable = true; //Has
            QuestionSets[7].SystemReusable = true; //System so shouldn't appear
            QuestionSets[7].Name = StaticValues.QuestionSet_ContactInformation;

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();
            QuestionSetRepository.Expect(a => a.Queryable).Return(QuestionSets.AsQueryable()).Repeat.Any();
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(QuestionSets[1]).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
        }
        /// <summary>
        /// Setups the data for create tests.
        /// </summary>
        private void SetupDataForCreateTests()
        {
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeUnits(Units, 5);
            ControllerRecordFakes.FakeSchools(Schools, 5);
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeItemTypes(ItemTypes, 3);

            for (int i = 0; i < 5; i++)
            {
                Units[i].School = Schools[i];
            }

            Users[1].Units.Add(Units[1]);
            Users[1].Units.Add(Units[3]);
            Users[1].Units.Add(Units[4]);
            Users[1].LoginID = "UserName";
            
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.GetAll()).Return(QuestionTypes).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();
            ItemRepository.Expect(a => a.GetByID(2)).Return(Items[1]).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetByID(2)).Return(ItemTypes[1]).Repeat.Any();
        }

        /// <summary>
        /// Sets up data for edit tests.
        /// </summary>
        private void SetupDataForTests()
        {
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeUnits(Units, 5);
            ControllerRecordFakes.FakeSchools(Schools, 5);
            var editor = CreateValidEntities.Editor(1);
            editor.User = Users[0];
            var itemQuestionSet = CreateValidEntities.ItemQuestionSet(1);
            itemQuestionSet.Item = CreateValidEntities.Item(1);
            itemQuestionSet.Item.AddEditor(editor);

            for (int i = 0; i < 5; i++)
            {
                Units[i].School = Schools[i];
            }

            Users[1].Units.Add(Units[1]);
            Users[1].Units.Add(Units[3]);
            Users[1].Units.Add(Units[4]);
            Users[1].LoginID = "UserName";

            var editor2 = CreateValidEntities.Editor(2);
            editor2.User = Users[1];
            var itemQuestionSet2 = CreateValidEntities.ItemQuestionSet(2);
            itemQuestionSet2.Item = CreateValidEntities.Item(2);
            itemQuestionSet2.Item.AddEditor(editor2);

            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 10);
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[1];
            QuestionSets[1].UserReusable = true;
            QuestionSets[2].UserReusable = true;
            QuestionSets[2].User = Users[1];
            QuestionSets[3].CollegeReusable = true;
            QuestionSets[3].School = Schools[1]; //Has
            QuestionSets[4].CollegeReusable = true;
            QuestionSets[4].School = Schools[2]; //Doesn't have
            QuestionSets[5].CollegeReusable = true;
            QuestionSets[5].School = Schools[4]; //Has
            QuestionSets[6].SystemReusable = true;
            QuestionSets[7].User = Users[0]; //Not the owner
            QuestionSets[7].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSets[8].User = Users[0]; //Not the owner
            QuestionSets[8].ItemTypes = new List<ItemTypeQuestionSet>();
            QuestionSets[8].Items.Add(itemQuestionSet);
            QuestionSets[9].User = Users[0]; //Not the owner
            QuestionSets[9].ItemTypes = new List<ItemTypeQuestionSet>();
            QuestionSets[9].Items.Add(itemQuestionSet2); //Is an editor

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionSetRepository.Expect(a => a.Queryable).Return(QuestionSets.AsQueryable()).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();
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
            public bool RoleReturnValue { get; set; }
            public string[] UserRoles { get; set; }

            public MockPrincipal(string[] userRoles)
            {
                UserRoles = userRoles;
            }

            public IIdentity Identity
            {
                get { return _identity ?? (_identity = new MockIdentity()); }
            }

            public bool IsInRole(string role)
            {
                if (UserRoles.Contains(role))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Mock the HTTPContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private readonly int _count;
            public string[] UserRoles { get; set; }
            public MockHttpContext(int count, string[] userRoles)
            {
                _count = count;
                UserRoles = userRoles;
            }

            public override IPrincipal User
            {
                get { return _user ?? (_user = new MockPrincipal(UserRoles)); }
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
