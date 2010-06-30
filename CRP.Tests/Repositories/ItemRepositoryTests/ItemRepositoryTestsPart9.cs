using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
        #region TransactionQuestionSet Tests
        #region Invalid Tests        
        /// <summary>
        /// Tests the question sets with duplicate transaction value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateTransactionValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddTransactionQuestionSet(questionSet);
                item.AddTransactionQuestionSet(questionSet);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionQuestionSet: Transaction Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        /// <summary>
        /// Tests the question sets with duplicate transaction value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateTransactionValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet1 = CreateValidEntities.QuestionSet(1);
                var questionSet2 = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddTransactionQuestionSet(questionSet1);
                item.AddTransactionQuestionSet(questionSet2);
                item.AddTransactionQuestionSet(questionSet1);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionQuestionSet: Transaction Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transaction ignores duplicate in quantity and saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionIgnoresDuplicateInQuantityAndSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.AddTransactionQuestionSet(questionSet);
            item.AddQuantityQuestionSet(questionSet);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, item.QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }

        #endregion Valid Tests
        #endregion TransactionQuestionSet Tests
        #region QuantityQuestionSet Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the question sets with duplicate quantity value does not save1.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateQuantityValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddQuantityQuestionSet(questionSet);
                item.AddQuantityQuestionSet(questionSet);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityQuestionSet: Quantity Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the question sets with duplicate quantity value does not save2.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateQuantityValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet1 = CreateValidEntities.QuestionSet(1);
                var questionSet2 = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddQuantityQuestionSet(questionSet1);
                item.AddQuantityQuestionSet(questionSet2);
                item.AddQuantityQuestionSet(questionSet1);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityQuestionSet: Quantity Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transaction ignores duplicate in transaction and saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionIgnoresDuplicateInTransactionAndSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.AddQuantityQuestionSet(questionSet);
            item.AddTransactionQuestionSet(questionSet);            
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, item.QuestionSets.Where(a => a.QuantityLevel).Count());
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion QuantityQuestionSet Tests

        #region AllowCheckPayment Tests

        /// <summary>
        /// Tests the AllowCheckPayment is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCheckPaymentIsFalseSaves()
        {
            #region Arrange
            Item item = GetValid(9);
            item.AllowCheckPayment = false;            
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowCheckPayment is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCheckPaymentIsTrueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.AllowCheckPayment = true;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion AllowCheckPayment Tests

        #region AllowCreditPayment Tests

        /// <summary>
        /// Tests the AllowCreditPayment is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCreditPaymentIsFalseSaves()
        {
            #region Arrange

            Item item = GetValid(9);
            item.AllowCreditPayment = false;

            #endregion Arrange

            #region Act

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(item.AllowCreditPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowCreditPayment is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCreditPaymentIsTrueSaves()
        {
            #region Arrange

            var item = GetValid(9);
            item.AllowCreditPayment = true;

            #endregion Arrange

            #region Act

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(item.AllowCreditPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());

            #endregion Assert
        }

        #endregion AllowCreditPayment Tests

        #region AllowedPaymentMethods Tests

        /// <summary>
        /// Tests the allowed payment methods allows save with default values.
        /// </summary>
        [TestMethod]
        public void TestAllowedPaymentMethodsAllowsSaveWithDefaultValues()
        {
            #region Arrange
            var item = GetValid(9);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(item.AllowCreditPayment);
            Assert.IsTrue(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the allowed payment methods prevents save with both payment method values false.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAllowedPaymentMethodsPreventsSaveWithBothPaymentMethodValuesFalse()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.AllowCreditPayment = false;
                item.AllowCheckPayment = false;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.IsFalse(item.AllowCreditPayment);
                Assert.IsFalse(item.AllowCheckPayment);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AllowedPaymentMethods: Must check at least one payment method");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }



        #endregion AllowedPaymentMethods Tests

    }
}
