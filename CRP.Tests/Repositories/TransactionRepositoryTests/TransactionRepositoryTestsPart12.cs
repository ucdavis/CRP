using System.Collections.Generic;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Paid Tests
        /// <summary>
        /// Tests the paid is true.
        /// </summary>
        [TestMethod]
        public void TestPaidIsTrue1()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the paid is true2. (Changed how paid flag is determined)
        /// </summary>
        [TestMethod]
        public void TestPaidIsTrue2()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            //transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.AreEqual(11101, transaction.Total);
            Assert.AreEqual(11111, transaction.TotalPaid);
            Assert.IsTrue(result);
            #endregion Assert
        }
        [TestMethod]
        public void TestPaidIsFalse2()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            //transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the paid is false3.
        /// </summary>
        [TestMethod]
        public void TestPaidIsFalse3()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[0].Accepted = false;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        #endregion Paid Tests

        
    }
}
