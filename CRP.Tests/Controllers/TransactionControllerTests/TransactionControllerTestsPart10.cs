using System;
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

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region Confirmation Tests        
        /// <summary>
        /// Tests the confirmation redirects to home when transaction id not found.
        /// </summary>
        [TestMethod]
        public void TestConfirmationRedirectsToHomeWhenTransactionIdNotFound()
        {
            #region Arrange
            SetupDataForConfirmationTests();
            #endregion Arrange

            #region Act/Assert
            Controller.Confirmation(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act/Assert	
        }

        [TestMethod]
        public void TestConfirmationReturnsView()
        {
            #region Arrange
            SetupDataForConfirmationTests();
            #endregion Arrange

            #region Act
            var result = Controller.Confirmation(2)
                .AssertViewRendered()
                .WithViewData<PaymentConfirmationViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SuccessLink);
            Assert.IsNotNull(result.CancelLink);
            Assert.IsNotNull(result.ErrorLink);
            Assert.IsNotNull(result.SiteId);
            Assert.IsNotNull(result.PaymentGatewayUrl);
            Assert.AreEqual("bqjmuo37nABh8nZtuJsVgQ==",result.ValidationKey, "The amount, and other values can cause this hash to change.");
            #endregion Assert		
        }

        #endregion Confirmation Tests

        #region Touchnet Links Tests               

        /// <summary>
        /// Tests the payment success redirects to home with message.
        /// </summary>
        [TestMethod]
        public void TestPaymentSuccessRedirectsToHomeWithMessage()
        {
            #region Arrange
            const string siteId = "17";
            const string transactionId = "12";                      
            #endregion Arrange

            #region Act
            Controller.PaymentSuccess(siteId, transactionId)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Payment Successfully processed", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the payment cancel redirects to home with message.
        /// </summary>
        [TestMethod]
        public void TestPaymentCancelRedirectsToHomeWithMessage()
        {
            #region Arrange
            const string siteId = "17";
            const string transactionId = "12";
            #endregion Arrange

            #region Act
            Controller.PaymentCancel(siteId, transactionId)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Payment Canceled", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the payment error redirects to home with message.
        /// </summary>
        [TestMethod]
        public void TestPaymentErrorRedirectsToHomeWithMessage()
        {
            #region Arrange
            const string siteId = "17";
            const string transactionId = "12";
            #endregion Arrange

            #region Act
            Controller.PaymentError(siteId, transactionId)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Payment Error", Controller.Message);
            #endregion Assert
        }

        #endregion Touchnet Links Tests

        #region Edit Tests

        #region Edit Get Tests


        /// <summary>
        /// Tests the edit get redirects to item management controller when transaction not found.
        /// </summary>
        [TestMethod]
        public void TestEditGetRedirectsToItemManagementControllerWhenTransactionNotFound()
        {
            #region Arrange
            SetupDataForEditTests();           
            #endregion Arrange

            #region Act
            Controller.Edit(1, null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit get redirects to item management controller when transaction item is null.
        /// </summary>
        [TestMethod]
        public void TestEditGetRedirectsToItemManagementControllerWhenTransactionItemIsNull()
        {
            #region Arrange
            SetupDataForEditTests();
            Transactions[1].Item = null;
            #endregion Arrange

            #region Act
            Controller.Edit(2, null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit get redirects to item management controller when no item access.
        /// </summary>
        [TestMethod]
        public void TestEditGetRedirectsToItemManagementControllerWhenNoItemAccess()
        {
            #region Arrange
            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Edit(2, null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }

        #endregion Edit Get Tests

        #endregion Edit Tests



        #region Helper Methods

        /// <summary>
        /// Setup the data for confirmation tests.
        /// </summary>
        private void SetupDataForConfirmationTests()
        {
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("/Home/Index").Repeat.Any();
            
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
           

            
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeUnits(Units, 3);
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            ControllerRecordFakes.FakeDisplayProfile(DisplayProfiles,1);
            Transactions[1].Item = Items[1];
            Items[1].Unit = Units[1];
            DisplayProfiles[0].Unit = Units[1];

            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
        }

        private void SetupDataForEditTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeEditors(Editors, 3);
            Transactions[1].Item = Items[0];
            Transactions[2].Item = Items[1];
            Users[0].LoginID = "UserName";
            for (int i = 0; i < 3; i++)
            {
                Editors[i].User = Users[i];
            }
            Items[1].AddEditor(Editors[1]);
            Items[1].AddEditor(Editors[2]);

            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(3)).Return(Transactions[2]).Repeat.Any();
        }
        #endregion Helper Methods
    }
}
