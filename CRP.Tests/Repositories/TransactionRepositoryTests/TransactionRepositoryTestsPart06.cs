using System;
using System.Collections.Generic;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region PaymentLogs Collection Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the payment logs with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPaymentLogsWithNullValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.PaymentLogs = null;
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
                results.AssertErrorsAre("PaymentLogs: may not be empty");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        /// <summary>
        /// Tests the payment logs with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestPaymentLogsWithEmptyListSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.PaymentLogs = new List<PaymentLog>();
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
            #endregion Assert
        }

        /// <summary>
        /// Tests the payment logs with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestPaymentLogsWithValidValueSaves()
        {
            #region Arrange
            var paymentLog = CreateValidEntities.PaymentLog(9);
            var transaction = GetValid(9);
            transaction.AddPaymentLog(paymentLog);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, transaction.PaymentLogs.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Tests

        /// <summary>
        /// Tests the payment logs with several values saves.
        /// </summary>
        [TestMethod]
        public void TestPaymentLogsWithSeveralValuesSaves()
        {
            #region Arrange
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
            }
            var transaction = GetValid(9);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[4]);
            Assert.AreEqual(0, Repository.OfType<PaymentLog>().GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(3, Repository.OfType<PaymentLog>().GetAll().Count);
            Assert.AreEqual(3, transaction.PaymentLogs.Count);
            foreach (var paymentLog in Repository.OfType<PaymentLog>().GetAll())
            {
                Assert.AreSame(transaction, paymentLog.Transaction);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the remove payment logs cascades to payment log.
        /// </summary>
        [TestMethod]
        public void TestRemovePaymentLogsCascadesToPaymentLog()
        {
            #region Arrange
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
            }
            var transaction = GetValid(9);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[4]);

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<PaymentLog>().GetAll().Count);
            #endregion Arrange

            #region Act
            transaction.RemovePaymentLog(paymentLogs[2]);
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(2, transaction.PaymentLogs.Count);
            Assert.AreEqual(2, Repository.OfType<PaymentLog>().GetAll().Count);
            #endregion Assert
        }
        /// <summary>
        /// Tests the remove payment logs cascades to payment log.
        /// </summary>
        [TestMethod]
        public void TestRemoveTransactionCascadesToPaymentLog()
        {
            #region Arrange
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
            }
            var transaction = GetValid(9);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[4]);

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<PaymentLog>().GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.Remove(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, Repository.OfType<PaymentLog>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion PaymentLogs Collection Tests
    }
}
