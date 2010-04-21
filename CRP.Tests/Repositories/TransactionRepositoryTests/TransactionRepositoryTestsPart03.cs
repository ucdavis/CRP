using System;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Credit Tests

        /// <summary>
        /// Tests the Credit is false saves.
        /// </summary>
        [TestMethod]
        public void TestCreditIsFalseSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Credit = false;
            transaction.Check = !transaction.Credit;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(transaction.Credit);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Credit is true saves.
        /// </summary>
        [TestMethod]
        public void TestCreditIsTrueSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Credit = true;
            transaction.Check = !transaction.Credit;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(transaction.Credit);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        #endregion Credit Tests

        #region Check Tests

        /// <summary>
        /// Tests the Check is false saves.
        /// </summary>
        [TestMethod]
        public void TestCheckIsFalseSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Check = false;
            transaction.Credit = !transaction.Check;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(transaction.Check);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Check is true saves.
        /// </summary>
        [TestMethod]
        public void TestCheckIsTrueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Check = true;
            transaction.Credit = !transaction.Check;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(transaction.Check);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        #endregion Check Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.IsActive = false;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(transaction.IsActive);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.IsActive = true;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(transaction.IsActive);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        #endregion IsActive Tests

        #region Amount Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the amount with minimum value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountWithMinimumValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Amount = decimal.MinValue;
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
        /// Tests the amount with negative value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountWithNegativeValueDoesNotSave()
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
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the amount of zero saves.
        /// </summary>
        [TestMethod]
        public void TestAmountOfZeroSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 0;
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
        /// Tests the amount with maximum value saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithMaximumValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = decimal.MaxValue;
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
        #endregion Valid Tests
        #endregion Amount Tests
    }
}
