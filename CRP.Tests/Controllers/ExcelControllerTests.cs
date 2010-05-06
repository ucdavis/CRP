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
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;


namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ExcelControllerTests : ControllerTestBase<ExcelController>
    {
        private readonly Type _controllerClass = typeof(ExcelController);
        protected List<ItemReport> ItemReports { get; set; }
        protected IRepository<ItemReport> ItemReportRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelControllerTests"/> class.
        /// </summary>
        public ExcelControllerTests()
        {
            Editors = new List<Editor>();
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

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
            Controller = new TestControllerBuilder().CreateController<ExcelController>();
        }

        #endregion Init


        [TestMethod]
        public void TestCreateExcelReportRedirectsToListIfReportNotFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            Controller.CreateExcelReport(1, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemReport not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateExcelReportRedirectsToListIfItemNotFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            Controller.CreateExcelReport(2, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateExcelReportRedirectsToListIfNoItemAccess()
        {
            #region Arrange
            SetupDataForTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.CreateExcelReport(2, 2)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateExcelReportWhenAdmin()
        {
            #region Arrange
            SetupDataForTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            Controller.CreateExcelReport(2, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.AdminHome());
            #endregion Act

            #region Assert
            Assert.AreEqual("Error Creating Excel Report The method or operation is not implemented.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateExcelReportWhenHasItemAccess()
        {
            #region Arrange
            SetupDataForTests();
            Items[1].AddEditor(Editors[0]);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.CreateExcelReport(2, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.AdminHome());
            #endregion Act

            #region Assert
            Assert.AreEqual("Error Creating Excel Report The method or operation is not implemented.", Controller.Message);
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
        public void TestControllerHasUserOnlyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UserOnlyAttribute not found.");
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
        /// Tests the controller method list contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateExcelReportContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("CreateExcelReport");
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
        private void SetupDataForTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });

            ControllerRecordFakes.FakeItemReports(ItemReports, 3);
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);

            ItemReportRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            ItemReportRepository.Expect(a => a.GetNullableByID(2)).Return(ItemReports[1]).Repeat.Any();

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
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
            //This will get past the code, but not allow an openId to be assigned to the transaction.
            public override HttpCookieCollection Cookies
            {
                get
                {
                    try
                    {
                        return new HttpCookieCollection();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            //This is for viewModel.SuccessLink = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Action("PaymentSuccess", "Transaction"));
            public override Uri Url
            {
                get
                {
                    string url = "http://www.Sample.com/somefolder/getStuff.aspx?id=1&var2=abc&var3=55";
                    Uri uri = new Uri(url);

                    return uri;
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
