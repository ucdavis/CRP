using System.Linq;
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
        #region Checkout Post Tests (Part 6)


        #region Checkout Quantity Answers Tests

        /// <summary>
        /// Tests the checkout quantity answers.
        /// </summary>
        [TestMethod]
        public void TestCheckoutQuantityAnswers()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            //One because we have a TransactionAnswer here
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual(9, args.QuantityAnswers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutQuantityAnswersDoesNotHaveEnoughAnswers()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            QuestionSets[2].AddQuestion(Questions[11]); //Add again
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The number of answers does not match the number of Quantity Level questions.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckoutQuantityAnswersHasTooManyAnswers()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            QuantityAnswerParameters = new QuestionAnswerParameter[10];
            for (int i = 0; i < 10; i++)
            {
                QuantityAnswerParameters[i] = new QuestionAnswerParameter();
            }
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The number of answers does not match the number of Quantity Level questions.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutQuantityAnswersWithValidationErrors1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            Questions[10].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            Questions[10].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            QuantityAnswerParameters[4].Answer = null;
            Items[1].QuantityName = "Populars";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The answer for question \"What is your Last name?\" for Populars 2 is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutQuantityAnswersWithValidationErrors2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            Questions[10].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            Questions[10].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            Questions[10].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Items[1].QuantityName = "Populars";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre(
                "The answer for question \"What is your Last name?\" for Populars 1 is not a valid date.",
                "The answer for question \"What is your Last name?\" for Populars 2 is not a valid date.",
                "The answer for question \"What is your Last name?\" for Populars 3 is not a valid date.");
            #endregion Assert
        }

        #endregion Checkout Quantity Answers Tests

        #region Donation Tests

        [TestMethod]
        public void TestCheckoutWithValidDataAndDonationCreatesChildRecords()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, 15.03m, 75.03m, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(60m, args.Amount);
            Assert.AreEqual(1, args.ChildTransactions.Count);
            Assert.AreEqual(15.03m, args.ChildTransactions.ElementAt(0).Amount);
            Assert.IsTrue(args.ChildTransactions.ElementAt(0).Donation);
            #endregion Assert
        }

        #endregion Donation Tests

        #region Restricted Key Tests
        [TestMethod]
        public void TestCheckoutWithValidDataAndRestrictedKeySaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].RestrictedKey = "Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, (Items[1].CostPerItem * 1), StaticValues.CreditCard, "Test", string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithValidDataAndDifferentRestrictedKeyDoesNotSave1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].RestrictedKey = "Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, (Items[1].CostPerItem * 1), StaticValues.CreditCard, "test", string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The item is restricted please enter the passphrase.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithValidDataAndDifferentRestrictedKeyDoesNotSave2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].RestrictedKey = "Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, (Items[1].CostPerItem * 1), StaticValues.CreditCard, "wrong", string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The item is restricted please enter the passphrase.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithValidDataAndDifferentRestrictedKeyDoesNotSave3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].RestrictedKey = "Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, (Items[1].CostPerItem * 1), StaticValues.CreditCard, string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The item is restricted please enter the passphrase.");
            #endregion Assert
        }


        [TestMethod]
        public void TestCheckoutWithValidDataAndDifferentRestrictedKeyDoesNotSave4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].RestrictedKey = "Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, (Items[1].CostPerItem * 1), StaticValues.CreditCard, null, string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The item is restricted please enter the passphrase.");
            #endregion Assert
        }

        #endregion Restricted Key Tests

        #region Inventory Tests

        [TestMethod]
        public void TestCheckoutWithValidDataButNotEnoughInventoryDoesNotSave()
        {
            #region Arrange
            var fakedate = SetupDataForCheckoutTests();
            Items[1].Quantity = Items[1].Sold + 5;
            Items[1].Expiration = fakedate.AddDays(10);
            Items[1].Available = true;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 6, null, (Items[1].CostPerItem * 6), StaticValues.CreditCard, null, string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("There is not enough inventory to complete your order.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithValidDataButNotEnoughInventorySavesIfHasItemAccess()
        {
            #region Arrange
            var fakedate = SetupDataForCheckoutTests();
            Items[1].Quantity = Items[1].Sold + 5;
            Items[1].Expiration = fakedate.AddDays(10);
            Items[1].Available = true;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 6, null, (Items[1].CostPerItem * 6), StaticValues.CreditCard, null, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithValidDataAndEnoughInventorySaves()
        {
            #region Arrange
            var fakedate = SetupDataForCheckoutTests();
            Items[1].Quantity = Items[1].Sold + 5;
            Items[1].Expiration = fakedate.AddDays(10);
            Items[1].Available = true;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 5, null, (Items[1].CostPerItem * 5), StaticValues.CreditCard, null, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }
        #endregion Inventory Tests

        #region Other Validation Tests

        [TestMethod, Ignore] //This has been changed to automatically change it to a payment type of check
        public void TestCheckoutWithZeroTotalDoesNotSaveIfCreditCard()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Coupons[1].Item = Items[1];
            Coupons[1].Code = "COUPON";
            Coupons[1].IsActive = true;
            Coupons[1].DiscountAmount = Items[1].CostPerItem;
            //Coupons[1].Unlimited = true;
            //Coupons[1].Used = true; //And used
            Coupons[1].MaxQuantity = 2;
            Coupons[1].Email = string.Empty;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, 0, StaticValues.CreditCard, null, "COUPON", TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Please select check payment type when amount is zero.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithZeroTotalDoesSaveIfCreditAndChangedToCheck()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Coupons[1].Item = Items[1];
            Coupons[1].Code = "COUPON";
            Coupons[1].IsActive = true;
            Coupons[1].DiscountAmount = Items[1].CostPerItem;
            //Coupons[1].Unlimited = true;
            //Coupons[1].Used = true; //And used
            Coupons[1].MaxQuantity = 2;
            Coupons[1].Email = string.Empty;
            TransactionRepository.Expect(a => a.GetNullableById(0)).Return(Transactions[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, 0, StaticValues.CreditCard, null, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.IsTrue(args.Check);
            Assert.IsFalse(args.Credit);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithZeroTotalDoesSaveIfCheck()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Coupons[1].Item = Items[1];
            Coupons[1].Code = "COUPON";
            Coupons[1].IsActive = true;
            Coupons[1].DiscountAmount = Items[1].CostPerItem;
            //Coupons[1].Unlimited = true;
            //Coupons[1].Used = true; //And used
            Coupons[1].MaxQuantity = 2;
            Coupons[1].Email = string.Empty;
            TransactionRepository.Expect(a => a.GetNullableById(0)).Return(Transactions[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, 0, StaticValues.Check, null, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithDifferentTotalDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests(); 
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, 0, StaticValues.CreditCard, null, string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("We are sorry, the total amount displayed on the form did not match the total we calculated.");
            #endregion Assert
        }

        #endregion Other Validation Tests

        #endregion Checkout Post Tests (Part 6)
    }
}
