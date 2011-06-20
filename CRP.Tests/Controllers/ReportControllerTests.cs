using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ReportControllerTests : ControllerTestBase<ReportController>
    {
        private readonly Type _controllerClass = typeof(ReportController);
        protected IChartProvider ChartProvider;
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<ItemReport> ItemReports { get; set; }
        protected IRepository<ItemReport> ItemReportRepository { get; set; }
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected IRepository<Question> QuestionRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<Transaction> Transactions { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }


        #region Init
        public ReportControllerTests()
        {
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            ItemReports = new List<ItemReport>();
            ItemReportRepository = FakeRepository<ItemReport>();
            Controller.Repository.Expect(a => a.OfType<ItemReport>()).Return(ItemReportRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();
        
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
            Controller.Repository.Expect(a => a.OfType<Transaction>()).Return(TransactionRepository).Repeat.Any();
        
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();
            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();
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
            ChartProvider = MockRepository.GenerateStub<IChartProvider>();
            Controller = new TestControllerBuilder().CreateController<ReportController>(ChartProvider);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the view report ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestViewReportIgnoreParameters()
        {
            "~/Report/ViewReport/5".ShouldMapTo<ReportController>(a => a.ViewReport(5, 12), true);
        }


        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Report/Create/?itemId=5".ShouldMapTo<ReportController>(a => a.Create(5), true);		
        }
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Report/Create/5".ShouldMapTo<ReportController>(a => a.Create(5, "Name", new CreateReportParameter[0]), true);
        }

        [TestMethod]
        public void TestViewSystemReportMapping()
        {
            "~/Report/ViewSystemReport/5".ShouldMapTo<ReportController>(a => a.ViewSystemReport(null), true);
        }

        [TestMethod]
        public void TestGenerateChartMapping()
        {
            "~/Report/GenerateChart/5".ShouldMapTo<ReportController>(a => a.GenerateChart(5), true);
        }
        #endregion Route Tests

        #region ViewReport Tests

        [TestMethod]
        public void TestViewReportRedirectsToItemManagementListIfItemReportNotFound()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            Controller.ViewReport(1, 2)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemReport not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestViewReportRedirectsToItemManagementListIfItemFound()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            Controller.ViewReport(2, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestViewReportReturnsReportViewModel()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.ViewReport(2, 2)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);

            #endregion Assert		
        }
        #endregion ViewReport Tests

        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsToItemManagementListWhenItemNotFound()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsViewWhenItemFound()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            Controller.Create(2)
                .AssertViewRendered()
                .WithViewData<CreateReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        #endregion Create Get Tests

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostRedirectsToItemManagementListWhenItemNotFound()
        {
            #region Arrange
            SetUpDataForTests();
            #endregion Arrange

            #region Act
            Controller.Create(1, "Name", new CreateReportParameter[0])
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostWithValidData()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetUpDataForTests();
            var reportParameters = new CreateReportParameter[1];
            reportParameters[0] = new CreateReportParameter
              {
                  Property = true,
                  Quantity = false,
                  Transaction = false,
                  QuestionId = 0,
                  QuestionSetId = 0,
                  PropertyName = "Paid"
              };
            #endregion Arrange

            #region Act
            Controller.Create(2, "Name", reportParameters)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            ItemReportRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ItemReport>.Is.Anything));
            Assert.AreEqual("Report has been created successfully.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView()
        {
            #region Arrange
            //Controller.ControllerContext.HttpContext.Response
            //    .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
            //    .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            //Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetUpDataForTests();
            var reportParameters = new CreateReportParameter[1];
            reportParameters[0] = new CreateReportParameter
            {
                Property = true,
                Quantity = false,
                Transaction = false,
                QuestionId = 0,
                QuestionSetId = 0,
                PropertyName = "Paid"
            };
            #endregion Arrange

            #region Act
            Controller.Create(2, " ", reportParameters)
                .AssertViewRendered()
                .WithViewData<CreateReportViewModel>();
            #endregion Act

            #region Assert
            ItemReportRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemReport>.Is.Anything));
            Assert.AreEqual("Errors with report found.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            #endregion Assert
        }


        /// <summary>
        /// Tests the create post without any create report parameters returns view with message.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithoutAnyCreateReportParametersReturnsViewWithMessage()
        {
            #region Arrange
            SetUpDataForTests();
            var reportParameters = new CreateReportParameter[0];
            //reportParameters[0] = new CreateReportParameter
            //{
            //    Property = true,
            //    Quantity = false,
            //    Transaction = false,
            //    QuestionId = 0,
            //    QuestionSetId = 0,
            //    PropertyName = "Paid"
            //};
            #endregion Arrange

            #region Act
            Controller.Create(2, " ", reportParameters)
                .AssertViewRendered()
                .WithViewData<CreateReportViewModel>();
            #endregion Act

            #region Assert
            ItemReportRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemReport>.Is.Anything));
            Assert.AreEqual("Report Columns not Selected", Controller.Message);
            #endregion Assert		
        }

        #endregion Create Post Tests

        #region ViewSystemReports Tests

        /// <summary>
        /// Tests the view system report with id zero returns department usage report.
        /// </summary>
        [TestMethod]
        public void TestViewSystemReportWithIdZeroReturnsDepartmentUsageReport()
        {
            #region Arrange
            SetupDataForViewSystemReports();
            ItemRepository.Expect(a => a.GetAll()).Return(Items).Repeat.Once();
            #endregion Arrange

            #region Act
            var result = Controller.ViewSystemReport(0)
                .AssertViewRendered()
                .WithViewData<SystemReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SystemReportData.Count());
            Assert.AreEqual(3, result.SystemReportData.ElementAt(0).Value);
            Assert.AreEqual(2, result.SystemReportData.ElementAt(1).Value);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the view system report with id one returns department money YTD report.
        /// </summary>
        [TestMethod]
        public void TestViewSystemReportWithIdOneReturnsDepartmentMoneyYtdReport()
        {
            #region Arrange
            SetupDataForViewSystemReports();
            TransactionRepository.Expect(a => a.GetAll()).Return(Transactions).Repeat.Once();
            #endregion Arrange

            #region Act
            var result = Controller.ViewSystemReport(1)
                .AssertViewRendered()
                .WithViewData<SystemReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SystemReportData.Count());
            #endregion Assert
        }


        #endregion ViewSystemReports Tests

        #region Helper Methods
        /// <summary>
        /// Setups the data for view system reports.
        /// </summary>
        private void SetupDataForViewSystemReports()
        {
            ControllerRecordFakes.FakeItems(Items, 5);
            ControllerRecordFakes.FakeUnits(Units, 2);
            ControllerRecordFakes.FakeTransactions(Transactions, 7);
            foreach (var item in Items) //To test grouping
            {
                if(item.Id % 2 == 0)
                {
                    item.Unit = Units[0];
                }
                else
                {
                    item.Unit = Units[1];
                }
            }
            foreach (var transaction in Transactions) //To test grouping
            {
                if(transaction.Id %2 == 0)
                {
                    transaction.Item = Items[0];
                }
                else
                {
                    transaction.Item = Items[1];
                }
            }
        }
        private void SetUpDataForTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeItemReports(ItemReports, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";

            ItemRepository.Expect(a => a.GetNullableById(2)).Return(Items[1]).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();

            ItemReportRepository.Expect(a => a.GetNullableById(2)).Return(ItemReports[1]).Repeat.Any();
            ItemReportRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionRepository.Expect(a => a.GetNullableById(0)).Return(null).Repeat.Any();
            QuestionSetRepository.Expect(a => a.GetNullableById(0)).Return(null).Repeat.Any();

            var questionTypes = new List<QuestionType>();
            ControllerRecordFakes.FakeQuestionTypes(questionTypes);
            QuestionTypeRepository.Expect(a => a.Queryable).Return(questionTypes.AsQueryable()).Repeat.Any();

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
            Assert.AreEqual(5, result.Count(), "It looks like a method was added or removed from the controller.");
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
            var controllerMethod = controllerClass.GetMethod("ViewReport");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method create  contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes3.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method ViewSystemReport contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodViewSystemReportContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("ViewSystemReport");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method GenerateChart contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodGenerateChartContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("GenerateChart");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
