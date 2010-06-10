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
        public void TestRefundRedirectsToItemManagementControllerListIfIdNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransaction(3, TransactionRepository);           
            #endregion Arrange

            #region Act
            Controller.Refund(4, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if id found but no item.
        /// </summary>
        [TestMethod]
        public void TestRefundRedirectsToItemManagementControllerListIfIdFoundButNoItem()
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
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestRefundRedirectsToItemManagementControllerListIfNoItemAccess()
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
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the refund redirects to URL if refund already exists
        /// </summary>
        [TestMethod]
        public void TestRefundRedirectsToUrlIfRefundAlreadyExists1()
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
        public void TestRefundRedirectsToUrlIfRefundAlreadyExists2()
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
        public void TestRefundReturnsViewWhenNoRefundExists()
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
        public void TestRefundReturnsViewWhenNoActiveRefundExists()
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
        public void TestTODO()
        {
            Assert.Inconclusive("Need to write these tests.");
        }
        
        #endregion Refund Post Tests
    }
}
