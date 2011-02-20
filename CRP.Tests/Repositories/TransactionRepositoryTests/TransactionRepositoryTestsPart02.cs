using System;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Item Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the item with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemWithNullValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Item = null;
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(transaction);
                var results = transaction.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be null");
                Assert.IsFalse(transaction.IsValid());
                Assert.IsTrue(transaction.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the item with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestItemWithNewValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Item = new Item();
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transaction);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Transaction.Item", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        /// <summary>
        /// Tests the item with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestItemWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadItems(3);
            Repository.OfType<Item>().DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.Item = Repository.OfType<Item>().GetNullableById(3);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreSame(Repository.OfType<Item>().GetNullableById(3), transaction.Item);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Tests

        /// <summary>
        /// Tests the delete transaction does not cascade to item.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionDoesNotCascadeToItem()
        {
            #region Arrange
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadItems(3);
            Repository.OfType<Item>().DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.Item = Repository.OfType<Item>().GetNullableById(3);

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(4, Repository.OfType<Item>().GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.Remove(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<Item>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion Item Tests

        #region TransactionDate Tests

        /// <summary>
        /// Tests the TransactionDate with past date saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionDateWithPastDateSaves()
        {
            #region Arrange

            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            Transaction transaction = GetValid(9);
            transaction.TransactionDate = fakeDate.AddDays(-10);

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.AreEqual(fakeDate.AddDays(-10), transaction.TransactionDate);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the TransactionDate is defaulted to the current date time.
        /// </summary>
        [TestMethod]
        public void TestTransactionDateIsDefaultedToTheCurrentDateTime()
        {
            #region Arrange

            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            var transaction = GetValid(9);

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.AreEqual(fakeDate, transaction.TransactionDate);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the TransactionDate with future date saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionDateWithFutureDateSaves()
        {
            #region Arrange

            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            var transaction = GetValid(9);
            transaction.TransactionDate = fakeDate.AddDays(10);

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.AreEqual(fakeDate.AddDays(10), transaction.TransactionDate);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        #endregion TransactionDate Tests
    }
}
