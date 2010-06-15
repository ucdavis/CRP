using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests (refunds)
    /// </summary>
    public partial class TransactionControllerTests
    {
        //Test Refund Get, Post
        //test Remove Refund
        //Test Details Refund

        #region Refund Get Tests


        /// <summary>
        /// Tests the refund redirects to item management controller list if id not found.
        /// </summary>
        [TestMethod]
        public void TestRefundGetRedirectsToItemManagementControllerListIfIdNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransaction(3, TransactionRepository);           
            #endregion Arrange

            #region Act
            Controller.Refund(4, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if id found but no item.
        /// </summary>
        [TestMethod]
        public void TestRefundGetRedirectsToItemManagementControllerListIfIdFoundButNoItem()
        {
            #region Arrange

            var transactions = new List<Transaction>(1);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions[0].Item = null;
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.Refund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestRefundGetRedirectsToItemManagementControllerListIfNoItemAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.Refund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to URL if refund already exists
        /// </summary>
        [TestMethod]
        public void TestRefundGetRedirectsToUrlIfRefundAlreadyExists1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[1]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            transactions[1].AddChildTransaction(refund);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(2, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            Assert.AreEqual("Active Refund already exists.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to URL if refund already exists
        /// </summary>
        [TestMethod]
        public void TestRefundGetRedirectsToUrlIfRefundAlreadyExists2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            //Items[1].AddEditor(Editors[0]); //Try using admin above.
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[1]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            transactions[1].AddChildTransaction(refund);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(2, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            Assert.AreEqual("Active Refund already exists.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund returns view when no refund exists.
        /// </summary>
        [TestMethod]
        public void TestRefundGetReturnsViewWhenNoRefundExists()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            //Items[1].AddEditor(Editors[0]); //Try using admin above.
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            //var refund = new Transaction(Items[1]);
            //refund.Refunded = true;
            //refund.Amount = 0.01m;
            //transactions[1].AddChildTransaction(refund);

            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[1], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(2, "", "")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual(" FID=001", result.Fid);
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund returns view when no active refund exists.
        /// </summary>
        [TestMethod]
        public void TestRefundGetReturnsViewWhenNoActiveRefundExists()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            //Items[1].AddEditor(Editors[0]); //Try using admin above.
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[1]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            refund.IsActive = false;
            transactions[1].AddChildTransaction(refund);

            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[1], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(2, "", "")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual(" FID=001", result.Fid);
            #endregion Assert
        }

        #endregion Refund Get Tests

        #region Refund Post Tests

        [TestMethod]
        public void TestRefundPostWhenTransactionIdNotFoundRedirects() 
        {
            #region Arrange
            var transaction = CreateValidEntities.Transaction(99);
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository);
            #endregion Arrange

            #region Act
            Controller.Refund(transaction,"", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestRefundPostRedirectsIfTransactionItemIsNull()
        {
            #region Arrange
            var transactions = new List<Transaction>(1);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions[0].Item = null;
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.Refund(transactions[0], "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert	
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestRefundPostRedirectsToItemManagementControllerListIfNoItemAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.Refund(transactions[0], "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestRefundPostCreatesChildTransactionWithValidData()
        {
            #region Arrange
           Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];


            transactions[1].Amount = 20m;
            var payment = CreateValidEntities.PaymentLog(1);
            payment.Accepted = true;
            payment.Amount = 20m;
            payment.Credit = true;
            payment.Check = !payment.Credit;

            transactions[1].AddPaymentLog(payment);

            var transactionrefund = CreateValidEntities.Transaction(2);
            transactionrefund.Amount = 0.01m;
            transactionrefund.CorrectionReason = "Test";
            transactionrefund.SetIdTo(2);


            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[1], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(transactionrefund, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.ChildTransactions.Count());
            Assert.IsTrue(args.RefundIssued);
            Assert.AreEqual(0.01m, args.RefundAmount);
            Assert.AreEqual(19.99m, args.TotalPaid);
            Assert.AreEqual("Test", args.ChildTransactions.ElementAt(0).CorrectionReason);
            #endregion Assert	
        }

        [TestMethod]
        public void TestRefundPostWithTooLargeRefundAmountDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                 .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                 .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];


            transactions[1].Amount = 20m;
            var payment = CreateValidEntities.PaymentLog(1);
            payment.Accepted = true;
            payment.Amount = 20m;
            payment.Credit = true;
            payment.Check = !payment.Credit;

            transactions[1].AddPaymentLog(payment);

            var transactionrefund = CreateValidEntities.Transaction(2);
            transactionrefund.Amount = 20.01m;
            transactionrefund.CorrectionReason = "refund Too Big";
            transactionrefund.SetIdTo(2);


            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[1], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(transactionrefund, "", "")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("The refund amount must not exceed the amount already paid.");
            Assert.IsNotNull(result);
            Assert.AreEqual("refund Too Big", result.CorrectionReason);
            Assert.AreEqual(20.01m, result.RefundAmount);
            #endregion Assert
        }

        [TestMethod]
        public void TestRefundPostWithNegativeRefundAmountDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                 .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                 .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];


            transactions[1].Amount = 20m;
            var payment = CreateValidEntities.PaymentLog(1);
            payment.Accepted = true;
            payment.Amount = 20m;
            payment.Credit = true;
            payment.Check = !payment.Credit;

            transactions[1].AddPaymentLog(payment);

            var transactionrefund = CreateValidEntities.Transaction(2);
            transactionrefund.Amount = -10m;
            transactionrefund.CorrectionReason = "Neg amount doesn't save";
            transactionrefund.SetIdTo(2);


            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[1], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.Refund(transactionrefund, "", "")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Refund Amount must be greater than zero.");
            Assert.IsNotNull(result);
            Assert.AreEqual("Neg amount doesn't save", result.CorrectionReason);
            Assert.AreEqual(-10m, result.RefundAmount);
            #endregion Assert
        }
        #endregion Refund Post Tests

        #region RemoveRefund Tests
        /// <summary>
        /// Tests the refund redirects to item management controller list if id not found.
        /// </summary>
        [TestMethod]
        public void TestRemoveRefundRedirectsToItemManagementControllerListIfIdNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransaction(3, TransactionRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveRefund(4, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if id found but no item.
        /// </summary>
        [TestMethod]
        public void TestRemoveRefundRedirectsToItemManagementControllerListIfIdFoundButNoItem()
        {
            #region Arrange

            var transactions = new List<Transaction>(1);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions[0].Item = null;
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.RemoveRefund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestRemoveRefundRedirectsToItemManagementControllerListIfNoItemAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.RemoveRefund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestRemoveRefundReturnsToListWhenNoRefundsExist()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveRefund(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Refund not found.", Controller.Message);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveRefundReturnsToListWhenNoActiveRefundsExist()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[0]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            refund.IsActive = false;
            transactions[0].AddChildTransaction(refund);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveRefund(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Refund not found.", Controller.Message);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveRefundInactivatesActiveRefundAndSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[0]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            refund.IsActive = true;
            transactions[0].AddChildTransaction(refund);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveRefund(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.ChildTransactions.Count());
            Assert.IsFalse(args.RefundIssued);
            Assert.AreEqual(0m, args.RefundAmount);
            Assert.IsFalse(args.ChildTransactions.ElementAt(0).IsActive);
            #endregion Assert
        }
        #endregion RemoveRefund Tests

        #region DetailsRefund Test
        /// <summary>
        /// Tests the refund redirects to item management controller list if id not found.
        /// </summary>
        [TestMethod]
        public void TestDetailsRefundRedirectsToItemManagementControllerListIfIdNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransaction(3, TransactionRepository);
            #endregion Arrange

            #region Act
            Controller.DetailsRefund(4, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if id found but no item.
        /// </summary>
        [TestMethod]
        public void TestDetailsRefundRedirectsToItemManagementControllerListIfIdFoundButNoItem()
        {
            #region Arrange

            var transactions = new List<Transaction>(1);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions[0].Item = null;
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.DetailsRefund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestDetailsRefundRedirectsToItemManagementControllerListIfNoItemAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.DetailsRefund(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsRefundReturnsToListWhenNoRefundsExist()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];
            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.DetailsRefund(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Refund not found.", Controller.Message);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsRefundReturnsToListWhenNoActiveRefundsExist()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[0]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            refund.IsActive = false;
            transactions[0].AddChildTransaction(refund);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.DetailsRefund(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Refund not found.", Controller.Message);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Refunds-orderBy=&Refunds-page=1#Refunds", result.Url);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsRefundReturnsExpectedViewData()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Refunder, RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);

            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[0].AddEditor(Editors[1]);
            Items[0].AddEditor(Editors[2]);

            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);
            var transactions = new List<Transaction>(2);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions.Add(CreateValidEntities.Transaction(2));
            transactions[0].Item = Items[0];
            transactions[1].Item = Items[1];

            var refund = new Transaction(Items[0]);
            refund.Refunded = true;
            refund.Amount = 0.01m;
            refund.IsActive = true;
            refund.CorrectionReason = "Detail reason here";
            refund.CreatedBy = "NotMe";
            transactions[0].AddChildTransaction(refund);
            transactions[0].Amount = 20m;

            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(transactions[0], QuestionSets[0], OpenIdUsers[1]);

            ControllerRecordFakes.FakeTransaction(1, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            var result = Controller.DetailsRefund(1, "", "")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.AreEqual(0.01m, result.RefundAmount);
            Assert.AreEqual("Email@maily.org", result.ContactEmail);
            Assert.AreEqual("FirstName LastName", result.ContactName);
            Assert.AreEqual("Detail reason here", result.CorrectionReason);
            Assert.AreEqual("NotMe", result.CreatedBy);
            Assert.AreEqual(" FID=001", result.Fid);
            #endregion Assert	
        }


        #endregion DetailsRefund Test
    }
}
