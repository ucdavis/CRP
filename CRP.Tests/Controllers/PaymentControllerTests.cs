using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Util;
using CRP.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using UCDArch.Testing;
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
    [TestClass]
    public class PaymentControllerTests : ControllerTestBase<PaymentController>
    {
        protected IRepository<Transaction> TransactionRepository { get; set; }
        protected IRepository<PaymentLog> PaymentLogRepository { get; set; }
        protected List<Transaction> Transactions { get; set; }
        private readonly Type _controllerClass = typeof(PaymentController);

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentControllerTests"/> class.
        /// </summary>
        public PaymentControllerTests()
        {
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
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
            Controller = new TestControllerBuilder().CreateController<PaymentController>();
        }

        #endregion Init

        #region Mapping/Route Tests

        /// <summary>
        /// Tests the link to transaction with one parameter mapping.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithOneParameterMapping()
        {
            "~/Payment/LinkToTransaction/5".ShouldMapTo<PaymentController>(a => a.LinkToTransaction(5),true);		
        }


        /// <summary>
        /// Tests the link to transaction with multiple parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithMultipleParametersMapping()
        {
            "~/Payment/LinkToTransaction/".ShouldMapTo<PaymentController>(a => a.LinkToTransaction(5, new PaymentLog[1]),true);		
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
            TransactionRepository.Expect(a => a.GetNullableByID(5)).Return(null).Repeat.Any();            
            #endregion Arrange

            #region Act/Assert
            Controller.LinkToTransaction(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act/Assert
        }


        /// <summary>
        /// Tests the link to transaction returns view when transaction id found.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionReturnsViewWhenTransactionIdFound()
        {
            #region Arrange
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2)
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
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
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
            var result = Controller.LinkToTransaction(2)
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
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            var payments = new PaymentLog[1];
            payments[0] = CreateValidEntities.PaymentLog(1);
            #endregion Arrange

            #region Act
            Controller.LinkToTransaction(2, payments)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
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
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);            
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            var payments = new PaymentLog[0];
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, payments)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("#Checks", result.Url);
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
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            FakeTransactions(3);
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            Transactions[1].Amount = 20.00m;
            var payments = new PaymentLog[2];
            payments[0] = CreateValidEntities.PaymentLog(1);
            payments[1] = CreateValidEntities.PaymentLog(2);
            Assert.AreEqual(0, Transactions[1].PaymentLogs.Count);
            #endregion Arrange

            #region Act
            var result = Controller.LinkToTransaction(2, payments)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            Assert.AreEqual("Checks associated with transaction.", Controller.Message);
            Assert.AreEqual(2, Transactions[1].PaymentLogs.Count);
            #endregion Assert		
        }

        #endregion LinkToTransaction AcceptPost Tests


        [TestMethod]
        public void TestMD5()
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
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
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
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
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

        #endregion Helper Methods
    }
}
