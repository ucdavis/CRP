using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region ToggleTransactionIsActive Tests              

        /// <summary>
        /// Tests the toggle transaction is active redirects to list if transaction id not found.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveRedirectsToListIfTransactionIdNotFound()
        {
            #region Arrange
            SetupDataForToggleTransactionIsActiveTests();
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the toggle transaction is active redirects to list if transaction item is null.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveRedirectsToListIfTransactionItemIsNull()
        {
            #region Arrange
            SetupDataForToggleTransactionIsActiveTests();
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(2)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }


        /// <summary>
        /// Tests the toggle transaction is active redirects to list if current user is not admin and not editor.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveRedirectsToListIfCurrentUserIsNotAdminAndNotEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            SetupDataForToggleTransactionIsActiveTests();
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active saves if valid and an editor.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveSavesIfValidAndAnEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            SetupDataForToggleTransactionIsActiveTests();
            Assert.IsTrue(Transactions[3].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(4)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[3]));
            Assert.AreEqual("Transaction has been deactivated.", Controller.Message);
            Assert.IsFalse(Transactions[3].IsActive);
            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active saves if valid and an admin.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveSavesIfValidAndAnAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            SetupDataForToggleTransactionIsActiveTests();
            Assert.IsTrue(Transactions[4].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[4]));
            Assert.AreEqual("Transaction has been deactivated.", Controller.Message);
            Assert.IsFalse(Transactions[4].IsActive);
            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active does not save if paid.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveDoesNotSaveIfPaid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            SetupDataForToggleTransactionIsActiveTests();
            ControllerRecordFakes.FakePaymentLogs(PaymentLogs, 1);
            PaymentLogs[0].Accepted = true;
            PaymentLogs[0].Amount = Transactions[4].Amount;
            Transactions[4].AddPaymentLog(PaymentLogs[0]);
            Assert.IsTrue(Transactions[4].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Transaction was unable to update.", Controller.Message);
            //Assert.IsTrue(Transactions[4].IsActive); Would not be saved...

            //FYI, the following messages do not appear on the UI
            Controller.ModelState.AssertErrorsAre("Paid transactions can not be deactivated.");

            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active when false saves if valid and an admin.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveWhenFalseSavesIfValidAndAnAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            SetupDataForToggleTransactionIsActiveTests();
            Items[2].Quantity = 100;
            Transactions[4].IsActive = false;
            Assert.IsFalse(Transactions[4].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[4]));
            Assert.AreEqual("Transaction has been activated.", Controller.Message);
            Assert.IsTrue(Transactions[4].IsActive);
            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active when false does not save if activation will exceed item quantity.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveWhenFalseDoesNotSaveIfActivationWillExceedItemQuantity()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            SetupDataForToggleTransactionIsActiveTests();
            Items[2].Quantity = 1;
            Transactions[4].IsActive = false;
            Transactions[4].Quantity = 5;
            Assert.IsFalse(Transactions[4].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Transaction was unable to update.", Controller.Message);
   
            //FYI, the following messages do not appear on the UI
            Controller.ModelState.AssertErrorsAre("Transaction can not be activated because it will cause the amount sold to exceed the amount availble.");

            #endregion Assert
        }

        /// <summary>
        /// Tests the toggle transaction is active when transaction invalid does not save.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveWhenTransactionInvalidDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            SetupDataForToggleTransactionIsActiveTests();
            Items[2].Quantity = 100;
            Transactions[4].IsActive = false;
            Transactions[4].Quantity = 5;
            Transactions[4].TransactionAnswers = null; //Invalid
            Assert.IsFalse(Transactions[4].IsActive);
            #endregion Arrange

            #region Act
            Controller.ToggleTransactionIsActive(5)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Transaction was unable to update.", Controller.Message);

            //FYI, the following messages do not appear on the UI
            Controller.ModelState.AssertErrorsAre("");

            #endregion Assert
        }


        #endregion ToggleTransactionIsActive Tests

        private void SetupDataForToggleTransactionIsActiveTests()
        {
            ControllerRecordFakes.FakeTransactions(Transactions, 5);
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);
            Users[1].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[1].AddEditor(Editors[0]);
            Items[1].AddEditor(Editors[1]); //Current User is an editor
            Items[1].AddEditor(Editors[2]);

            //Current user is not an editor
            Items[2].AddEditor(Editors[0]); 
            Items[2].AddEditor(Editors[2]);

            Transactions[1].Item = null; //Shouldn't really ever happen...
            Transactions[2].Item = Items[0];
            Transactions[3].Item = Items[1]; //Is an editor
            Transactions[4].Item = Items[2]; //Is not an editor

            foreach (var transaction in Transactions)
            {
                transaction.Amount = 20m;
                transaction.Quantity = 1;
            }


            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(3)).Return(Transactions[2]).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(4)).Return(Transactions[3]).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(5)).Return(Transactions[4]).Repeat.Any();
        }
    }
}
