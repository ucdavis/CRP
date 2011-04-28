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
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            TransactionAnswerParameters[0].Answer = "any thing Else";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            Questions[8].Name = "Boolean Test";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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

        #region CheckBox List Tests

        /// <summary>
        /// Tests the checkout transaction answers check box list1.
        /// Check without validators and one option selected
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxList1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new[]{"Red"};
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        /// Tests the checkout transaction answers check box list2.
        /// Check without validators and two option selected
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxList2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new[] { "Red", "Blue" };
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("Red, Blue", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers check box list3.
        /// All options Selected
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxList3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new[] { "Red", "Blue", "No Colour" };
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("Red, Blue, No Colour", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers check box list4.
        /// Nothing picked, empty string in answer
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxList4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersCheckBoxList5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new[]{string.Empty};
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersCheckBoxListWithRequired1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new[] { "Red" };
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        /// Tests the checkout transaction answers check box list with required2.
        /// Required and no value should fail
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxListWithRequired2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Checkbox List Test is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersCheckBoxListWithRequired3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].CblAnswer = new []{string.Empty};
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            Questions[8].Name = "Checkbox List Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Checkbox List Test is a required field");
            #endregion Assert
        }
        #endregion CheckBox List Tests

        #region Drop Down Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersDropDown1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Red";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        /// Tests the checkout transaction answers DropDown.
        /// I have this test here, but ignored because there is not validation at the controller level
        /// to check the answer is in the list of options.
        /// </summary>
        [TestMethod, Ignore]
        public void TestCheckoutTransactionAnswersDropDown2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Not In List";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersDropDown3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersDropDown4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "No Colour";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersDropDownRequired1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Red";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
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
        public void TestCheckoutTransactionAnswersDropDownRequired2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Drop Down Test is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDropDownRequired3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            Questions[8].Name = "Drop Down Test";
            var questionOptions = new QuestionOption[3];
            questionOptions[0] = new QuestionOption("Blue");
            questionOptions[1] = new QuestionOption("Red");
            questionOptions[2] = new QuestionOption("No Colour");
            Questions[8].AddOption(questionOptions[0]);
            Questions[8].AddOption(questionOptions[1]);
            Questions[8].AddOption(questionOptions[2]);
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Drop Down Test is a required field");
            #endregion Assert
        }

        #endregion Required Tests
        

        #endregion Drop Down Tests

        #region Date Tests

        #region Valid Format Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersDate1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "02/29/2000";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("02/29/2000", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDate2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "02/28/2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("02/28/2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDate3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "12/31/2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("12/31/2010", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDate4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "1/1/2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("1/1/2010", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDate5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "1-1-2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("1-1-2010", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }
        #endregion Valid Format Tests

        #region Invalid Format Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersDateInvalid1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "1x1x2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateInvalid2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "1.1.2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateInvalid3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "13/01/2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateInvalid4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "02/29/2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateInvalid5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "00/01/2010";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is not a valid date.");
            #endregion Assert
        }

        #endregion Invalid Format Tests

        #region Not Required

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateNotRequired1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateNotRequired2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual("", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        #endregion Not Required

        #region Required

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateRequired1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersDateRequired2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            Questions[8].Name = "Date Test";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Date Test is a required field");
            #endregion Assert
        }

        #endregion Required

        #endregion Date Tests

        #endregion Checkout Transaction Answer Tests continued

        #endregion Checkout Post Tests (Part 5)
    }
}
