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
        #region Checkout Post Tests (Part 5)
        #region Checkout Transaction Answer Tests continued

        #region Boolean Tests
        /// <summary>
        /// Tests the checkout transaction answers boolean.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersBoolean1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "true";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            Questions[8].Name = "Boolean Test";
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
            Assert.AreEqual("true", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers boolean.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersBoolean2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "false";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            Questions[8].Name = "Boolean Test";
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
            Assert.AreEqual("false", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers boolean.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersBoolean3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            Questions[8].Name = "Boolean Test";
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
            Assert.AreEqual("false", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers boolean.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersBoolean4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "anthingElse";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            Questions[8].Name = "Boolean Test";
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
            Assert.AreEqual("true", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        //Boolean should never have validation applied to it

        #endregion Boolean Tests

        #region Radio Buttons Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButton1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Red";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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
            Assert.AreEqual("Red", args.TransactionAnswers.ElementAt(0).Answer); 
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers radio button2.
        /// I have this test here, but ignored because there is not validation at the controller level
        /// to check the answer is in the list of options.
        /// </summary>
        [TestMethod, Ignore]
        public void TestCheckoutTransactionAnswersRadioButton2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Not In List";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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

        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButton3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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

        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButton4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "No Colour";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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
            Assert.AreEqual("No Colour", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        #region Required Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButtonRequired1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Red";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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
            Assert.AreEqual("Red", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButtonRequired2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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
            Controller.ModelState.AssertErrorsAre("Radio Buttons Test is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersRadioButtonRequired3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            Questions[8].Name = "Radio Buttons Test";
            QuestionOption[] questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
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
            Controller.ModelState.AssertErrorsAre("Radio Buttons Test is a required field");
            #endregion Assert
        }

        #endregion Required Tests

        #endregion Radio Buttons Tests

        //TODO: add a transaction only question set to test the validators

        #endregion Checkout Transaction Answer Tests continued

        #endregion Checkout Post Tests (Part 5)
    }
}
