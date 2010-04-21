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
    public class QuestionOptionRepositoryTests : AbstractRepositoryTests<QuestionOption, int >
    {
        protected IRepository<QuestionOption> QuestionOptionRepository { get; set; }
        
        #region Init and Overrides
        public QuestionOptionRepositoryTests()
        {
            QuestionOptionRepository = new Repository<QuestionOption>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuestionOption GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.QuestionOption(counter);
            rtValue.Question = Repository.OfType<Question>().GetByID(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuestionOption> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<QuestionOption>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuestionOption entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuestionOption entity, ARTAction action)
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
        /// QuestionOption requires Question
        ///     Question Requires QuestionSet and QustionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<QuestionOption>().DbContext.BeginTransaction();
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            LoadRecords(5);
            Repository.OfType<QuestionOption>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = null;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = " ";
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = "x".RepeatTimes(201);
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                Assert.AreEqual(201, questionOption.Name.Length);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var questionOption = GetValid(9);
            questionOption.Name = "x";
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var questionOption = GetValid(9);
            questionOption.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, questionOption.Name.Length);
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Question Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the question with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Question = null;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be empty");
                Assert.IsFalse(questionOption.IsValid());
                Assert.IsTrue(questionOption.IsTransient());
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
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Question = new Question();
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(questionOption);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.QuestionOption.Question", ex.Message);
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
            var questionOption = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableByID(3);
            questionOption.Question = question;
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region CRUD Test

        /// <summary>
        /// Tests the delete question option does not cascade to question.
        /// </summary>
        [TestMethod]
        public void TestDeleteQuestionOptionDoesNotCascadeToQuestion()
        {
            #region Arrange
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestions(3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            var questionOption = GetValid(9);
            var question = Repository.OfType<Question>().GetNullableByID(3);
            questionOption.Question = question;

            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //because we load 1 in init                
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.Remove(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count); //because we load 1 in init
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }

        #endregion CRUD Test
        #endregion QuestionSet Tests

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

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Question", "CRP.Core.Domain.Question", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionOption));

        }



        #endregion Reflection of Database
    }
}
