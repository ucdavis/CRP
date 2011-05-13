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
        #region Checkout Post Tests (Part 4)
        #region Checkout Transaction Answer Tests continued
        #region Text Area Tests
        /// <summary>
        /// Tests the checkout transaction answers text area1.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextArea1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "This is the  Answer to my text area question.  New Para.";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test";
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
            Assert.AreEqual("This is the  Answer to my text area question.  New Para.", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }


        [TestMethod]
        public void TestCheckoutTransactionAnswersTextArea2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test";
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
            Assert.AreEqual(" ", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout transaction answers text Area.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextArea3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test";
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

        /// <summary>
        /// Tests the checkout transaction answers text Area.
        /// </summary>
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextArea4() //Note Change on May 13, 2011 caused this to fail. The only reason I can think it passed before was a database change to the answers table - JCS
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test";
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
            Assert.IsNull(args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        #region Required Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaRequiredValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Has Value";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Required";
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
            Assert.AreEqual("Has Value", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaRequiredValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Required";
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
            Assert.AreEqual(" ", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaRequiredValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Required";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Required is a required field");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaRequiredValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Required";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Required is a required field");
            #endregion Assert
        }

        #endregion Required Validator Tests

        #region Email Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "test@test.com";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
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
            Assert.AreEqual("test@test.com", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators3()
        {
            //This isn't allowed anymore...
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
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
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Email is not a valid email.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators4() //Note Change on May 13, 2011 caused this to fail. The only reason I can think it passed before was a database change to the answers table - JCS
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
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
            Assert.AreEqual(null, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Email is not a valid email.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Text@Somewhere";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Email is not a valid email.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaEmailValidators6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "ucdavis.Edu";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Email";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Email is not a valid email.");
            #endregion Assert
        }


        #endregion Email Validator Tests

        #region URL Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"http://test.caes.ucdavis.edu/crp";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
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
            Assert.AreEqual(@"http://test.caes.ucdavis.edu/crp", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
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
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators4() //Note Change on May 13, 2011 caused this to fail. The only reason I can think it passed before was a database change to the answers table - JCS
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
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
            Assert.AreEqual(null, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "Text@Somewhere";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "ucdavis.Edu";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaUrlValidators7()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "test@test.com";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Url";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Url is not a valid url.");
            #endregion Assert
        }

        #endregion Url Validator Tests

        #region Date Validator Tests
        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"01/01/2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(@"01/01/2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"01/1/2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(@"01/1/2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"01-01-2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(@"01-01-2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"9-30-2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(@"9-30-2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = @"9-4-2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(@"9-4-2001", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators7() //Note Change on May 13, 2011 caused this to fail. The only reason I can think it passed before was a database change to the answers table - JCS
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(null, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators8()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Assert.AreEqual(string.Empty, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators9()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators10()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "2001\01\01";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators11()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "02-29-2001"; //Not a leap year
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators12()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "04-2001-30";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaDateValidators13()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "January 3, 2001";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Date";
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
            Controller.ModelState.AssertErrorsAre("Text Area Test With Date is not a valid date.");
            #endregion Assert
        }
        #endregion Date Validator Tests

        #region Phone Number Validator Tests

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators1()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555 555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("555 555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators2()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "(555) 555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("(555) 555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555-555-5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("555-555-5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "(555)555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual("(555)555 5555", args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators5()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "555 5555";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators6() //Note Change on May 13, 2011 caused this to fail. The only reason I can think it passed before was a database change to the answers table - JCS
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = null;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            Assert.AreEqual(null, args.TransactionAnswers.ElementAt(0).Answer);
            #endregion Assert
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Phone Number is not a valid phone number.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators7()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = string.Empty;
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
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
            //    .AssertViewRendered()
            //    .WithViewData<ItemDetailViewModel>();
            //#endregion Act

            //#region Assert
            //TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            //Assert.IsNull(Controller.Message);
            //Controller.ModelState.AssertErrorsAre("Text Area Test With Phone Number is not a valid phone number.");
            //#endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators8()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "            ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutTransactionAnswersTextAreaPhoneNumberValidators9()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = " ";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
            Questions[8].QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            Questions[8].Name = "Text Area Test With Phone Number";
            Questions[8].Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Text Area Test With Phone Number is not a valid phone number.");
            #endregion Assert
        }
        #endregion Phone Number Validator Tests

        #endregion Text Area Tests

        #endregion Checkout Transaction Answer Tests continued
        #endregion Checkout Post Tests (Part 4)
    }
}
