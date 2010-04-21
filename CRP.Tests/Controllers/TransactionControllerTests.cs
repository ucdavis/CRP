using System.Collections.Generic;
using System.Configuration;
using CRP.Controllers;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    [TestClass]
    public class TransactionControllerTests : ControllerTestBase<TransactionController>
    {
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }
        public INotificationProvider NotificationProvider { get; set; }
        protected List<Transaction> Transactions { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }

        
        #region Init
        public TransactionControllerTests()
        {
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
            Controller.Repository.Expect(a => a.OfType<Transaction>()).Return(TransactionRepository).Repeat.Any();
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
        /// Tests the index mapping.
        /// Alan: Modified to remove index for now. 2/24/2010
        /// </summary>
        //[TestMethod]
        //public void TestIndexMapping()
        //{
        //    "~/Transaction/Index/Test".ShouldMapTo<TransactionController>(a => a.Index());
        //}

        /// <summary>
        /// Tests the checkout mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5));
        }

        /// <summary>
        /// Tests the checkout with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithParametersMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5,1, null, "test", string.Empty, string.Empty , new QuestionAnswerParameter[1],new QuestionAnswerParameter[1], true ), true);
        }

        #endregion Route Tests

        #region Total Tests


        /// <summary>
        /// Tests the total to string format.
        /// Used for the MD5 hash for touchNet.
        /// </summary>
        [TestMethod]
        public void TestTotalToStringFormat()
        {
            #region Arrange
            FakeTransactions(1);
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
            FakeTransactions(1);
            Transactions[0].Amount = 12.35m;
            string postingKey = "FB8E61EF5F63028C";
            string transationId = "A234";            
            #endregion Arrange

            #region Act
            var result = TransactionController.CalculateValidationString(
                postingKey, 
                transationId,
                Transactions[0].Total.ToString());
            #endregion Act

            #region Assert
            Assert.AreEqual("hAGcy7esDK7joiFIPJQKRA==", result);
            #endregion Assert		
        }

        #endregion Total Tests

        /// <summary>
        /// Fakes the Transactions.
        /// Author: Sylvestre, Jason
        /// Create: 2010/03/17
        /// </summary>
        /// <param name="count">The number of Transactions to add.</param>
        private void FakeTransactions(int count)
        {
            var offSet = Transactions.Count;
            for (int i = 0; i < count; i++)
            {
            Transactions.Add(CreateValidEntities.Transaction(i + 1 + offSet));
            Transactions[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        
    }
}
