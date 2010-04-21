using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    /// <summary>
    /// Entity Name:  PaymentLog
    /// LookupFieldName: Name
    /// </summary>
    [TestClass]
    public class PaymentLogRepositoryTests : AbstractRepositoryTests<PaymentLog, int>
    {
        /// <summary>
        /// Gets or sets the PaymentLog repository.
        /// </summary>
        /// <value>The PaymentLog repository.</value>
        public IRepository<PaymentLog> PaymentLogRepository { get; set; }
  
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentLogRepositoryTests"/> class.
        /// </summary>
        public PaymentLogRepositoryTests()
        {
            PaymentLogRepository = new Repository<PaymentLog>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override PaymentLog GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.PaymentLog(counter);
            rtValue.Transaction = Repository.OfType<Transaction>().GetNullableByID(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<PaymentLog> GetQuery(int numberAtEnd)
        {
            return PaymentLogRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(PaymentLog entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(PaymentLog entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            LoadTransactions(1);

            PaymentLogRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            PaymentLogRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides  

        #region Name Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Name = "x".RepeatTimes(201);
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                Assert.AreEqual(201, paymentLog.Name.Length);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests        

        #region Valid Tests

        /// <summary>
        /// Tests the name with null value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithNullValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Name = null;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the name with empty string and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithEmptyStringAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Name = string.Empty;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Name = "x";
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, paymentLog.Name.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion Name Tests

        #region Amount Tests

        #region Valid Tests
        /// <summary>
        /// Tests the amount with zero value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithZeroValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Amount = 0;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the amount with negative value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithNegativeValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Amount = -1;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the amount with less than A penny value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithLessThanAPennyValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Amount = 0.009m;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the amount with penny value saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithPennyValueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Amount = 0.01m;
            paymentLog.Credit = false;

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the amount with large value saves.
        /// </summary>
        [TestMethod]
        public void TestAmountWithLargeValueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Amount = 999999999999m;
            paymentLog.Credit = false;

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Amount Tests

        #region DatePayment Tests

        #region Valid Tests

        /// <summary>
        /// Tests the date payment with past date saves.
        /// </summary>
        [TestMethod]
        public void TestDatePaymentWithPastDateSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.DatePayment = DateTime.Now.AddDays(-1);

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the date payment with current date saves.
        /// </summary>
        [TestMethod]
        public void TestDatePaymentWithCurrentDateSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.DatePayment = DateTime.Now;

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the date payment with future date saves.
        /// </summary>
        [TestMethod]
        public void TestDatePaymentWithFutureDateSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.DatePayment = DateTime.Now.AddDays(1);

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion DatePayment Tests

        #region Transaction Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the transaction with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionWithNullValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Transaction = null;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Transaction: may not be empty");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }	
        }

        /// <summary>
        /// Tests the transaction with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTransactionWithNewValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Transaction = new Transaction();
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Transaction, Entity: CRP.Core.Domain.Transaction", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transaction with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadTransactions(3);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();
            var paymentLog = GetValid(9);
            var transaction = Repository.OfType<Transaction>().GetNullableByID(3);
            transaction.AddPaymentLog(paymentLog);
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Test

        /// <summary>
        /// Tests the delete payment log does not cascade to transaction.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public void TestDeletePaymentLogDoesNotCascadeToTransaction()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange

                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                LoadTransactions(3);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();
                paymentLog = GetValid(9);
                var transaction = Repository.OfType<Transaction>().GetNullableByID(3);
                transaction.AddPaymentLog(paymentLog);
                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();

                Assert.AreEqual(4, Repository.OfType<Transaction>().GetAll().Count); //because we load 1 in init
                Assert.AreSame(paymentLog, Repository.OfType<Transaction>().GetNullableByID(3).PaymentLogs.ElementAt(0));

                #endregion Arrange

                #region Act

                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.Remove(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();

                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                Assert.IsNotNull(ex);
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.PaymentLog#6]", ex.Message);
                Assert.AreEqual(4, Repository.OfType<Transaction>().GetAll().Count);
                #endregion Assert

                throw;
            }	
        }
        #endregion CRUD Test

        #endregion Transaction Tests

        #region CheckNumber Tests
        #region Valid Tests

        /// <summary>
        /// Tests the check number with null value and credit and not accepted saves.
        /// </summary>
        [TestMethod] 
        public void TestCheckNumberWithNullValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CheckNumber = null;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the check number with negative value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCheckNumberWithNegativeValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CheckNumber = -1;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the check number with zero value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCheckNumberWithZeroValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CheckNumber = 0;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the check number with large value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCheckNumberWithLargeValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CheckNumber = int.MaxValue;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion CheckNumber Tests

        #region GatewayTransactionId Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the gateway transaction id with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestGatewayTransactionIdWithTooLongValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.GatewayTransactionId = "x".RepeatTimes(17);
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                Assert.AreEqual(17, paymentLog.GatewayTransactionId.Length);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("GatewayTransactionId: length must be between 0 and 16");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }        
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the gateway transaction id with null value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestGatewayTransactionIdWithNullValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.GatewayTransactionId = null;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the gateway transaction id with empty string and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestGatewayTransactionIdWithEmptyStringAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.GatewayTransactionId = string.Empty;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the gateway transaction id with spaces only and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestGatewayTransactionIdWithSpacesOnlyAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.GatewayTransactionId = " ";
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the gateway transaction id with long value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestGatewayTransactionIdWithLongValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.GatewayTransactionId = "x".RepeatTimes(16);
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(16, paymentLog.GatewayTransactionId.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the gateway transaction id with one character and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestGatewayTransactionIdWithOneCharacterAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.GatewayTransactionId = "x";
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, paymentLog.GatewayTransactionId.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion GatewayTransactionId Tests
        #region CardType Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the CardType with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCardTypeWithTooLongValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.CardType = "x".RepeatTimes(21);
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                Assert.AreEqual(21, paymentLog.CardType.Length);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CardType: length must be between 0 and 20");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the CardType with null value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCardTypeWithNullValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CardType = null;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CardType with empty string and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCardTypeWithEmptyStringAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CardType = string.Empty;
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the CardType with spaces only and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCardTypeWithSpacesOnlyAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CardType = " ";
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the CardType with long value and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCardTypeWithLongValueAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CardType = "x".RepeatTimes(20);
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(20, paymentLog.CardType.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the card type with one character and credit and not accepted saves.
        /// </summary>
        [TestMethod]
        public void TestCardTypeWithOneCharacterAndCreditAndNotAcceptedSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.CardType = "x";
            paymentLog.Credit = true;
            paymentLog.Check = !paymentLog.Credit;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, paymentLog.CardType.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion CardType Tests

        #region Accepted Tests

        /// <summary>
        /// Tests the Accepted is false saves.
        /// </summary>
        [TestMethod]
        public void TestAcceptedIsFalseSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Accepted is true saves.
        /// </summary>
        [TestMethod]
        public void TestAcceptedIsTrueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Accepted = true;
            paymentLog.Credit = false;
            paymentLog.CheckNumber = 1;
            paymentLog.Amount = 1;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        #endregion Accepted Tests
        #region Check Tests

        /// <summary>
        /// Tests the Check is false saves.
        /// </summary>
        [TestMethod]
        public void TestCheckIsFalseSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Check = false;
            paymentLog.Credit = true;
            paymentLog.Accepted = false;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Check is true saves.
        /// </summary>
        [TestMethod]
        public void TestCheckIsTrueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Check = true;
            paymentLog.Credit = false;
            paymentLog.CheckNumber = 1;
            paymentLog.Amount = 1;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        #endregion Check Tests
        #region Credit Tests
        /// <summary>
        /// Tests the Credit is false saves.
        /// </summary>
        [TestMethod]
        public void TestCreditIsFalseSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Credit = false;
            paymentLog.Check = !paymentLog.Credit;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Credit is true saves.
        /// </summary>
        [TestMethod]
        public void TestCreditIsTrueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Credit = true;
            paymentLog.Check = false;
            paymentLog.Accepted = false;
            paymentLog.CheckNumber = 1;
            paymentLog.Amount = 1;
            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        #endregion Check Tests

        #region Notes Tests
        #region Valid Tests
        /// <summary>
        /// Tests the notes with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithNullValueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Notes = null;

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the notes with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithEmptyStringSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Notes = string.Empty;

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the notes with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithSpacesOnlySaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Notes = " ";

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the notes with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithOneCharacterSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Notes = "x";

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the notes with large value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithLargeValueSaves()
        {
            #region Arrange
            var paymentLog = GetValid(9);
            paymentLog.Notes = "1234567890".RepeatTimes(100);

            #endregion Arrange

            #region Act
            PaymentLogRepository.DbContext.BeginTransaction();
            PaymentLogRepository.EnsurePersistent(paymentLog);
            PaymentLogRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, paymentLog.Notes.Length);
            Assert.IsFalse(paymentLog.IsTransient());
            Assert.IsTrue(paymentLog.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Notes Tests

        #region CheckNumberRequired Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the check number required when check and check number null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckNumberRequiredWhenCheckAndCheckNumberNullDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.CheckNumber = null;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CheckNumberRequired: Check number required when credit card not used.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the check number required when check and check number negative does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckNumberRequiredWhenCheckAndCheckNumberNegativeDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.CheckNumber = -1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CheckNumberRequired: Check number required when credit card not used.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the check number required when check and check number zero does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckNumberRequiredWhenCheckAndCheckNumberZeroDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.CheckNumber = 0;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CheckNumberRequired: Check number required when credit card not used.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #endregion CheckNumberRequired Tests

        #region NameRequired Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the name required when check and name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCheckAndNameWithNullValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Name = null;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the name required when check and name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCheckAndNameWithEmptyStringDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Name = string.Empty;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the name required when check and name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCheckAndNameWithSpacesOnlyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Name = " ";
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the name required when check and name with null value and accepted is false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCheckAndNameWithNullValueAndAcceptedIsFalseDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Accepted = false;
                paymentLog.Name = null;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the name required when credit and accepted and name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCreditAndAcceptedAndNameWithNullValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Name = null;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the name required when credit and accepted and name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCreditAndAcceptedAndNameWithEmptyStringDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Name = string.Empty;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the name required when credit and accepted and name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameRequiredWhenCreditAndAcceptedAndNameWithSpacesOnlyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Name = " ";
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("NameRequired: Payee name required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        
        #endregion Invalid Tests
        #endregion NameRequired Tests

        #region AmountRequired Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the amount required when credit and accepted and amount is less than one penny does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCreditAndAcceptedAndAmountIsLessThanOnePennyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Amount = 0.009m;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the amount required when credit and accepted and amount is zero does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCreditAndAcceptedAndAmountIsZeroDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Amount = 0;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the amount required when credit and accepted and amount is negative does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCreditAndAcceptedAndAmountIsNegativeDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = "Visa";
                paymentLog.Amount = -1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the amount required when check and amount is zero does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCheckAndAmountIsZeroDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Accepted = false;
                paymentLog.Amount = 0;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the amount required when check and amount is less than one penny does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCheckAndAmountIsLessThanOnePennyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Accepted = false;
                paymentLog.Amount = 0.009m;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the amount required when check and amount is negative does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAmountRequiredWhenCheckAndAmountIsNegativeDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = false;
                paymentLog.Accepted = false;
                paymentLog.Amount = -1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AmountRequired: Amount must be more than 1 cent.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests
        #endregion AmountRequired Tests

        #region GatewayTransactionIdRequired Tests
        /// <summary>
        /// Tests the gateway transaction id required when credit and accepted and gateway transaction id has null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestGatewayTransactionIdRequiredWhenCreditAndAcceptedAndGatewayTransactionIdHasNullValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = null;
                paymentLog.CardType = "Visa";
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("GatewayTransactionIdRequired: Gateway Transaction Id Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the gateway transaction id required when credit and accepted and gateway transaction id has empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestGatewayTransactionIdRequiredWhenCreditAndAcceptedAndGatewayTransactionIdHasEmptyStringDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = string.Empty;
                paymentLog.CardType = "Visa";
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("GatewayTransactionIdRequired: Gateway Transaction Id Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the gateway transaction id required when credit and accepted and gateway transaction id has spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestGatewayTransactionIdRequiredWhenCreditAndAcceptedAndGatewayTransactionIdHasSpacesOnlyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = " ";
                paymentLog.CardType = "Visa";
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("GatewayTransactionIdRequired: Gateway Transaction Id Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion GatewayTransactionIdRequired Tests

        #region CardTypeRequired Tests
        /// <summary>
        /// Tests the CardTypeRequired when credit and accepted and gateway transaction id has null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCardTypeRequiredWhenCreditAndAcceptedAndCardTypeHasNullValueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = null;
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CardTypeRequired: Card Type Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the card type required when credit and accepted and card type has empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCardTypeRequiredWhenCreditAndAcceptedAndCardTypeHasEmptyStringDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = string.Empty;
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CardTypeRequired: Card Type Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the card type required when credit and accepted and card type has spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCardTypeRequiredWhenCreditAndAcceptedAndCardTypeHasSpacesOnlyDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = true;
                paymentLog.Accepted = true;
                paymentLog.GatewayTransactionId = "1";
                paymentLog.CardType = " ";
                paymentLog.Amount = 1;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CardTypeRequired: Card Type Required.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion CardTypeRequired Tests

        #region CheckAndCredit Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the check or credit when credit and check are false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckOrCreditWhenCreditAndCheckAreFalseDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = false;
                paymentLog.Credit = false;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CheckOrCredit: Check or Credit must be selected.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the check or credit when credit and check are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckOrCreditWhenCreditAndCheckAreTrueDoesNotSave()
        {
            PaymentLog paymentLog = null;
            try
            {
                #region Arrange
                paymentLog = GetValid(9);
                paymentLog.Check = true;
                paymentLog.Credit = true;
                #endregion Arrange

                #region Act
                PaymentLogRepository.DbContext.BeginTransaction();
                PaymentLogRepository.EnsurePersistent(paymentLog);
                PaymentLogRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(paymentLog);
                var results = paymentLog.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CheckOrCredit: Check or Credit must be selected.");
                Assert.IsFalse(paymentLog.IsValid());
                Assert.IsTrue(paymentLog.IsTransient());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #endregion CheckAndCredit Tests


        #region Reflection of Database

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange

            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Accepted", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Amount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("AmountRequired", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Amount must be more than 1 cent.\")]"
            }));
            expectedFields.Add(new NameAndType("CardType", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)20)]"
            }));
            expectedFields.Add(new NameAndType("CardTypeRequired", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Card Type Required.\")]"
            }));
            expectedFields.Add(new NameAndType("Check", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("CheckNumber", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("CheckNumberRequired", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Check number required when credit card not used.\")]"
            }));
            expectedFields.Add(new NameAndType("CheckOrCredit", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Check or Credit must be selected.\")]"
            }));
            expectedFields.Add(new NameAndType("Credit", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("DatePayment", "System.DateTime", new List<string>()));            
            expectedFields.Add(new NameAndType("GatewayTransactionId", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)16)]"
            }));
            expectedFields.Add(new NameAndType("GatewayTransactionIdRequired", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Gateway Transaction Id Required.\")]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("NameRequired", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Payee name required.\")]"
            }));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));            
            expectedFields.Add(new NameAndType("Transaction", "CRP.Core.Domain.Transaction", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(PaymentLog));

        }
        #endregion reflection
    }
}