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
    [TestClass]
    public class TransactionAnswerRepositoryTests : AbstractRepositoryTests<TransactionAnswer, int >
    {
        protected IRepository<TransactionAnswer> TransactionAnswerRepository { get; set; }

        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionAnswerRepositoryTests"/> class.
        /// </summary>
        public TransactionAnswerRepositoryTests()
        {
            TransactionAnswerRepository = new Repository<TransactionAnswer>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override TransactionAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.TransactionAnswer(counter);
            rtValue.Transaction = Repository.OfType<Transaction>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            rtValue.Question = Repository.OfType<Question>().GetById(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<TransactionAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<TransactionAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(TransactionAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(TransactionAnswer entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Answer);
                    break;
                case ARTAction.Restore:
                    entity.Answer = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Answer;
                    entity.Answer = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// TransactionAnswer requires Transaction
        ///     Transaction requires Items
        ///         Items requires Units and ItemTypes
        /// TransactionAnswer requires QuestionSet
        /// TransactionAnswer requires Question
        ///     QuestionRequires QuestionSet and QuestionType 
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<TransactionAnswer>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            LoadTransactions(1);
            LoadRecords(5);
            Repository.OfType<TransactionAnswer>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Transaction Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the transaction with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionWithNullValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Transaction = null;
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Transaction: may not be null");
                Assert.IsFalse(transactionAnswer.IsValid());
                Assert.IsTrue(transactionAnswer.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the transaction with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestTransactionWithNewValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Transaction = new Transaction();
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.TransactionAnswer.Transaction", ex.Message);
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
            var transactionAnswer = GetValid(9);
            var transaction = Repository.OfType<Transaction>().GetNullableById(3);
            transaction.AddTransactionAnswer(transactionAnswer);
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transactionAnswer.IsTransient());
            Assert.IsTrue(transactionAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Test

        /// <summary>
        /// Tests the delete transaction answer does not cascade to transaction.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public void TestDeleteTransactionAnswerDoesNotCascadeToTransaction()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange

                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                LoadTransactions(3);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();
                transactionAnswer = GetValid(9);
                var transaction = Repository.OfType<Transaction>().GetNullableById(3);
                transaction.AddTransactionAnswer(transactionAnswer);
                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();

                Assert.AreEqual(4, Repository.OfType<Transaction>().GetAll().Count); //because we load 1 in init
                Assert.AreSame(transactionAnswer, Repository.OfType<Transaction>().GetNullableById(3).TransactionAnswers.ElementAt(0));

                #endregion Arrange

                #region Act

                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.Remove(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();

                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.TransactionAnswer#6]", ex.Message);
                Assert.AreEqual(4, Repository.OfType<Transaction>().GetAll().Count);
                #endregion Assert

                throw;
            }
        }
        #endregion CRUD Test

        #endregion Transaction Tests

        #region QuestionSet Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the question set with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetWithNullValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.QuestionSet = null;
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSet: may not be null");
                Assert.IsFalse(transactionAnswer.IsValid());
                Assert.IsTrue(transactionAnswer.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the question set with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestQuestionSetWithNewValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.TransactionAnswer.QuestionSet", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the question set with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadQuestionSets(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var transactionAnswer = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(3);
            transactionAnswer.QuestionSet = questionSet;
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transactionAnswer.IsTransient());
            Assert.IsTrue(transactionAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests

        /// <summary>
        /// Tests the delete transaction answer does not cascade to question set.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionAnswerDoesNotCascadeToQuestionSet()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadQuestionSets(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var transactionAnswer = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(3);
            transactionAnswer.QuestionSet = questionSet;

            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //One added in load data
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.Remove(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //One added in load data
            #endregion Assert		
        }
        #endregion CRUD Tests

        #endregion QuestionSet Tests

        #region Question Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the question  with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Question = null;
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null");
                Assert.IsFalse(transactionAnswer.IsValid());
                Assert.IsTrue(transactionAnswer.IsTransient());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the question with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestQuestionWithNewValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Question = new Question();
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transactionAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.TransactionAnswer.Question", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the question with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();            
            LoadQuestions(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var transactionAnswer = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableById(3);
            transactionAnswer.Question = question;
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transactionAnswer.IsTransient());
            Assert.IsTrue(transactionAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests

        /// <summary>
        /// Tests the delete question does not cascade to question.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionAnswerDoesNotCascadeToQuestion()
        {
            #region Arrange
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestions(3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            var transactionAnswer = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableById(3);
            transactionAnswer.Question = question;

            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //One added in load data
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.Remove(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //One added in load data
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Question Tests

        #region Answer Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Answer with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithNullValueDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Answer = null;
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(transactionAnswer.IsTransient());
                Assert.IsFalse(transactionAnswer.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithEmptyStringDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Answer = string.Empty;
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(transactionAnswer.IsTransient());
                Assert.IsFalse(transactionAnswer.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithSpacesOnlyDoesNotSave()
        {
            TransactionAnswer transactionAnswer = null;
            try
            {
                #region Arrange
                transactionAnswer = GetValid(9);
                transactionAnswer.Answer = " ";
                #endregion Arrange

                #region Act
                TransactionAnswerRepository.DbContext.BeginTransaction();
                TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
                TransactionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(transactionAnswer);
                var results = transactionAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(transactionAnswer.IsTransient());
                Assert.IsFalse(transactionAnswer.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Answer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneCharacterSaves()
        {
            #region Arrange
            var transactionAnswer = GetValid(9);
            transactionAnswer.Answer = "x";
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(transactionAnswer.IsTransient());
            Assert.IsTrue(transactionAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var transactionAnswer = GetValid(9);
            transactionAnswer.Answer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            TransactionAnswerRepository.DbContext.BeginTransaction();
            TransactionAnswerRepository.EnsurePersistent(transactionAnswer);
            TransactionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, transactionAnswer.Answer.Length);
            Assert.IsFalse(transactionAnswer.IsTransient());
            Assert.IsTrue(transactionAnswer.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Answer Tests
  
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
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Question", "CRP.Core.Domain.Question", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuestionSet", "CRP.Core.Domain.QuestionSet", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Transaction", "CRP.Core.Domain.Transaction", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TransactionAnswer));

        }

        #endregion Reflection of Database
    }
}
