using System;
using System.Collections.Generic;
using System.Threading;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;
using System.Linq;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Notified Tests

        /// <summary>
        /// Tests the Notified is false saves.
        /// </summary>
        [TestMethod]
        public void TestNotifiedIsFalseSaves()
        {
            #region Arrange

            Transaction transaction = GetValid(9);
            transaction.Notified = false;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(transaction.Notified);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Notified is true saves.
        /// </summary>
        [TestMethod]
        public void TestNotifiedIsTrueSaves()
        {
            #region Arrange

            var transaction = GetValid(9);
            transaction.Notified = true;

            #endregion Arrange

            #region Act

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(transaction.Notified);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());

            #endregion Assert
        }

        #endregion Notified Tests

        #region NotifiedDate Tests

        /// <summary>
        /// Tests the NotifiedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Transaction record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the NotifiedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the NotifiedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the notified date with null date date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithNullDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.NotifiedDate = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNull(record.NotifiedDate);
            #endregion Assert
        }
        #endregion NotifiedDate Tests
        
    }
}
