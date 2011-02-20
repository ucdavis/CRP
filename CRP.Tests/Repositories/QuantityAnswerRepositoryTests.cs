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
    public class QuantityAnswerRepositoryTests : AbstractRepositoryTests<QuantityAnswer, int>
    {        
        protected IRepository<QuantityAnswer> QuantityAnswerRepository { get; set; }

        #region Init and Overrides
        public QuantityAnswerRepositoryTests()
        {
            QuantityAnswerRepository = new Repository<QuantityAnswer>();           
        }
        
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuantityAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.QuantityAnswer(counter);
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
        protected override IQueryable<QuantityAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<QuantityAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuantityAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuantityAnswer entity, ARTAction action)
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
        /// QuantityAnswer Requires Transaction
        ///     Transaction Requires Item
        ///         Item requires Unit and ItemType
        /// QuantityAnswer Requires QuestionSet
        /// QuantityAnswer Requires Question
        ///     Question Requires QuestionSet and QuestionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<QuantityAnswer>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadTransactions(1);
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            //LoadQuestionSets(1);
            LoadQuestions(1);
            LoadRecords(5);
            Repository.OfType<QuantityAnswer>().DbContext.CommitTransaction();
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Transaction = null;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Transaction: may not be null");
                Assert.IsFalse(quantityAnswer.IsValid());
                Assert.IsTrue(quantityAnswer.IsTransient());
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Transaction = new Transaction();
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.QuantityAnswer.Transaction", ex.Message);
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
            var quantityAnswer = GetValid(9);
            var transaction = Repository.OfType<Transaction>().GetNullableById(3);
            transaction.AddQuantityAnswer(quantityAnswer);
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Test

        /// <summary>
        /// Tests the delete QuantityAnswer does not cascade to transaction.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public void TestDeleteQuantityAnswerDoesNotCascadeToTransaction()
        {
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange

                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                LoadTransactions(3);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();
                quantityAnswer = GetValid(9);
                var transaction = Repository.OfType<Transaction>().GetNullableById(3);
                transaction.AddQuantityAnswer(quantityAnswer);
                Repository.OfType<Transaction>().DbContext.BeginTransaction();
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Repository.OfType<Transaction>().DbContext.CommitTransaction();

                Assert.AreEqual(4, Repository.OfType<Transaction>().GetAll().Count); //because we load 1 in init
                Assert.AreSame(quantityAnswer, Repository.OfType<Transaction>().GetNullableById(3).QuantityAnswers.ElementAt(0));

                #endregion Arrange

                #region Act

                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.Remove(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();

                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.QuantityAnswer#6]", ex.Message);
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.QuestionSet = null;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSet: may not be null");
                Assert.IsFalse(quantityAnswer.IsValid());
                Assert.IsTrue(quantityAnswer.IsTransient());
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.QuantityAnswer.QuestionSet", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        /// <summary>
        /// Tests the QuestionSet with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadQuestionSets(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var quantityAnswer = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(3);
            quantityAnswer.QuestionSet = questionSet;
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Test

        [TestMethod]
        public void TestDeleteQuantityAnswerDoesNotCascadeToQuestionSet()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadQuestionSets(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var quantityAnswer = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(3);
            quantityAnswer.QuestionSet = questionSet;

            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //because we load 1 in init                
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.Remove(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //because we load 1 in init
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }

        #endregion CRUD Test
        #endregion QuestionSet Tests

        #region Question Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the question with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Question = null;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null");
                Assert.IsFalse(quantityAnswer.IsValid());
                Assert.IsTrue(quantityAnswer.IsTransient());
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Question = new Question();
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.QuantityAnswer.Question", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        /// <summary>
        /// Tests the Question with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestions(3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            var quantityAnswer = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableById(3);
            quantityAnswer.Question = question;
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Test

        /// <summary>
        /// Tests the delete quantity answer does not cascade to question.
        /// </summary>
        [TestMethod]
        public void TestDeleteQuantityAnswerDoesNotCascadeToQuestion()
        {
            #region Arrange
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestions(3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            var quantityAnswer = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableById(3);
            quantityAnswer.Question = question;

            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //because we load 1 in init                
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.Remove(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //because we load 1 in init
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }

        #endregion CRUD Test
        #endregion QuestionSet Tests

        #region QuantityId Tests

        /// <summary>
        /// Tests the quantity id with valid GUID saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityIdWithValidGuidSaves()
        {
            #region Arrange
            var quantityAnswer = GetValid(9);
            quantityAnswer.QuantityId = Guid.NewGuid();
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreNotEqual(Guid.Empty, quantityAnswer.QuantityId);
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert		
        }

        #endregion QuantityId Tests

        #region Answer Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Answer with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithNullValueDoesNotSave()
        {
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Answer = null;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(quantityAnswer.IsTransient());
                Assert.IsFalse(quantityAnswer.IsValid());
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Answer = string.Empty;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(quantityAnswer.IsTransient());
                Assert.IsFalse(quantityAnswer.IsValid());
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
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.Answer = " ";
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answer: may not be null or empty");
                Assert.IsTrue(quantityAnswer.IsTransient());
                Assert.IsFalse(quantityAnswer.IsValid());
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
            var quantityAnswer = GetValid(9);
            quantityAnswer.Answer = "x";
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var quantityAnswer = GetValid(9);
            quantityAnswer.Answer = "x".RepeatTimes(1000);
            #endregion Arrange

            #region Act
            QuantityAnswerRepository.DbContext.BeginTransaction();
            QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
            QuantityAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, quantityAnswer.Answer.Length);
            Assert.IsFalse(quantityAnswer.IsTransient());
            Assert.IsTrue(quantityAnswer.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests

        #endregion Answer Tests

        #region QuantityIdNotEmpty Test

        /// <summary>
        /// Tests the quantity id not empty does not save if quantity id is empty.
        /// This one should not ever happen.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityIdNotEmptyDoesNotSaveIfQuantityIdIsEmpty()
        {
            QuantityAnswer quantityAnswer = null;
            try
            {
                #region Arrange
                quantityAnswer = GetValid(9);
                quantityAnswer.QuantityId = Guid.Empty;
                #endregion Arrange

                #region Act
                QuantityAnswerRepository.DbContext.BeginTransaction();
                QuantityAnswerRepository.EnsurePersistent(quantityAnswer);
                QuantityAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(quantityAnswer);
                var results = quantityAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityIdNotEmpty: QuantityId may not be empty.");
                Assert.IsFalse(quantityAnswer.IsValid());
                Assert.IsTrue(quantityAnswer.IsTransient());
                #endregion Assert

                throw;
            }
        }

        #endregion QuantityIdNotEmpty Test


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
            expectedFields.Add(new NameAndType("QuantityId", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityIdNotEmpty", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"QuantityId may not be empty.\")]"
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

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuantityAnswer));

        }



        #endregion Reflection of Database
    }
}
