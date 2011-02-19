using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
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
            transactionAnswer.QuestionSet = Repository.OfType<QuestionSet>().GetNullableById(1);
            transactionAnswer.Question = Repository.OfType<Question>().GetNullableById(1);
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
                transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableById(1);
                transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableById(1);
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
                    transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableById(1);
                    transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableById(1);
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
            catch (Exception ex)
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
                    transactionAnswers[i].QuestionSet = Repository.OfType<QuestionSet>().GetNullableById(1);
                    transactionAnswers[i].Question = Repository.OfType<Question>().GetNullableById(1);
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
    }
}
