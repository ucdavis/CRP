using System.Linq;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Core.Resources;
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
        #region Checkout Post Tests (Part 3)
        /// <summary>
        /// Tests the checkout calculates amount correctly.
        /// </summary>
        [TestMethod]
        public void TestCheckoutCalculatesAmountCorrectly()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 6.01m;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(18.03m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout calculates amount correctly with coupon.
        /// </summary>
        [TestMethod]
        public void TestCheckoutCalculatesAmountCorrectlyWithCoupon()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 6.01m;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual((6.01m * 3) - (5.01m * 2), args.Amount);
            #endregion Assert
        }

        #region Checkout Transaction Answer Tests

        /// <summary>
        /// Tests the checkout transaction answers.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswers()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            SetupDataForAllContactInformationTransactionAnswerParameters();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(9, args.TransactionAnswers.Count);
            #endregion Assert
        }

        #region Checkout TextBox Tests

        /// <summary>
        /// Tests the checkout transaction answers text box.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBox1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Has Value";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("Has Value", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers text box.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBox2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual(" ", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }
        //TODO: Rest of text bao with validators too.

        #endregion Checkout TextBox Tests

        //TODO: add a transaction only question set to test the validators

        #endregion Checkout Transaction Answer Tests

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
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
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
        #endregion Checkout Quantity Answers Tests

        //test quantity answers
        //test donation creates another transaction
        //test restricted key
        //test inventory exists


        #endregion Checkout Post Tests (Part 3)
    }
}
