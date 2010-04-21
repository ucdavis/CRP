using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
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
    [TestClass]
    public class CheckControllerTests : ControllerTestBase<CheckController>
    {
        protected List<Transaction> Transactions { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }


        #region Init

        public CheckControllerTests()
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
            Controller = new TestControllerBuilder().CreateController<CheckController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the link to transaction mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionMappingIgnoreParameters()
        {
            "~/Check/LinkToTransaction/5".ShouldMapTo<CheckController>(a => a.LinkToTransaction(5), true);
        }

        /// <summary>
        /// Tests the link to transaction mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionMappingIgnoreParameters2()
        {
            "~/Check/LinkToTransaction/5".ShouldMapTo<CheckController>(a => a.LinkToTransaction(5, new Check[2]), true);
        }
        

        #endregion Route Tests

        #region LinkToTransaction Tests

        /// <summary>
        /// Tests the link to transaction with only transaction id redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithOnlyTransactionIdRedirectsToListWhenIdNotFound()
        {
            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.LinkToTransaction(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the link to transaction with only transaction id returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionWithOnlyTransactionIdReturnsViewWhenIdFound()
        {
            FakeTransactions(2);
            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(Transactions[0]).Repeat.Any();
            var result = Controller.LinkToTransaction(1)
                .AssertViewRendered()
                .WithViewData<LinkCheckViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual("PaymentConfirmation1", result.Transaction.PaymentConfirmation);
        }

        /// <summary>
        /// Tests the link to transaction redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToTransactionRedirectsToListWhenIdNotFound()
        {
            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.LinkToTransaction(1, new Check[1])
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
        }


        [TestMethod, Ignore]
        public void TestLinkToTransactionWhenIdFoundSaves()
        {
            var checks = new Check[2];
            checks[0] = CreateValidEntities.Check(1);
            checks[1] = CreateValidEntities.Check(2);
            checks[0].Amount = (decimal)10.00;
            checks[1].Amount = (decimal)9.49;

            FakeTransactions(2);
            Transactions[0].Item = CreateValidEntities.Item(1);
            Transactions[0].Item.SetIdTo(1);    
            TransactionRepository.Expect(a => a.GetNullableByID(1)).Return(Transactions[0]).Repeat.Any();

            Controller.LinkToTransaction(1, checks)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Details(1));


        }
        

        #endregion LinkToTransaction Tests

        #region Helper Methods

        private void FakeTransactions(int count)
        {
            var offSet = Transactions.Count;
            for (int i = 0; i < count; i++)
            {
                Transactions.Add(CreateValidEntities.Transaction(i + 1 + offSet));
                Transactions[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #endregion Helper Methods
    }
}
