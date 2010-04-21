using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Donation Tests

        /// <summary>
        /// Tests the Donation is false saves.
        /// </summary>
        [TestMethod]
        public void TestDonationIsFalseSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Donation = false;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(transaction.Donation);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Donation is true saves.
        /// </summary>
        [TestMethod]
        public void TestDonationIsTrueSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Donation = true;
            transaction.Amount = 1m;
            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(transaction.Donation);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        #endregion Donation Tests

        #region Quantity Tests

        /// <summary>
        /// Tests the Quantity of zero saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityOfZeroSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Quantity = 0;
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
        /// Tests the Quantity with minimum value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWithMinimumValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Quantity = int.MinValue;
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
        /// Tests the Quantity with maximum value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWithMaximumValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Quantity = int.MaxValue;
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
        /// Tests the Quantity with negative one saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWithNegativeOneSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Quantity = -1;
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
        #endregion Quantity Tests

        #region ParentTransaction Tests

        /// <summary>
        /// Tests the parent transaction with null value saves.
        /// </summary>
        [TestMethod]
        public void TestParentTransactionWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.ParentTransaction = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(transaction.ParentTransaction);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the parent transaction with A valid non null value saves.
        /// </summary>
        [TestMethod]
        public void TestParentTransactionWithAValidNonNullValueSaves()
        {
            #region Arrange
            //var count = TransactionRepository.GetAll().Count;
            var transaction = GetValid(9);
            var childTransaction = GetValid(8);
            childTransaction.Donation = true;
            childTransaction.Amount = 10.0m;
            transaction.AddChildTransaction(childTransaction);
            //Assert.AreEqual(count, TransactionRepository.GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            //Assert.AreEqual(count + 2, TransactionRepository.GetAll().Count);
            Assert.AreSame(transaction, childTransaction.ParentTransaction);
            #endregion Assert
        }

        #endregion ParentTransaction Tests
    }
}
