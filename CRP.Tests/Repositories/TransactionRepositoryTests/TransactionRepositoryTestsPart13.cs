using System;
using System.Linq;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region PaymentType Tests
        /// <summary>
        /// Tests the payment type with credit and check are false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPaymentTypeWithCreditAndCheckAreFalseDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Credit = false;
                transaction.Check = false;
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(transaction);
                var results = transaction.ValidationResults().AsMessageList();
                results.AssertErrorsAre("PaymentType: Payment type was not selected.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        /// <summary>
        /// Tests the payment type with credit and check are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPaymentTypeWithCreditAndCheckAreTrueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Credit = true;
                transaction.Check = true;
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(transaction);
                var results = transaction.ValidationResults().AsMessageList();
                results.AssertErrorsAre("PaymentType: Payment type was not selected.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }

        #endregion PaymentType Tests

        #region CorrectionAmount Tests
        [TestMethod]
        public void TestCorrectionAmountSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 1m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);

            var correction = new Transaction(transaction.Item);
            correction.Donation = false;
            correction.Amount = -15.00m;
            correction.CreatedBy = "Test";
            transaction.AddChildTransaction(correction);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Count());
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(-15m, transaction.CorrectionTotal);
            Assert.AreEqual(11m, transaction.Total);
            #endregion Assert
        }
        #endregion CorrectionAmount Tests

        #region CreatedBy Tests

        [TestMethod]
        public void TestCreatedByWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.IsNull(transaction.CreatedBy);
            #endregion Assert
        }
        [TestMethod]
        public void TestCreatedByWithEmptyValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = string.Empty;
            transaction.Amount = -1;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(string.Empty, transaction.CreatedBy);
            #endregion Assert
        }
        [TestMethod]
        public void TestCreatedByWithSpacesOnlySaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = " ";
            transaction.Amount = -1;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(" ", transaction.CreatedBy);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatedByWithFiftyCharactersSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = " ";
            transaction.Amount = -1;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(" ", transaction.CreatedBy);
            #endregion Assert
        }
        #endregion CreatedBy Tests
    }
}
