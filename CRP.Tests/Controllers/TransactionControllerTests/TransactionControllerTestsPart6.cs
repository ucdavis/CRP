using System.Linq;
using CRP.Controllers;
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

        /// <summary>
        /// Tests the checkout transaction answers text box.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBox3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
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
            Assert.AreEqual(string.Empty, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers text box.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBox4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
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
            Assert.IsNull(args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        #region Required Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxRequiredValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Has Value";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Required";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
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

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxRequiredValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Required";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
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

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxRequiredValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Required";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Required is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxRequiredValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Required";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Required is a required field");
            #endregion Assert
        }
        
        #endregion Required Validator Tests

        #region Email Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "test@test.com";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
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
            Assert.AreEqual("test@test.com", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Text@Somewhere";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxEmailValidators6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "ucdavis.Edu";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Email is not a valid email.");
            #endregion Assert
        }
        

        #endregion Email Validator Tests

        #region Url Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"http://test.caes.ucdavis.edu/crp";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
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
            Assert.AreEqual(@"http://test.caes.ucdavis.edu/crp", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Text@Somewhere";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "ucdavis.Edu";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxUrlValidators7()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "test@test.com";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Url is not a valid url.");
            #endregion Assert
        }

        #endregion Url Validator Tests

        #region Date Validator Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"2001/01/01";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
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
            Assert.AreEqual(@"2001/01/01", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"2001/1/01";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
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
            Assert.AreEqual(@"2001/1/01", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"2001-01-01";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
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
            Assert.AreEqual(@"2001-01-01", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"2001-9-30";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
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
            Assert.AreEqual(@"2001-9-30", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"2001-9-4";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
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
            Assert.AreEqual(@"2001-9-4", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators7()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators8()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators9()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators10()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "2001\01\01";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators11()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "04-05-2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators12()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "04-2001-30";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxDateValidators13()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "January 3, 2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Date";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Date is not a valid date.");
            #endregion Assert
        }
        #endregion Date Validator Tests

        #region Phone Number Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555 555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("555 555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "(555) 555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("(555) 555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555-555-5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("555-555-5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "(555)555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("(555)555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators7()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators8()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "            ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextBoxPhoneNumberValidators9()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            Questions[8].Name = "Text Box Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Box Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }
        #endregion Phone Number Validator Tests

        #endregion Checkout TextBox Tests

       
        #endregion Checkout Post Tests (Part 3)

        #endregion Checkout Post Tests (Part 3)
    }
}
