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
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests (refunds)
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region SendNotification Tests

        /// <summary>
        /// Tests the SendNotification redirects to item management controller list if id not found.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationRedirectsToItemManagementControllerListIfIdNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransaction(3, TransactionRepository);
            #endregion Arrange

            #region Act
            Controller.SendNotification(4, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Arg<IRepository>.Is.Anything, Arg<Transaction>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the SendNotification redirects to item management controller list if id found but no item.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationRedirectsToItemManagementControllerListIfIdFoundButNoItem()
        {
            #region Arrange

            var transactions = new List<Transaction>(1);
            transactions.Add(CreateValidEntities.Transaction(1));
            transactions[0].Item = null;
            ControllerRecordFakes.FakeTransaction(2, TransactionRepository, transactions);
            #endregion Arrange

            #region Act
            Controller.SendNotification(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Arg<IRepository>.Is.Anything, Arg<Transaction>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the SendNotification redirects to item management controller list if no item access.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationRedirectsToItemManagementControllerListIfNoItemAccess()
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
            Controller.SendNotification(1, "", "")
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Arg<IRepository>.Is.Anything, Arg<Transaction>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestSendNotificationSendsEmail()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var payment = CreateValidEntities.PaymentLog(null);
            payment.Amount = 200.00m;
            payment.Accepted = true;
            payment.Credit = false;
            payment.Check = true;
            Transactions[1].AddPaymentLog(payment);
            Transactions[1].Item.SetIdTo(2);

            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString() + " FID=001";
            parameters.PMT_AMT = Transactions[1].Total;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableById(1)).Return(Transactions[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendNotification(1, "", "")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Notifications-orderBy=&Notifications-page=1#Notifications", result.Url);
            #endregion Assert
        }

        #endregion SendNotification Tests
    }
}
