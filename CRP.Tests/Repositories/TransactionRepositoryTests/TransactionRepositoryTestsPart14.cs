using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

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
    }
}
