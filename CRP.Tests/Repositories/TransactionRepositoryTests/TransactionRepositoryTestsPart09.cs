using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region ChildTransactions Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the child transactions with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestChildTransactionsWithNullValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.ChildTransactions = null;
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
                results.AssertErrorsAre("ChildTransactions: may not be null");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestChildTransactionsWithEmptyListSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.ChildTransactions = new List<Transaction>();
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, transaction.ChildTransactions.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the child transactions with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestChildTransactionsWithValidValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            transaction.AddChildTransaction(donationTransaction);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Count());
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the child transactions with two valid value saves.
        /// </summary>
        [TestMethod]
        public void TestChildTransactionsWithTwoValidValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Count());
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Tests

        /// <summary>
        /// Tests the child transactions are cascaded with save.
        /// </summary>
        [TestMethod]
        public void TestChildTransactionsAreCascadedWithSave()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 5m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            Assert.AreEqual(5, TransactionRepository.GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(25.0m, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Sum(s => s.Amount));
            Assert.AreEqual(8, TransactionRepository.GetAll().Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the child transactions with remove does not actually remove value from database.
        /// </summary>
        [TestMethod]
        public void TestChildTransactionsWithRemoveDoesNotActuallyRemoveValueFromDatabase()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 5m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            Assert.AreEqual(25.0m, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Sum(s => s.Amount));
            Assert.AreEqual(8, TransactionRepository.GetAll().Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Arrange

            #region Act
            transaction.ChildTransactions.Remove(donationTransaction);
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(25.0m, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Sum(s => s.Amount));
            Assert.AreEqual(8, TransactionRepository.GetAll().Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion ChildTransactions Tests
    }
}
