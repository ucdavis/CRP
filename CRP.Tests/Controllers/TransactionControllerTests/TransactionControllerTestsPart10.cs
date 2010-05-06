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
            Transactions[1].TransactionGuid = new Guid("c7e4f7cc-670e-48c2-8f0a-03262180aa67");
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
            Assert.AreEqual(@"fyQbylaAdaiir//SYXs1SA==", result.ValidationKey, "The amount, and other values can cause this hash to change.");
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


        /// <summary>
        /// Tests the edit get returns view when editor access.
        /// </summary>
        [TestMethod]
        public void TestEditGetReturnsViewWhenEditorAccess1()
        {
            #region Arrange
            SetupDataForEditTests();
            Items[1].AddEditor(Editors[0]);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });            
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, null, null)
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName LastName", result.ContactName);
            Assert.AreEqual("Email@maily.org", result.ContactEmail);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit get returns view when editor access.
        /// </summary>
        [TestMethod]
        public void TestEditGetReturnsViewWhenEditorAccess2()
        {
            #region Arrange
            SetupDataForEditTests();
            //Items[1].AddEditor(Editors[0]);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, null, null)
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName LastName", result.ContactName);
            Assert.AreEqual("Email@maily.org", result.ContactEmail);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetPopulatesPageAndSortWithValidValuesOnly1()
        {
            #region Arrange
            SetupDataForEditTests();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, null, "-2")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.Page);
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditGetPopulatesPageAndSortWithValidValuesOnly2()
        {
            #region Arrange
            SetupDataForEditTests();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, "MyQuantity-asc", "25")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("25", result.Page);
            Assert.AreEqual("", result.Sort);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetPopulatesPageAndSortWithValidValuesOnly3()
        {
            #region Arrange
            SetupDataForEditTests();
            var validSort = new List<string>();
            validSort.Add("TransactionNumber-asc");
            validSort.Add("TransactionNumber-desc");
            validSort.Add("Quantity-asc");
            validSort.Add("Quantity-desc");
            validSort.Add("Paid-asc");
            validSort.Add("Paid-desc");
            validSort.Add("IsActive-asc");
            validSort.Add("IsActive-desc");
            #endregion Arrange

            var counter = 0;
            foreach (var sortValue in validSort)
            {
                counter++;
                #region Act
                var result = Controller.Edit(3, sortValue, counter.ToString())
                    .AssertViewRendered()
                    .WithViewData<EditTransactionViewModel>();
                #endregion Act

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(counter.ToString(), result.Page);
                Assert.AreEqual(sortValue, result.Sort);
                #endregion Assert
            }            
        }

        #endregion Edit Get Tests

        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostRedirectsToItemManagementControllerWhenTransactionNotFound()
        {
            #region Arrange
            SetupDataForEditTests();
            #endregion Arrange

            #region Act
            Controller.Edit(Transactions[0], null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Transaction not found.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsToItemManagementControllerWhenTransactionItemIsNull()
        {
            #region Arrange
            SetupDataForEditTests();
            Transactions[1].Item = null;
            #endregion Arrange

            #region Act
            Controller.Edit(Transactions[1], null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToItemManagementControllerWhenNoItemAccess()
        {
            #region Arrange
            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Edit(Transactions[1], null, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostCreatesADonationWithValidValuesAndPositiveAmount()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var newTransaction = new Transaction();
            newTransaction.SetIdTo(Transactions[2].Id);
            newTransaction.Amount = 12.06m;
            newTransaction.CorrectionReason = "They wanted to make a donation";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(newTransaction, null, null)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Checks-orderBy=&Checks-page=1#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[2]));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.AreEqual(1, args.ChildTransactions.Count);
            Assert.AreEqual(12.06m, args.ChildTransactions.ElementAt(0).Amount);
            Assert.IsTrue(args.ChildTransactions.ElementAt(0).Donation);
            Assert.AreEqual("They wanted to make a donation", args.ChildTransactions.ElementAt(0).CorrectionReason);
            Assert.AreEqual("UserName", args.ChildTransactions.ElementAt(0).CreatedBy);
            Assert.AreSame(Transactions[2], args.ChildTransactions.ElementAt(0).ParentTransaction);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostCreatesADonationWithValidValuesAndNegativeAmount()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var newTransaction = new Transaction();
            newTransaction.SetIdTo(Transactions[2].Id);
            newTransaction.Amount = -12.07m;
            newTransaction.CorrectionReason = "oops, not a donation";
            Transactions[2].Amount = 20.00m;
            var oldDonation = new Transaction(Transactions[2].Item);
            oldDonation.Amount = 20.00m;
            oldDonation.Donation = true;
            Transactions[2].AddChildTransaction(oldDonation);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(newTransaction, "Quantity-asc", "2")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Checks-orderBy=Quantity-asc&Checks-page=2#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[2]));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.AreEqual(2, args.ChildTransactions.Count);
            Assert.AreEqual(-12.07m, args.ChildTransactions.ElementAt(1).Amount);
            Assert.IsFalse(args.ChildTransactions.ElementAt(1).Donation);
            Assert.AreEqual("oops, not a donation", args.ChildTransactions.ElementAt(1).CorrectionReason);
            Assert.AreEqual("UserName", args.ChildTransactions.ElementAt(1).CreatedBy);
            Assert.AreSame(Transactions[2], args.ChildTransactions.ElementAt(1).ParentTransaction);
            Assert.AreEqual(7.93m, args.DonationTotal);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostCreatesADonationWithValidValuesAndNegativeAmountWithPaymentLogs()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var newTransaction = new Transaction();
            newTransaction.SetIdTo(Transactions[2].Id);
            newTransaction.Amount = -12.07m;
            newTransaction.CorrectionReason = "oops, not a donation";
            Transactions[2].Amount = 20.00m;
            var oldDonation = new Transaction(Transactions[2].Item);
            oldDonation.Amount = 20.00m;
            oldDonation.Donation = true;
            Transactions[2].AddChildTransaction(oldDonation);
            var paymentLog = new PaymentLog();
            paymentLog.Amount = 27.93m;
            paymentLog.Check = true;
            paymentLog.Accepted = true;
            paymentLog.CheckNumber = 1;
            paymentLog.DatePayment = DateTime.Now.Date;
            paymentLog.Name = "Jon";
            Transactions[2].AddPaymentLog(paymentLog);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(newTransaction, "Quantity-asc", "2")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("http://sample.com/ItemManagement/Details/2?Checks-orderBy=Quantity-asc&Checks-page=2#Checks", result.Url);
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[2]));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.AreEqual(2, args.ChildTransactions.Count);
            Assert.AreEqual(-12.07m, args.ChildTransactions.ElementAt(1).Amount);
            Assert.IsFalse(args.ChildTransactions.ElementAt(1).Donation);
            Assert.AreEqual("oops, not a donation", args.ChildTransactions.ElementAt(1).CorrectionReason);
            Assert.AreEqual("UserName", args.ChildTransactions.ElementAt(1).CreatedBy);
            Assert.AreSame(Transactions[2], args.ChildTransactions.ElementAt(1).ParentTransaction);
            Assert.AreEqual(7.93m, args.DonationTotal);
            Assert.IsTrue(args.Paid);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostCreatesADonationWithValidValuesAndNegativeAmountWithPaymentLogsMoreThanCorrectyionWillAllow()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Details/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            SetupDataForEditTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var newTransaction = new Transaction();
            newTransaction.SetIdTo(Transactions[2].Id);
            newTransaction.Amount = -12.07m;
            newTransaction.CorrectionReason = "oops, not a donation";
            Transactions[2].Amount = 20.00m;
            var oldDonation = new Transaction(Transactions[2].Item);
            oldDonation.Amount = 20.00m;
            oldDonation.Donation = true;
            Transactions[2].AddChildTransaction(oldDonation);
            var paymentLog = new PaymentLog();
            paymentLog.Amount = 30.00m;  //Can't reduce below what has been paid.
            paymentLog.Check = true;
            paymentLog.Accepted = true;
            paymentLog.CheckNumber = 1;
            paymentLog.DatePayment = DateTime.Now.Date;
            paymentLog.Name = "Jon";
            Transactions[2].AddPaymentLog(paymentLog);
            #endregion Arrange

            #region Act

            var result = Controller.Edit(newTransaction, "Quantity-asc", "2")
                .AssertViewRendered()
                .WithViewData<EditTransactionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("The total of all correction amounts must not exceed the amount already paid.");

            #endregion Assert
        }
        #endregion Edit Post Tests


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


            SetupDataForPopulateItemTransactionAnswer();
            LoadTransactionAnswers(Transactions[2], QuestionSets[0], OpenIdUsers[1]);
            

            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(2)).Return(Transactions[1]).Repeat.Any();
            TransactionRepository.Expect(a => a.GetNullableByID(3)).Return(Transactions[2]).Repeat.Any();
        }

        private void LoadTransactionAnswers(Transaction transaction, QuestionSet questionSet, OpenIdUser openIdUser)
        {
            if (questionSet != null)
            {
                var questionAnswer = new Dictionary<string, string>();
                questionAnswer.Add(StaticValues.Question_FirstName, openIdUser.FirstName);
                questionAnswer.Add(StaticValues.Question_LastName, openIdUser.LastName);
                questionAnswer.Add(StaticValues.Question_StreetAddress, openIdUser.StreetAddress);
                questionAnswer.Add(StaticValues.Question_AddressLine2, openIdUser.Address2);
                questionAnswer.Add(StaticValues.Question_City, openIdUser.City);
                questionAnswer.Add(StaticValues.Question_State, openIdUser.State);
                questionAnswer.Add(StaticValues.Question_Zip, openIdUser.Zip);
                questionAnswer.Add(StaticValues.Question_PhoneNumber, openIdUser.PhoneNumber);
                questionAnswer.Add(StaticValues.Question_Email, openIdUser.Email);
                foreach (var question in questionSet.Questions)
                {
                    //If it doesn't find the question, it will thow an exception. (a good thing.)
                    var ans = questionAnswer[question.Name];
                    // create the answer object
                    var answer = new TransactionAnswer(transaction, questionSet, question, ans);
                    transaction.AddTransactionAnswer(answer);
                }
            }
        }

        #endregion Helper Methods
    }
}
