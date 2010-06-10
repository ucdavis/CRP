using System.Linq;
using CRP.Controllers.ViewModels;
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
        #region Lookup Get Tests

        [TestMethod]
        public void TestLookupGetReturnsViewModel()
        {
            Controller.Lookup()
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
        }
        

        #endregion Lookup Get Tests

        #region Lookup Post Tests               

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            Transactions[1].Amount = 200m;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();            
            #endregion Arrange

            #region Act
            var result = Controller.Lookup("99", "jasoncsylvestre@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("99", result.TransactionNumber);
            Assert.AreEqual("jasoncsylvestre@gmail.com", result.Email);
            Assert.AreEqual("Unable to locate order, please check your information and try again.", Controller.Message);
            Assert.IsFalse(result.ShowCreditCardReSubmit);
            #endregion Assert		
        }

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            AssignContactEmail(Transactions[1]);
            Transactions[1].Amount = 200m;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Lookup(Transactions[1].TransactionNumber, "jasoncsylvestre@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Transactions[1].TransactionNumber, result.TransactionNumber);
            Assert.AreEqual("jasoncsylvestre@gmail.com", result.Email);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.IsFalse(result.ShowCreditCardReSubmit);
            #endregion Assert
        }

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionFoundButDifferentEmail()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            AssignContactEmail(Transactions[1]);
            Transactions[1].Amount = 200m;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Lookup(Transactions[1].TransactionNumber, "NotMe@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Transaction);
            Assert.AreEqual("NotMe@gmail.com", result.Email);
            Assert.AreEqual(Transactions[1].TransactionNumber, result.TransactionNumber);
            Assert.AreEqual("Unable to locate order, please check your information and try again.", Controller.Message);
            Assert.IsFalse(result.ShowCreditCardReSubmit);
            #endregion Assert
        }

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionFound2()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            AssignContactEmail(Transactions[1]);
            Transactions[1].Credit = true;
            Transactions[1].Check = false;
            Transactions[1].Amount = 200m;

            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Lookup(Transactions[1].TransactionNumber, "jasoncsylvestre@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Transactions[1].TransactionNumber, result.TransactionNumber);
            Assert.AreEqual("jasoncsylvestre@gmail.com", result.Email);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.IsFalse(result.ShowCreditCardReSubmit);
            #endregion Assert
        }

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionFound3()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            AssignContactEmail(Transactions[1]);
            Transactions[1].Credit = true;
            Transactions[1].Check = false;
            Transactions[1].Amount = 200m;
            var payment = CreateValidEntities.PaymentLog(null);
            payment.Accepted = false;
            payment.Amount = 10m;
            payment.Credit = true;
            payment.TnStatus = "C";
            Transactions[1].AddPaymentLog(payment);
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Lookup(Transactions[1].TransactionNumber, "jasoncsylvestre@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Transactions[1].TransactionNumber, result.TransactionNumber);
            Assert.AreEqual("jasoncsylvestre@gmail.com", result.Email);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.IsTrue(result.ShowCreditCardReSubmit);
            #endregion Assert
        }

        [TestMethod]
        public void TestLookupPostReturnsViewWithExpectedInformationWhenTransactionFound4()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            AssignContactEmail(Transactions[1]);
            Transactions[1].Credit = true;
            Transactions[1].Check = false;
            Transactions[1].Amount = 200m;
            var payment = CreateValidEntities.PaymentLog(null);
            payment.Accepted = false;
            payment.Amount = 10m;
            payment.Credit = true;
            payment.TnStatus = "E";
            Transactions[1].AddPaymentLog(payment);
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Lookup(Transactions[1].TransactionNumber, "jasoncsylvestre@gmail.com")
                .AssertViewRendered()
                .WithViewData<LookupViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Transactions[1].TransactionNumber, result.TransactionNumber);
            Assert.AreEqual("jasoncsylvestre@gmail.com", result.Email);
            Assert.AreSame(Transactions[1], result.Transaction);
            Assert.IsTrue(result.ShowCreditCardReSubmit);
            #endregion Assert
        }

        #endregion Lookup Post Tests
    }
}
