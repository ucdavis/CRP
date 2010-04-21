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
    }
}
