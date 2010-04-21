using System;
using System.Collections.Generic;
using System.Threading;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region CorrectionTotal Tests

        /// <summary>
        /// Tests the correction total sums all non donation amounts with non null created by field.
        /// </summary>
        [TestMethod]
        public void TestCorrectionTotalSumsAllNonDonationAmountsWithNonNullCreatedByField1()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = false;
            donationTransaction.Amount = -10.00m;
            donationTransaction.CreatedBy = "test";
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = -6.00m;
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.CorrectionTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(-16, result);
            #endregion Assert	
        }

        /// <summary>
        /// Tests the correction total sums all non donation amounts with non null created by field.
        /// </summary>
        [TestMethod]
        public void TestCorrectionTotalSumsAllNonDonationAmountsWithNonNullCreatedByField2()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = false;
            donationTransaction.Amount = -10.00m;
            donationTransaction.CreatedBy = null;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = -6.00m;
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.CorrectionTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(-6, result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the correction total sums all non donation amounts even positive ones with non null created by field.
        /// </summary>
        [TestMethod]
        public void TestCorrectionTotalSumsAllNonDonationAmountsEvenPositiveOnesWithNonNullCreatedByField()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = false;
            donationTransaction.Amount = 10.00m;
            donationTransaction.CreatedBy = "test";
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            var donationTransaction3 = new Transaction(transaction.Item);
            donationTransaction3.Donation = false;
            donationTransaction3.Amount = -6.00m;
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.CorrectionTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(4, result);
            #endregion Assert
        }
        #endregion CorrectionTotal Tests

        #region RegularAmount Tests

        /// <summary>
        /// Tests the regular amount when it is not A child transaction does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRegularAmountWhenItIsNotAChildTransactionDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Amount = -0.01m;
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
                results.AssertErrorsAre("RegularAmount: Amount must be zero or more.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }	
        }
        /// <summary>
        /// Tests the regular amount when it is A donation does not save.
        /// But caught by controller only
        /// </summary>
        [TestMethod]
        public void TestRegularAmountWhenItIsADonationDoesNotSave()
        {
            #region Arrange

            var transaction = GetValid(9);
            var validDonation = new Transaction(transaction.Item);
            validDonation.Donation = true;
            validDonation.Amount = 10.00m;
            transaction.AddChildTransaction(validDonation);
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
  
            #endregion Arrange
   
            #region Arrange
            var donation = new Transaction(transaction.Item);
            donation.Donation = true;
            donation.Amount = -0.01m;
            transaction.AddChildTransaction(donation);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, transaction.PaymentLogs.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            var results = donation.ValidationResults().AsMessageList();
            results.AssertErrorsAre("RegularAmount: Amount must be zero or more.");
            #endregion Assert
        }
        #endregion RegularAmount Tests

        #region CorrectionAmount Tests
        /// <summary>
        /// Tests the correction amount with zero does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCorrectionAmountWithZeroDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.CreatedBy = "Test";
                transaction.Donation = false;
                transaction.Amount = 0m;
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
                results.AssertErrorsAre("CorrectionAmount: Amount must be less than zero.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the correction amount with positive value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCorrectionAmountWithPositiveValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.CreatedBy = "Test";
                transaction.Donation = false;
                transaction.Amount = 0.01m;
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
                results.AssertErrorsAre("CorrectionAmount: Amount must be less than zero.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }

        #region CorrectionTotalAmount Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCorrectionTotalAmountWithInvalidDataDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange

                transaction = GetValid(9);
                transaction.Amount = 20.00m;
                var donations = new List<Transaction>();
                for (int i = 0; i < 3; i++)
                {
                    donations.Add(new Transaction(transaction.Item));
                    donations[i].Amount = 5.00m;
                    donations[i].Donation = true;
                    donations[i].CreatedBy = "test";
                }
                foreach (var donation in donations)
                {
                    transaction.AddChildTransaction(donation);
                }
                var corrections = new List<Transaction>();
                for (int i = 0; i < 6; i++)
                {
                    donations.Add(new Transaction(transaction.Item));
                    donations[i].Amount = -3.00m;
                    donations[i].Donation = false;
                    donations[i].CreatedBy = "test";
                }
                foreach (var correction in corrections)
                {
                    transaction.AddChildTransaction(correction);
                }

                #endregion Arrange
            }
            catch(Exception)
            {
                Assert.Inconclusive("Test should not have failed here");
                throw new NotImplementedException();
            }
            try
            {
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
                results.AssertErrorsAre("CorrectionTotalAmount: The total of all correction amounts must not exceed the donation amounts");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }

        #endregion CorrectionTotalAmount Tests

        #endregion CorrectionAmount Tests
    }
}
