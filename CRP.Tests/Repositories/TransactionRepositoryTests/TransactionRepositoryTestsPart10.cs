using System.Collections.Generic;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region DonationTotal Tests

        [TestMethod]
        public void TestDonationTotalReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = 11.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.DonationTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(25, result);
            #endregion Assert
        }

        #endregion DonationTotal Tests

        #region AmountTotal Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestAmountTotalReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = 11.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.AmountTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(14, result);
            #endregion Assert
        }

        #endregion AmountTotal Tests

        #region Total Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = 11.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.Total;
            #endregion Act

            #region Assert
            Assert.AreEqual(39, result);
            #endregion Assert
        }

        #endregion Total Tests
        #region TotalPaid Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Amount = i + 1;
                paymentLogs[i].Accepted = false;
            }
            paymentLogs[2].Accepted = true;
            paymentLogs[4].Accepted = true;
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaid;
            #endregion Act

            #region Assert
            Assert.AreEqual(8, result);
            #endregion Assert
        }

        #endregion TotalPaid Tests
    }
}
