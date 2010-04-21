﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    [TestClass]
    public class TransactionControllerTests : ControllerTestBase<TransactionController>
    {
        private readonly Type _controllerClass = typeof(TransactionController);
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }
        public INotificationProvider NotificationProvider { get; set; }
        protected List<Transaction> Transactions { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<DisplayProfile> DisplayProfiles { get; set; }
        protected IRepository<DisplayProfile> DisplayProfileRepository { get; set; }
        protected List<OpenIdUser> OpenIdUsers { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<Question> Questions { get; set; }
        protected IRepository<Question> QuestionRepository { get; set; }

        #region Init
        public TransactionControllerTests()
        {
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
            Controller.Repository.Expect(a => a.OfType<Transaction>()).Return(TransactionRepository).Repeat.Any();
            
            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            DisplayProfiles = new List<DisplayProfile>();
            DisplayProfileRepository = FakeRepository<DisplayProfile>();
            Controller.Repository.Expect(a => a.OfType<DisplayProfile>()).Return(DisplayProfileRepository).Repeat.Any();

            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();
        
            Questions = new List<Question>();
            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();
        
            OpenIdUsers = new List<OpenIdUser>();
            //OpenIdUserRepository = FakeRepository<OpenIdUser>();
            //Controller.Repository.Expect(a => a.OfType<OpenIdUser>()).Return(OpenIdUserRepository).Repeat.Any();
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            OpenIdUserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OpenIdUser, string>>();
            NotificationProvider = MockRepository.GenerateStub<INotificationProvider>();
            Controller = new TestControllerBuilder().CreateController<TransactionController>(OpenIdUserRepository, NotificationProvider);
        }

        #endregion Init

        #region Route Tests


        /// <summary>
        /// Tests the checkout get mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5));
        }

        /// <summary>
        /// Tests the checkout post mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5,1, null, "test", string.Empty, string.Empty , new QuestionAnswerParameter[1],new QuestionAnswerParameter[1], true ), true);
        }

        /// <summary>
        /// Tests the confirmation get mapping.
        /// </summary>
        [TestMethod]
        public void TestConfirmationGetMapping()
        {
            "~/Transaction/Confirmation/5".ShouldMapTo<TransactionController>(a => a.Confirmation(5));
        }

        /// <summary>
        /// Tests the payment success mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentSuccessMapping()
        {
            "~/Transaction/PaymentSuccess/5".ShouldMapTo<TransactionController>(a => a.PaymentSuccess("12","12"), true);
        }

        /// <summary>
        /// Tests the payment error mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentErrorMapping()
        {
            "~/Transaction/PaymentError/5".ShouldMapTo<TransactionController>(a => a.PaymentError("12", "12"), true);
        }

        /// <summary>
        /// Tests the payment cancel mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentCancelMapping()
        {
            "~/Transaction/PaymentCancel/5".ShouldMapTo<TransactionController>(a => a.PaymentCancel("12", "12"), true);
        }

        /// <summary>
        /// Tests the edit get mapping.
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Transaction/Edit/5".ShouldMapTo<TransactionController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit post mapping.
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Transaction/Edit/".ShouldMapTo<TransactionController>(a => a.Edit(new Transaction()), true);
        }

        /// <summary>
        /// Tests the payment result mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentResultMapping()
        {
            "~/Transaction/PaymentResult/".ShouldMapTo<TransactionController>(a => a.PaymentResult(new PaymentResultParameters()), true);
        }

        /// <summary>
        /// Tests the lookup get mapping.
        /// </summary>
        [TestMethod]
        public void TestLookupGetMapping()
        {
            "~/Transaction/Lookup".ShouldMapTo<TransactionController>(a => a.Lookup());
        }

        /// <summary>
        /// Tests the lookup post mapping.
        /// </summary>
        [TestMethod]
        public void TestLookupPostMapping()
        {
            "~/Transaction/Lookup/test".ShouldMapTo<TransactionController>(a => a.Lookup("order", "email"), true);
        }
        #endregion Route Tests

        #region Checkout Get Tests

        /// <summary>
        /// Tests the checkout get redirects to home controller if item is not found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetRedirectsToHomeControllerIfItemIsNotFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }
        
        /// <summary>
        /// Tests the checkout get returns view when item is found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetReturnsViewWhenItemIsFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Checkout(2)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the checkout get will populate contact information if open id found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetWillPopulateContactInformationIfOpenIdFound()
        {
            #region Arrange
            SetupDataForTests();
            SetupDataForPopulateItemTransactionAnswer();
            #endregion Arrange

            #region Act
            var result = Controller.Checkout(2)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.Answers.Count());
            Assert.AreEqual("FirstName", result.Answers.ElementAt(0).Answer);
            Assert.AreEqual("LastName", result.Answers.ElementAt(1).Answer);
            Assert.AreEqual("Address1", result.Answers.ElementAt(2).Answer);
            Assert.AreEqual("Address2", result.Answers.ElementAt(3).Answer);
            Assert.AreEqual("City", result.Answers.ElementAt(4).Answer);
            Assert.AreEqual("CA", result.Answers.ElementAt(5).Answer);
            Assert.AreEqual("95616", result.Answers.ElementAt(6).Answer);
            Assert.AreEqual("555 555 5555", result.Answers.ElementAt(7).Answer);
            Assert.AreEqual("Email@maily.org", result.Answers.ElementAt(8).Answer);
            #endregion Assert		
        }

        

        #endregion Checkout Get Tests

        #region Total Tests


        /// <summary>
        /// Tests the total to string format.
        /// Used for the MD5 hash for touchNet.
        /// </summary>
        [TestMethod]
        public void TestTotalToStringFormat()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 1);
            Transactions[0].Amount = 123456789.12m;
            #endregion Arrange

            #region Act
            var result = Transactions[0].Total.ToString();
            #endregion Act

            #region Assert
            Assert.AreEqual("123456789.12", result);
            #endregion Assert		
        }


        [TestMethod]
        public void TestMd5Calculation()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 1);
            Transactions[0].Amount = 12.35m;
            const string postingKey = "FB8E61EF5F63028C";
            const string transactionId = "A234";            
            #endregion Arrange

            #region Act
            var result = TransactionController.CalculateValidationString(
                postingKey, 
                transactionId,
                Transactions[0].Total.ToString());
            #endregion Act

            #region Assert
            Assert.AreEqual("hAGcy7esDK7joiFIPJQKRA==", result);
            #endregion Assert		
        }

        #endregion Total Tests


        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from super controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromSuperController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only two attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyTwoAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        #endregion Controller Class Tests

        #region Controller Method Tests

        /// <summary>
        /// Tests the controller contains expected number of public methods.
        /// </summary>
        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(12, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method checkout get contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method checkout post contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method checkout post contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<CaptchaValidatorAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "CaptchaValidatorAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method confirmation contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodConfirmationContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Confirmation");
            #endregion Arrange

            #region Act           
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the controller method payment success contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentSuccessContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentSuccess");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment cancel contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentCancelContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentCancel");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment error contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentErrorContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentError");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method calculate validation string contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCalculateValidationStringContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("CalculateValidationString");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit get contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AnyoneWithRoleAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit post contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AnyoneWithRoleAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit post contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment result contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentResultContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentResult");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment result contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentResultContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentResult");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<BypassAntiForgeryTokenAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "BypassAntiForgeryTokenAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method lookup get contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLookupGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Lookup");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method lookup post contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLookupPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Lookup");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests

        #region Helper Methods

        /// <summary>
        /// Setups the data for tests.
        /// </summary>
        private void SetupDataForTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeUnits(Units, 1);
            ControllerRecordFakes.FakeDisplayProfile(DisplayProfiles, 3);
            Items[1].Unit = Units[0];
            DisplayProfiles[1].Unit = Units[0];

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Setups the data for populate item transaction answer.
        /// Needs Items populated
        /// </summary>
        private void SetupDataForPopulateItemTransactionAnswer()
        {
            ControllerRecordFakes.FakeOpenIdUsers(OpenIdUsers, 3);
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 3);
            ControllerRecordFakes.FakeQuestions(Questions, 10);
            LoadContactInfoQuestions();
            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;
            QuestionSets[0].SystemReusable = true;
            for (int i = 0; i < 9; i++)
            {
                QuestionSets[0].AddQuestion(Questions[i]);
            }
            Items[1].AddTransactionQuestionSet(QuestionSets[0]);
            
            OpenIdUsers[1].SetIdTo("UserName");
            OpenIdUsers[1].Address2 = "Address2";
            OpenIdUsers[1].City = "City";
            OpenIdUsers[1].Email = "Email@maily.org";
            OpenIdUsers[1].FirstName = "FirstName";
            OpenIdUsers[1].LastName = "LastName";
            OpenIdUsers[1].PhoneNumber = "555 555 5555";
            OpenIdUsers[1].State = "CA";
            OpenIdUsers[1].StreetAddress = "Address1";
            OpenIdUsers[1].Zip = "95616";

            OpenIdUserRepository.Expect(a => a.GetNullableByID("UserName")).Return(OpenIdUsers[1]).Repeat.Any();
        }

        private void LoadContactInfoQuestions()
        {
            Questions[0].Name = StaticValues.Question_FirstName;
            Questions[1].Name = StaticValues.Question_LastName;
            Questions[2].Name = StaticValues.Question_StreetAddress;
            Questions[3].Name = StaticValues.Question_AddressLine2;
            Questions[4].Name = StaticValues.Question_City;
            Questions[5].Name = StaticValues.Question_State;
            Questions[6].Name = StaticValues.Question_Zip;
            Questions[7].Name = StaticValues.Question_PhoneNumber;
            Questions[8].Name = StaticValues.Question_Email;
        }

        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;
            public bool RoleReturnValue { get; set; }
            public string[] UserRoles { get; set; }

            public MockPrincipal(string[] userRoles)
            {
                UserRoles = userRoles;
            }

            public IIdentity Identity
            {
                get { return _identity ?? (_identity = new MockIdentity()); }
            }

            public bool IsInRole(string role)
            {
                if (UserRoles.Contains(role))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Mock the HTTPContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private readonly int _count;
            public string[] UserRoles { get; set; }
            public MockHttpContext(int count, string[] userRoles)
            {
                _count = count;
                UserRoles = userRoles;
            }

            public override IPrincipal User
            {
                get { return _user ?? (_user = new MockPrincipal(UserRoles)); }
                set
                {
                    _user = value;
                }
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return new MockHttpRequest(_count);
                }
            }
        }

        public class MockHttpRequest : HttpRequestBase
        {
            MockHttpFileCollectionBase Mocked { get; set; }

            public MockHttpRequest(int count)
            {
                Mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return Mocked;
                }
            }
        }

        public class MockHttpFileCollectionBase : HttpFileCollectionBase
        {
            public int Counter { get; set; }

            public MockHttpFileCollectionBase(int count)
            {
                Counter = count;
                for (int i = 0; i < count; i++)
                {
                    BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
                }

            }

            public override int Count
            {
                get
                {
                    return Counter;
                }
            }
            public override HttpPostedFileBase Get(string name)
            {
                return new MockHttpPostedFileBase();
            }
            public override HttpPostedFileBase this[string name]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
            public override HttpPostedFileBase this[int index]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase
        {
            public override int ContentLength
            {
                get
                {
                    return 5;
                }
            }
            public override string FileName
            {
                get
                {
                    return "Mocked File Name";
                }
            }
            public override Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                    return memStream;
                }
            }
        }

        #endregion

        #endregion Helper Methods
        
    }
}
