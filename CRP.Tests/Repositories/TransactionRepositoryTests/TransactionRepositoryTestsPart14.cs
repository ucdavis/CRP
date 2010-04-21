using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region CorrectionReason Tests
        #region Invalid Tests

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CorrectionReason with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCorrectionReasonWithOneCharacterSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CorrectionReason = "x";
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CorrectionReason with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCorrectionReasonWithLongValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CorrectionReason = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, transaction.CorrectionReason.Length);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the correction reason with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCorrectionReasonWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CorrectionReason = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(transaction.CorrectionReason);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the correction reason with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCorrectionReasonWithEmptyStringSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CorrectionReason = string.Empty;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Empty, transaction.CorrectionReason);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the correction reason with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestCorrectionReasonWithSpacesOnlySaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CorrectionReason = " ";
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(" ", transaction.CorrectionReason);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CorrectionReason Tests

        #region UncorrectedDonationTotal Tests
        /// <summary>
        /// Tests the uncorrected donation total with correction returns expected result.
        /// </summary>
        [TestMethod]
        public void TestUncorrectedDonationTotalWithCorrectionReturnsExpectedResult()
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
            donationTransaction3.Amount = -6.00m;
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.UncorrectedDonationTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(25, result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the uncorrected donation total with out correction returns expected result.
        /// </summary>
        [TestMethod]
        public void TestUncorrectedDonationTotalWithOutCorrectionReturnsExpectedResult()
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
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.UncorrectedDonationTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(25, result);
            #endregion Assert
        }


        /// <summary>
        /// Tests the uncorrected donation total with negative donation will sum it.
        /// </summary>
        [TestMethod]
        public void TestUncorrectedDonationTotalWithNegativeDonationWillSumIt()
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
            donationTransaction3.Donation = true;
            donationTransaction3.Amount = 11.00m; //This one really should never happen
            donationTransaction3.CreatedBy = "test";
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.UncorrectedDonationTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(36, result);
            #endregion Assert		
        }

        #endregion UncorrectedDonationTotal Tests
    }
}
