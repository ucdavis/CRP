using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class TransactionRepositoryTests : AbstractRepositoryTests<Transaction, int >
    {
        public IRepository<Transaction> TransactionRepository { get; set; }
        public IRepositoryWithTypedId<OpenIdUser, string> OpenIDUserRepository { get; set; }
      
        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepositoryTests"/> class.
        /// </summary>
        public TransactionRepositoryTests()
        {
            TransactionRepository = new Repository<Transaction>();
            OpenIDUserRepository = new RepositoryWithTypedId<OpenIdUser, string>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Transaction GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Transaction(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.Check = true;
                rtValue.Credit = false;
            }
            else
            {
                rtValue.Check = false;
                rtValue.Credit = true;
            }
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Transaction> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Transaction>().Queryable.Where(a => a.Check);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Transaction entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Transaction entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Check);
                    break;
                case ARTAction.Restore:
                    entity.Check = BoolRestoreValue;
                    entity.Credit = !entity.Check;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Check;
                    entity.Check = updateValue;
                    entity.Credit = !entity.Check;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Transaction Requires Item
        ///     Item requires Unit and ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

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
                results.AssertErrorsAre("Item: may not be empty");
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
            transaction.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreSame(Repository.OfType<Item>().GetNullableByID(3), transaction.Item);
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
            transaction.Item = Repository.OfType<Item>().GetNullableByID(3);

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

        #region OpenIDUser Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestOpenIDUserWithNewValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.OpenIDUser = new OpenIdUser();
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
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.OpenIdUser, Entity: CRP.Core.Domain.OpenIdUser", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
                
        /// <summary>
        /// Tests the open ID user with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.OpenIDUser = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert			
        }

        /// <summary>
        /// Tests the open ID user with existing value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserWithExistingValueSaves()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = OpenIDUserRepository.GetNullableByID("2");
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreSame(OpenIDUserRepository.GetNullableByID("2"),transaction.OpenIDUser);
            Assert.IsNotNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the open ID user does not cascade when saved.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserDoesNotCascadeWhenSaved()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = CreateValidEntities.OpenIdUser(9);
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            Assert.IsNotNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the delete transaction does not cascade to open id user.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionDoesNotCascadeToOpenIdUser()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = CreateValidEntities.OpenIdUser(9);
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
    
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.Remove(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            #endregion Assert		
        }

        #endregion CRUD Tests
        #endregion OpenIDUser Tests

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
                paymentLogs.Add(CreateValidEntities.PaymentLog(i+1));
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

        #region TransactionAnswers Collection Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionAnswersWithNullValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.TransactionAnswers = null;
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
                results.AssertErrorsAre("TransactionAnswers: may not be empty");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        /// <summary>
        /// Tests the transaction answers with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionAnswersWithEmptyListSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.TransactionAnswers = new List<TransactionAnswer>();
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, transaction.TransactionAnswers.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the transaction answers with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionAnswersWithValidValueSaves()
        {
            #region Arrange
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            var transactionAnswer = CreateValidEntities.TransactionAnswer(9);
            transactionAnswer.QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
            transactionAnswer.Question = Repository.OfType<Question>().GetNullableByID(1);
            var transaction = GetValid(9);
            transaction.AddTransactionAnswer(transactionAnswer);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, transaction.TransactionAnswers.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Tests
        /// <summary>
        /// Tests the transaction answers with several values saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionAnswersWithSeveralValuesSaves()
        {
            #region Arrange
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            var transactionAnswers = new List<TransactionAnswer>();
            for (int i = 0; i < 5; i++)
            {
                transactionAnswers.Add(CreateValidEntities.TransactionAnswer(i + 1));
                transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
            }
            var transaction = GetValid(9);
            transaction.AddTransactionAnswer(transactionAnswers[1]);
            transaction.AddTransactionAnswer(transactionAnswers[2]);
            transaction.AddTransactionAnswer(transactionAnswers[4]);
            Assert.AreEqual(0, Repository.OfType<TransactionAnswer>().GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(3, Repository.OfType<TransactionAnswer>().GetAll().Count);
            Assert.AreEqual(3, transaction.TransactionAnswers.Count);
            foreach (var ta in Repository.OfType<TransactionAnswer>().GetAll())
            {
                Assert.AreSame(transaction, ta.Transaction);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the remove transaction answers does not cascades to transaction answer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestRemoveTransactionAnswersDoesNotCascadesToTransactionAnswer()
        {
            try
            {
                #region Arrange

                LoadQuestionSets(1);
                LoadQuestionTypes(1);
                LoadQuestions(1);
                var transactionAnswers = new List<TransactionAnswer>();
                for (int i = 0; i < 5; i++)
                {
                    transactionAnswers.Add(CreateValidEntities.TransactionAnswer(i + 1));
                    transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                    transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
                }
                var transaction = GetValid(9);
                transaction.AddTransactionAnswer(transactionAnswers[1]);
                transaction.AddTransactionAnswer(transactionAnswers[2]);
                transaction.AddTransactionAnswer(transactionAnswers[4]);

                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                Assert.AreEqual(3, Repository.OfType<TransactionAnswer>().GetAll().Count);
                #endregion Arrange

                #region Act
                transaction.TransactionAnswers.Remove(transaction.TransactionAnswers.ElementAt(1));
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
                
            }
            catch(Exception ex)
            {
                #region Assert
                Assert.AreEqual("could not delete collection rows: [CRP.Core.Domain.Transaction.TransactionAnswers#6][SQL: UPDATE TransactionAnswers SET TransactionId = null WHERE TransactionId = @p0 AND id = @p1]", ex.Message);
                #endregion Assert
                throw;
            }
        }

        /// <summary>
        /// Tests the remove transaction does not cascades to transaction answer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestRemoveTransactionDoesNotCascadesToTransactionAnswer()
        {
            try
            {
                #region Arrange
                LoadQuestionSets(1);
                LoadQuestionTypes(1);
                LoadQuestions(1);
                var transactionAnswers = new List<TransactionAnswer>();
                for (int i = 0; i < 5; i++)
                {
                    transactionAnswers.Add(CreateValidEntities.TransactionAnswer(i + 1));
                    transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                    transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
                }
                var transaction = GetValid(9);
                transaction.AddTransactionAnswer(transactionAnswers[1]);
                transaction.AddTransactionAnswer(transactionAnswers[2]);
                transaction.AddTransactionAnswer(transactionAnswers[4]);

                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                Assert.AreEqual(3, Repository.OfType<TransactionAnswer>().GetAll().Count);
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.Remove(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act

            }
            catch (Exception ex)
            {
                #region Assert
                Assert.AreEqual("could not delete collection: [CRP.Core.Domain.Transaction.TransactionAnswers#6][SQL: UPDATE TransactionAnswers SET TransactionId = null WHERE TransactionId = @p0]", ex.Message);
                #endregion Assert
                throw;
            }
        }
        #endregion CRUD Tests
        #endregion TransactionAnswers Collection Tests

        #region QuantityAnswers Collection Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the quantity answers with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAnswersWithNullValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.QuantityAnswers = null;
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
                results.AssertErrorsAre("QuantityAnswers: may not be empty");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        /// <summary>
        /// Tests the quantity answers with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityAnswersWithEmptyListSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.QuantityAnswers = new List<QuantityAnswer>();
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, transaction.QuantityAnswers.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the quantity answers with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityAnswersWithValidValueSaves()
        {
            #region Arrange
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            var quantityAnswer = CreateValidEntities.QuantityAnswer(9);
            quantityAnswer.QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
            quantityAnswer.Question = Repository.OfType<Question>().GetNullableByID(1);
            var transaction = GetValid(9);
            transaction.AddQuantityAnswer(quantityAnswer);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, transaction.QuantityAnswers.Count);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Tests
        /// <summary>
        /// Tests the quantity answers with several values saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityAnswersWithSeveralValuesSaves()
        {
            #region Arrange
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            var quantityAnswers = new List<QuantityAnswer>();
            for (int i = 0; i < 5; i++)
            {
                quantityAnswers.Add(CreateValidEntities.QuantityAnswer(i + 1));
                quantityAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                quantityAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
            }
            var transaction = GetValid(9);
            transaction.AddQuantityAnswer(quantityAnswers[1]);
            transaction.AddQuantityAnswer(quantityAnswers[2]);
            transaction.AddQuantityAnswer(quantityAnswers[4]);
            Assert.AreEqual(0, Repository.OfType<QuantityAnswer>().GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(3, Repository.OfType<QuantityAnswer>().GetAll().Count);
            Assert.AreEqual(3, transaction.QuantityAnswers.Count);
            foreach (var ta in Repository.OfType<QuantityAnswer>().GetAll())
            {
                Assert.AreSame(transaction, ta.Transaction);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the remove quantity answers does not cascades to quantity answer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestRemoveQuantityAnswersDoesNotCascadesToQuantityAnswer()
        {
            try
            {
                #region Arrange
                LoadQuestionSets(1);
                LoadQuestionTypes(1);
                LoadQuestions(1);
                var quantityAnswers = new List<QuantityAnswer>();
                for (int i = 0; i < 5; i++)
                {
                    quantityAnswers.Add(CreateValidEntities.QuantityAnswer(i + 1));
                    quantityAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                    quantityAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
                }
                var transaction = GetValid(9);
                transaction.AddQuantityAnswer(quantityAnswers[1]);
                transaction.AddQuantityAnswer(quantityAnswers[2]);
                transaction.AddQuantityAnswer(quantityAnswers[4]);

                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Arrange

                #region Act
                transaction.QuantityAnswers.Remove(transaction.QuantityAnswers.ElementAt(1));
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act

            }
            catch (Exception ex)
            {
                #region Assert
                Assert.AreEqual("could not delete collection rows: [CRP.Core.Domain.Transaction.QuantityAnswers#6][SQL: UPDATE QuantityAnswers SET TransactionId = null WHERE TransactionId = @p0 AND id = @p1]", ex.Message);
                #endregion Assert
                throw;
            }
        }

        /// <summary>
        /// Tests the remove quantity answers does not cascades to quantity answer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestRemoveTransactionDoesNotCascadesToQuantityAnswer()
        {
            try
            {
                #region Arrange
                LoadQuestionSets(1);
                LoadQuestionTypes(1);
                LoadQuestions(1);
                var quantityAnswers = new List<QuantityAnswer>();
                for (int i = 0; i < 5; i++)
                {
                    quantityAnswers.Add(CreateValidEntities.QuantityAnswer(i + 1));
                    quantityAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(1);
                    quantityAnswers[i].Question = Repository.OfType<Question>().GetNullableByID(1);
                }
                var transaction = GetValid(9);
                transaction.AddQuantityAnswer(quantityAnswers[1]);
                transaction.AddQuantityAnswer(quantityAnswers[2]);
                transaction.AddQuantityAnswer(quantityAnswers[4]);

                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.Remove(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act

            }
            catch (Exception ex)
            {
                #region Assert
                Assert.AreEqual("could not delete collection: [CRP.Core.Domain.Transaction.QuantityAnswers#6][SQL: UPDATE QuantityAnswers SET TransactionId = null WHERE TransactionId = @p0]", ex.Message);
                #endregion Assert
                throw;
            }
        }
        #endregion CRUD Tests
        #endregion QuantityAnswers Collection Tests

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
                results.AssertErrorsAre("ChildTransactions: may not be empty");
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

        #region DonationTotal Tests

        [TestMethod]
        public void TestDonationTotalReturnsExpectedResult()
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
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.DonationTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(25, result);
            #endregion Assert		
        }

        #endregion DonationTotal Tests

        #region AmountTotal Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestAmountTotalReturnsExpectedResult()
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
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.AmountTotal;
            #endregion Act

            #region Assert
            Assert.AreEqual(14, result);
            #endregion Assert
        }

        #endregion AmountTotal Tests

        #region Total Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalReturnsExpectedResult()
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
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);
            transaction.AddChildTransaction(donationTransaction3);
            #endregion Arrange

            #region Act
            var result = transaction.Total;
            #endregion Act

            #region Assert
            Assert.AreEqual(39, result);
            #endregion Assert
        }

        #endregion Total Tests

        #region TotalPaid Tests

        /// <summary>
        /// Tests the amount total returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Amount = i+1;
                paymentLogs[i].Accepted = false;
            }
            paymentLogs[2].Accepted = true;
            paymentLogs[4].Accepted = true;
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaid;
            #endregion Act

            #region Assert
            Assert.AreEqual(8, result);
            #endregion Assert
        }

        #endregion TotalPaid Tests
        #region TotalPaidByCheck Tests
        /// <summary>
        /// Tests the total paid by check returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidByCheckReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));                
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }

            paymentLogs[0].Amount = 1;
            paymentLogs[1].Amount = 10;
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[3].Accepted = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;
            
            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaidByCheck;
            #endregion Act

            #region Assert
            Assert.AreEqual(10100, result);
            #endregion Assert
        }
        #endregion TotalPaidByCheck Tests
        #region TotalPaidByCredit Tests
        /// <summary>
        /// Tests the total paid by check returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidByCreditReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Credit = false;
                paymentLogs[i].Check = true;
            }

            paymentLogs[0].Amount = 1;
            paymentLogs[1].Amount = 10;
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Credit = true;
            paymentLogs[2].Check = false;
            paymentLogs[3].Credit = true;
            paymentLogs[3].Check = false;
            paymentLogs[3].Accepted = false;
            paymentLogs[4].Credit = true;
            paymentLogs[4].Check = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaidByCredit;
            #endregion Act

            #region Assert
            Assert.AreEqual(10100, result);
            #endregion Assert
        }
        #endregion TotalPaidByCredit Tests

        #region Paid Tests
        /// <summary>
        /// Tests the paid is true.
        /// </summary>
        [TestMethod]
        public void TestPaidIsTrue()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the paid is false1.
        /// </summary>
        [TestMethod]
        public void TestPaidIsFalse1()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            //transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }
        [TestMethod]
        public void TestPaidIsFalse2()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            //transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the paid is false3.
        /// </summary>
        [TestMethod]
        public void TestPaidIsFalse3()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 11100m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }
            var donationTransaction1 = new Transaction(transaction.Item);
            donationTransaction1.Donation = false;
            donationTransaction1.Amount = 1.00m;
            transaction.AddChildTransaction(donationTransaction1);
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 10.0m;
            transaction.AddChildTransaction(donationTransaction2);

            paymentLogs[0].Amount = 1; //Child transaction 1
            paymentLogs[1].Amount = 10; //Child transaction 2
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;
            
            paymentLogs[0].Accepted = false;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.Paid;
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        #endregion Paid Tests

        #region PaymentType Tests
        /// <summary>
        /// Tests the payment type with credit and check are false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPaymentTypeWithCreditAndCheckAreFalseDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Credit = false;
                transaction.Check = false;
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
                results.AssertErrorsAre("PaymentType: Payment type was not selected.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }
        /// <summary>
        /// Tests the payment type with credit and check are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPaymentTypeWithCreditAndCheckAreTrueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Credit = true;
                transaction.Check = true;
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
                results.AssertErrorsAre("PaymentType: Payment type was not selected.");
                Assert.IsTrue(transaction.IsTransient());
                Assert.IsFalse(transaction.IsValid());
                throw;
            }
        }

        #endregion PaymentType Tests

        #region CorrectionAmount Tests
        [TestMethod]
        public void TestCorrectionAmountSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 1m;
            var donationTransaction = new Transaction(transaction.Item);
            donationTransaction.Donation = true;
            donationTransaction.Amount = 10.00m;
            var donationTransaction2 = new Transaction(transaction.Item);
            donationTransaction2.Donation = true;
            donationTransaction2.Amount = 15.00m;
            transaction.AddChildTransaction(donationTransaction);
            transaction.AddChildTransaction(donationTransaction2);

            var correction = new Transaction(transaction.Item);
            correction.Donation = false;
            correction.Amount = -15.00m;
            correction.CreatedBy = "Test";
            transaction.AddChildTransaction(correction);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, TransactionRepository.GetAll().Where(a => a.ParentTransaction == transaction).Count());
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(-15m, transaction.CorrectionTotal);
            Assert.AreEqual(11m, transaction.Total);
            #endregion Assert
        }
        #endregion CorrectionAmount Tests

        #region CreatedBy Tests

        [TestMethod]
        public void TestCreatedByWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.IsNull(transaction.CreatedBy);
            #endregion Assert		
        }
        [TestMethod]
        public void TestCreatedByWithEmptyValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = string.Empty;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(string.Empty, transaction.CreatedBy);
            #endregion Assert
        }
        [TestMethod]
        public void TestCreatedByWithSpacesOnlySaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = " ";
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(" ", transaction.CreatedBy);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatedByWithFiftyCharactersSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.CreatedBy = " ";
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            Assert.AreEqual(" ", transaction.CreatedBy);
            #endregion Assert
        }
        #endregion CreatedBy Tests

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
            expectedFields.Add(new NameAndType("Amount", "System.Decimal", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(Min = 0, Message = \"must be zero or more\")]"
            }));
            expectedFields.Add(new NameAndType("AmountTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Check", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ChildTransactions", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Transaction]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Credit", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Donation", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("DonationTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("OpenIDUser", "CRP.Core.Domain.OpenIdUser", new List<string>()));
            expectedFields.Add(new NameAndType("Paid", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ParentTransaction", "CRP.Core.Domain.Transaction", new List<string>()));
            expectedFields.Add(new NameAndType("PaymentLogs", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.PaymentLog]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("PaymentType", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Payment type was not selected.\")]"
            }));
            expectedFields.Add(new NameAndType("Quantity", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityAnswers", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.QuantityAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Total", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaid", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaidByCheck", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaidByCredit", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionAnswers", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.TransactionAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("TransactionDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionNumber", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Transaction));

        }

        #endregion Reflection of Database
    }
}
