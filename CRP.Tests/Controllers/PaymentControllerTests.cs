using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
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
    public class PaymentControllerTests : ControllerTestBase<PaymentController>
    {
        protected readonly IPrincipal Principal = new MockPrincipal(false);
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }
        protected IRepository<PaymentLog> PaymentLogRepository { get; set; }
        protected List<Transaction> Transactions { get; set; }
        public INotificationProvider NotificationProvider { get; set; }
        private readonly Type _controllerClass = typeof(PaymentController);

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentControllerTests"/> class.
        /// </summary>
        public PaymentControllerTests()
        {
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Editors = new List<Editor>();
            EditorRepository = FakeRepository<Editor>();
            PaymentLogRepository = FakeRepository<PaymentLog>();
            Controller.Repository.Expect(a => a.OfType<Transaction>()).Return(TransactionRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<PaymentLog>()).Return(PaymentLogRepository).Repeat.Any();
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
            NotificationProvider = MockRepository.GenerateStub<INotificationProvider>();
            Controller = new TestControllerBuilder().CreateController<PaymentController>(NotificationProvider);
        }

        #endregion Init

        #region Mapping/Route Tests

        /// <summary>
        /// Tests the link to transaction with one parameter mapping.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithOneParameterMapping()
        {
            "~/Payment/LinkToTransaction/5".ShouldMapTo<PaymentController>(a => a.LinkToTransaction(5, null, null),true);		
        }


        /// <summary>
        /// Tests the link to transaction with multiple parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithMultipleParametersMapping()
        {
            "~/Payment/LinkToTransaction/".ShouldMapTo<PaymentController>(a => a.LinkToTransaction(5, new PaymentLog[1], null, null),true);		
        }

        #endregion Mapping/Route Tests

        #region LinkToTransaction Get Tests

        /// <summary>
        /// Tests the link to transaction where transaction id not found redirects to item management controller list.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWhereTransactionIdNotFoundRedirectsToItemManagementControllerList()
        {
            #region Arrange
            TransactionRepository.Expect(a => a.GetNullableById(5)).Return(null).Repeat.Any();            
            #endregion Arrange

            #region Act/Assert
            Controller.LinkToTransaction(5, null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act/Assert
        }


        /// <summary>
        /// Tests the link to transaction returns view when transaction id found.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionReturnsViewWhenTransactionIdFoundAndUserIsAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, null, null)
                .AssertViewRendered()
                .WithViewData<LinkPaymentViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.AreEqual(0, result.PaymentLogs.Count());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to transaction returns view when transaction id found and user is editor.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionReturnsViewWhenTransactionIdFoundAndUserIsEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            FakeTransactions(3);
            FakeItems(1);
            FakeUsers(2);
            FakeEditors(2);
            Users[0].LoginID = "OtherGuy";
            Users[1].LoginID = "UserName";
            Editors[0].User = Users[0];
            Editors[1].User = Users[1];
            Items[0].AddEditor(Editors[0]);
            Items[0].AddEditor(Editors[1]);
            Transactions[1].Item = Items[0];
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, null, null)
                .AssertViewRendered()
                .WithViewData<LinkPaymentViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.AreEqual(0, result.PaymentLogs.Count());
            #endregion Assert
        }


        /// <summary>
        /// Tests the test link to transaction returns view with existing payment logs when transaction id found.
        /// </summary>
        [TestMethod]
        public void TestTestLinkToTransactionReturnsViewWithExistingPaymentLogsWhenTransactionIdFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 3; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i+1));
            }
            paymentLogs[1].Credit = true;
            paymentLogs[1].Check = false;
            Transactions[1].AddPaymentLog(paymentLogs[0]);
            Transactions[1].AddPaymentLog(paymentLogs[1]);
            Transactions[1].AddPaymentLog(paymentLogs[2]);
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, null, null)
                .AssertViewRendered()
                .WithViewData<LinkPaymentViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.AreEqual(2, result.PaymentLogs.Count());
            #endregion Assert		
        }

        #endregion LinkToTransaction Get Tests

        #region LinkToTransaction AcceptPost Tests

        /// <summary>
        /// Tests the link to transaction post where id not found redirects to item management controller list.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionPostWhereIdNotFoundRedirectsToItemManagementControllerList()
        {
            #region Arrange
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(null).Repeat.Any();
            var payments = new PaymentLog[1];
            payments[0] = CreateValidEntities.PaymentLog(1);
            #endregion Arrange

            #region Act
            Controller.LinkToTransaction(2, payments, null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to transaction post where no checks saves.
        /// Probably will not happen, but it should still be ok.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionPostWhereNoChecksSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);            
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            var payments = new PaymentLog[0];
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, payments, null, null)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("?Checks-orderBy=&Checks-page=1#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            Assert.AreEqual("Checks associated with transaction.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to transaction with two new checks saves.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithTwoNewChecksSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            Transactions[1].Amount = 20.00m;
            var payments = new PaymentLog[2];
            payments[0] = CreateValidEntities.PaymentLog(1);
            payments[1] = CreateValidEntities.PaymentLog(2);
            Assert.AreEqual(0, Transactions[1].PaymentLogs.Count);
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, payments, null, null)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("?Checks-orderBy=&Checks-page=1#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            Assert.AreEqual("Checks associated with transaction.", Controller.Message);
            Assert.AreEqual(2, Transactions[1].PaymentLogs.Count);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to transaction post with invalid data rolls back.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionPostWithInvalidDataRollsBack1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            var dbContext = MockRepository.GenerateMock<IDbContext>();
            TransactionRepository.Expect(a => a.DbContext).Return(dbContext).Repeat.Any();
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            Transactions[1].Amount = 20.00m;
            var payments = new PaymentLog[2];
            payments[0] = CreateValidEntities.PaymentLog(1);
            payments[1] = CreateValidEntities.PaymentLog(2);
            payments[0].Amount = 20;
            payments[1].Amount = 15;                   
            #endregion Arrange

            #region Act
            Controller.LinkToTransaction(2, payments, null, null)
                .AssertViewRendered()
                .WithViewData<LinkPaymentViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.DbContext.RollbackTransaction());
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("The check amount has exceeded the total amount. Enter a donation first.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to transaction post with invalid data rolls back.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionPostWithInvalidDataRollsBack2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            var dbContext = MockRepository.GenerateMock<IDbContext>();
            TransactionRepository.Expect(a => a.DbContext).Return(dbContext).Repeat.Any();
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            Transactions[1].Amount = 20.00m;
            var payments = new PaymentLog[2];
            payments[0] = CreateValidEntities.PaymentLog(1);
            payments[1] = CreateValidEntities.PaymentLog(2);
            payments[0].Amount = 10;
            payments[1].Amount = 10;
            payments[1].Name = null; //Invalid
            #endregion Arrange

            #region Act
            Controller.LinkToTransaction(2, payments, null, null)
                .AssertViewRendered()
                .WithViewData<LinkPaymentViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.DbContext.RollbackTransaction());
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("At least one check is invalid or incomplete");
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to transaction post where invalid but not accepted check saves.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionPostWhereInvalidButNotAcceptedCheckSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            FakeTransactions(3);
            Transactions[1].Amount = 20;
            TransactionRepository.Expect(a => a.GetNullableById(2)).Return(Transactions[1]).Repeat.Any();
            var payments = new PaymentLog[2];
            payments[0] = CreateValidEntities.PaymentLog(1);
            payments[1] = CreateValidEntities.PaymentLog(2);
            payments[0].Amount = 10;
            payments[1].Amount = 10;
            payments[1].Name = null; //Invalid
            payments[1].Accepted = false;
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, payments, null, null)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("?Checks-orderBy=&Checks-page=1#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            Assert.AreEqual("Checks associated with transaction.", Controller.Message);
            #endregion Assert
        }

        #endregion LinkToTransaction AcceptPost Tests


        [TestMethod]
        public void TestMd5()
        {
            #region Arrange
            const string postingKey = "FB8E61EF5F63028C";
            // ReSharper disable InconsistentNaming
            const string EXT_Trans_Id = "A234";
            const string AMT = "12.35";
            // ReSharper restore InconsistentNaming            
            #endregion Arrange

            #region Act
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(postingKey + EXT_Trans_Id + AMT));
            var result = Convert.ToBase64String(data);
            #endregion Act

            #region Assert
            Assert.AreEqual("hAGcy7esDK7joiFIPJQKRA==", result);
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
        /// Tests the controller has 4 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasFourAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, result.Count());
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
        /// Tests the controller has user only attribute.
        /// </summary>
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
            Assert.AreEqual(2, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to transaction contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToTransactionContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToTransaction");
            #endregion Arrange

            #region Act
            var expectedAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, expectedAttributes.Count(), "Extra Attributes found");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to transaction contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToTransactionContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToTransaction");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
       
        #endregion Controller Method Tests
        #endregion Reflection Tests

        #region Helper Methods

        /// <summary>
        /// Fakes the Transactions.
        /// Author: Sylvestre, Jason
        /// Create: 2010/03/16
        /// </summary>
        /// <param name="count">The number of Transactions to add.</param>
        private void FakeTransactions(int count)
        {
            var offSet = Transactions.Count;
            for (int i = 0; i < count; i++)
            {
                Transactions.Add(CreateValidEntities.Transaction(i + 1 + offSet));
                Transactions[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

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

            public MockPrincipal(bool roleReturnValue)
            {
                RoleReturnValue = roleReturnValue;
            }

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
                //return false;
                return RoleReturnValue;
            }
        }

        /// <summary>
        /// Mock the HTTPContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private int _count;
            public bool RoleReturnValue { get; set; }
            public MockHttpContext(int count, bool roleReturnValue)
            {
                _count = count;
                RoleReturnValue = roleReturnValue;
            }

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal(RoleReturnValue);
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
