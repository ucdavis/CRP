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
    public class QuestionRepositoryTests :AbstractRepositoryTests<Question, int >
    {
        private IRepository<Question> QuestionRepository { get; set; }

        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepositoryTests"/> class.
        /// </summary>
        public QuestionRepositoryTests()
        {
            QuestionRepository = new Repository<Question>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Question GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Question(counter);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            var questionType = Repository.OfType<QuestionType>().GetById(1);
            rtValue.QuestionType = questionType;

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Question> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Question>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Question entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Question entity, ARTAction action)
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
        /// Question requires QuestionSet
        /// Question requires QuestionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadRecords(5);
            Repository.OfType<Question>().DbContext.CommitTransaction();
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = " ";
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = "x".RepeatTimes(201);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(201, question.Name.Length);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            var question = GetValid(9);
            question.Name = "x";
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, question.Name.Length);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region QuestionType Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the question type with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionTypeWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionType: may not be empty");
                Assert.IsFalse(question.IsValid());
                Assert.IsTrue(question.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the question type with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestQuestionTypeWithNewValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = new QuestionType();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(question);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Question.QuestionType", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the question type with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionTypeWithValidValueSaves()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes(3);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
            var question = GetValid(9);
            var questionType = Repository.OfType<QuestionType>().GetNullableByID(3);
            question.QuestionType = questionType;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Test
        /// <summary>
        /// Tests the type of the delete question does not cascade to question.
        /// </summary>
        [TestMethod]
        public void TestDeleteQuestionDoesNotCascadeToQuestionType()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes(3);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
            var question = GetValid(9);
            var questionType = Repository.OfType<QuestionType>().GetNullableByID(3);
            question.QuestionType = questionType;

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<QuestionType>().GetAll().Count); //because we load 1 in init                
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<QuestionType>().GetAll().Count); //because we load 1 in init
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion CRUD Test
        #endregion QuestionType Tests
        #region QuestionSet Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the QuestionSet with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionSet = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSet: may not be empty");
                Assert.IsFalse(question.IsValid());
                Assert.IsTrue(question.IsTransient());
                #endregion Assert

                throw;
            }
        }
        /// <summary>
        /// Tests the QuestionSet with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionSetWithNewValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(question);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.QuestionSet, Entity: CRP.Core.Domain.QuestionSet", ex.Message);
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
            var question = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(3);
            question.QuestionSet = questionSet;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Test
        /// <summary>
        /// Tests the type of the delete question does not cascade to QuestionSet.
        /// </summary>
        [TestMethod]
        public void TestDeleteQuestionDoesNotCascadeToQuestionSet()
        {
            #region Arrange
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadQuestionSets(3);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            var question = GetValid(9);
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(3);
            question.QuestionSet = questionSet;

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //because we load 1 in init                
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count); //because we load 1 in init
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion CRUD Test
        #endregion QuestionSet Tests

        #region Options Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the options with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Options = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Options: may not be empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the options with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestOptionsWithEmptyListSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Options = new List<QuestionOption>();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the options for mapping problem.
        /// </summary>
        [TestMethod]
        public void TestOptionsForMappingProblem()
        {
            #region Arrange
            SetOptions(true);
            var questionOption = CreateValidEntities.QuestionOption(1);
            Assert.AreEqual(0, Repository.OfType<QuestionOption>().GetAll().Count);

            var question = GetValid(null);
            question.Options = new List<QuestionOption>();
            question.AddOption(questionOption);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, Repository.OfType<QuestionOption>().GetAll().Count);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the new options are saved.
        /// </summary>
        [TestMethod]
        public void TestNewOptionsAreSaved()
        {
            #region Arrange          
            SetOptions(true);
            var question = GetValid(null);
            question.AddOption(CreateValidEntities.QuestionOption(1));
            question.AddOption(CreateValidEntities.QuestionOption(2));
            Assert.AreEqual(0, Repository.OfType<QuestionOption>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<QuestionOption>().GetAll().Count);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert	
        }

        /// <summary>
        /// Tests the remove option cascades to question option.
        /// </summary>
        [TestMethod]
        public void TestRemoveOptionCascadesToQuestionOption()
        {
            #region Arrange
            SetOptions(true);
            var question = GetValid(null);
            question.AddOption(CreateValidEntities.QuestionOption(1));
            question.AddOption(CreateValidEntities.QuestionOption(2));
            question.AddOption(CreateValidEntities.QuestionOption(3));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<QuestionOption>().GetAll().Count);
            Assert.AreEqual(3, question.Options.Count);
            #endregion Arrange

            #region Act
            question.RemoveOptions(question.Options.ElementAt(1));
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, question.Options.Count);
            Assert.AreEqual(2, Repository.OfType<QuestionOption>().GetAll().Count);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert		
        }
        /// <summary>
        /// Tests the remove question cascades to question option.
        /// </summary>
        [TestMethod]
        public void TestRemoveQuestionCascadesToQuestionOption()
        {
            #region Arrange
            SetOptions(true);
            var question = GetValid(null);
            question.AddOption(CreateValidEntities.QuestionOption(1));
            question.AddOption(CreateValidEntities.QuestionOption(2));
            question.AddOption(CreateValidEntities.QuestionOption(3));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<QuestionOption>().GetAll().Count);
            Assert.AreEqual(3, question.Options.Count);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, Repository.OfType<QuestionOption>().GetAll().Count);
            #endregion Assert
        }
        /// <summary>
        /// Tests the update option cascades to question option.
        /// </summary>
        [TestMethod]
        public void TestUpdateOptionCascadesToQuestionOption()
        {
            #region Arrange
            var question = GetValid(null);
            SetOptions(true);
            question.AddOption(CreateValidEntities.QuestionOption(1));
            question.AddOption(CreateValidEntities.QuestionOption(2));
            question.AddOption(CreateValidEntities.QuestionOption(3));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            Assert.AreEqual("Name2", Repository.OfType<QuestionOption>().GetById(2).Name);
            #endregion Arrange

            #region Act
            question.Options.ElementAt(1).Name = "Updated";
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            
            Assert.AreEqual("Updated", Repository.OfType<QuestionOption>().GetById(2).Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Options Collection Tests

        #region Validators Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the Validators with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestValidatorsWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Validators = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Validators: may not be empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the Validators with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestValidatorsWithEmptyListSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Validators = new List<Validator>();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the Validators for mapping problem.
        /// </summary>
        [TestMethod]
        public void TestValidatorsForMappingProblem()
        {
            #region Arrange
            Repository.OfType<Validator>().DbContext.BeginTransaction();
            LoadValidators(3);
            Repository.OfType<Validator>().DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);

            var question = GetValid(null);
            question.Validators = new List<Validator>();
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(2));
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);
            Assert.AreEqual(1, question.Validators.Count());
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the new Validators are saved.
        /// Validators has a cross table
        /// </summary>
        [TestMethod]
        public void TestNewValidatorsAreNotSaved()
        {
            #region Arrange
            Repository.OfType<Validator>().DbContext.BeginTransaction();
            LoadValidators(3);
            Repository.OfType<Validator>().DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);

            var question = GetValid(null);
            question.Validators = new List<Validator>();
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(2));
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(1));
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);
            Assert.AreEqual(2, question.Validators.Count());
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the remove Validators does not cascade to Validators.
        /// </summary>
        [TestMethod]
        public void TestRemoveValidatorsDoesNotCascadeToValidators()
        {
            #region Arrange
            Repository.OfType<Validator>().DbContext.BeginTransaction();
            LoadValidators(3);
            Repository.OfType<Validator>().DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);

            var question = GetValid(null);
            question.Validators = new List<Validator>();
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(2));
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(1));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);
            Assert.AreEqual(2, question.Validators.Count());
            #endregion Arrange

            #region Act
            question.Validators.Remove(question.Validators.ElementAt(0));
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<Validator>().GetAll().Count);
            Assert.AreEqual(1, question.Validators.Count());
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Validators Collection Tests

        #region ValidationClasses Tests

        /// <summary>
        /// Tests the validation classes joins as expected.
        /// </summary>
        [TestMethod]
        public void TestValidationClassesJoinsAsExpected()
        {
            #region Arrange
            Repository.OfType<Validator>().DbContext.BeginTransaction();
            LoadValidators(3);
            Repository.OfType<Validator>().DbContext.CommitTransaction();

            var question = GetValid(null);
            question.Validators = new List<Validator>();
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(2));
            question.Validators.Add(Repository.OfType<Validator>().GetNullableByID(1));
            #endregion Arrange

            #region Act
            var result = question.ValidationClasses;
            #endregion Act

            #region Assert
            Assert.AreEqual("Class2 Class1", result);
            #endregion Assert	
        }

        #endregion ValidationClasses Tests

        #region Order Tests

        /// <summary>
        /// Tests the order value of zero saves.
        /// </summary>
        [TestMethod]
        public void TestOrderValueOfZeroSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Order = 0;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert	
        }
        /// <summary>
        /// Tests the order with small value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithSmallValueSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Order = int.MinValue;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the order with large value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithLargeValueSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Order = int.MaxValue;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        
        #endregion Order Tests

        #region OptionsNames Tests
        /// <summary>
        /// Tests the options names with invalid value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsNamesWithNameWithSpacesOnlyDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                SetOptions(true);
                question = GetValid(9);
                question.AddOption(CreateValidEntities.QuestionOption(1));
                question.AddOption(CreateValidEntities.QuestionOption(2));
                question.AddOption(CreateValidEntities.QuestionOption(3));
                question.Options.ElementAt(1).Name = " ";
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNames: One or more options is invalid");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the options names with name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsNamesWithNameWithEmptyStringDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                SetOptions(true);
                question = GetValid(9);
                question.AddOption(CreateValidEntities.QuestionOption(1));
                question.AddOption(CreateValidEntities.QuestionOption(2));
                question.AddOption(CreateValidEntities.QuestionOption(3));
                question.Options.ElementAt(1).Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNames: One or more options is invalid");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }
        /// <summary>
        /// Tests the options names with name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsNamesWithNameWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                SetOptions(true);
                question = GetValid(9);
                question.AddOption(CreateValidEntities.QuestionOption(1));
                question.AddOption(CreateValidEntities.QuestionOption(2));
                question.AddOption(CreateValidEntities.QuestionOption(3));
                question.Options.ElementAt(1).Name = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNames: One or more options is invalid");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        #endregion OptionsNames Tests

        #region OptionsRequired Tests
        /// <summary>
        /// Tests the options required with no options does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsRequiredWithNoOptionsDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                SetOptions(true);
                question = GetValid(9);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsRequired: The question type requires at least one option.");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        #endregion OptionsRequired Tests

        #region OptionsNotAllowed Tests
        /// <summary>
        /// Tests the options not allowed with options does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsNotAllowedWithOptionsDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                SetOptions(true);
                question = GetValid(9);
                question.AddOption(CreateValidEntities.QuestionOption(1));
                SetOptions(false);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNotAllowed: Options not allowed");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        #endregion OptionsNotAllowed Tests

        #region AddOption Test

        /// <summary>
        /// Tests the does not add option when has options is false.
        /// </summary>
        [TestMethod]
        public void TestDoesNotAddOptionWhenHasOptionsIsFalse()
        {
            #region Arrange
            SetOptions(false);
            var question = GetValid(9);
            #endregion Arrange

            #region Act
            question.AddOption(CreateValidEntities.QuestionOption(1));
            #endregion Act

            #region Assert
            Assert.AreEqual(0, question.Options.Count);
            #endregion Assert		
        }

        #endregion AddOption Test

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
            expectedFields.Add(new NameAndType("Options", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.QuestionOption]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("OptionsNames", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more options is invalid\")]"
            }));
            expectedFields.Add(new NameAndType("OptionsNotAllowed", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Options not allowed\")]"
            }));
            expectedFields.Add(new NameAndType("OptionsRequired", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"The question type requires at least one option.\")]"
            }));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionSet", "CRP.Core.Domain.QuestionSet", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuestionType", "CRP.Core.Domain.QuestionType", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ValidationClasses", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Validators", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Validator]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Question));

        }

        #endregion Reflection of Database

        /// <summary>
        /// Turns the on options.
        /// </summary>
        private void SetOptions(bool hasOptions)
        {
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            var questionType = Repository.OfType<QuestionType>().GetById(1);
            questionType.HasOptions = hasOptions;
            Repository.OfType<QuestionType>().EnsurePersistent(questionType);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
        }
    }
}
