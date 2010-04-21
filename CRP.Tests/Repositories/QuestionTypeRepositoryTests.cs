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
    public class QuestionTypeRepositoryTests : AbstractRepositoryTests<QuestionType, int >
    {
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }

        #region Init and Overrides
        public QuestionTypeRepositoryTests()
        {
            QuestionTypeRepository = new Repository<QuestionType>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuestionType GetValid(int? counter)
        {
            return CreateValidEntities.QuestionType(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuestionType> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<QuestionType>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuestionType entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuestionType entity, ARTAction action)
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
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
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
            QuestionType questionType = null;
            try
            {
                #region Arrange
                questionType = GetValid(9);
                questionType.Name = null;
                #endregion Arrange

                #region Act
                QuestionTypeRepository.DbContext.BeginTransaction();
                QuestionTypeRepository.EnsurePersistent(questionType);
                QuestionTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionType);
                var results = questionType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionType.IsTransient());
                Assert.IsFalse(questionType.IsValid());
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
            QuestionType questionType = null;
            try
            {
                #region Arrange
                questionType = GetValid(9);
                questionType.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionTypeRepository.DbContext.BeginTransaction();
                QuestionTypeRepository.EnsurePersistent(questionType);
                QuestionTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionType);
                var results = questionType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionType.IsTransient());
                Assert.IsFalse(questionType.IsValid());
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
            QuestionType questionType = null;
            try
            {
                #region Arrange
                questionType = GetValid(9);
                questionType.Name = " ";
                #endregion Arrange

                #region Act
                QuestionTypeRepository.DbContext.BeginTransaction();
                QuestionTypeRepository.EnsurePersistent(questionType);
                QuestionTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionType);
                var results = questionType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionType.IsTransient());
                Assert.IsFalse(questionType.IsValid());
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
            QuestionType questionType = null;
            try
            {
                #region Arrange
                questionType = GetValid(9);
                questionType.Name = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                QuestionTypeRepository.DbContext.BeginTransaction();
                QuestionTypeRepository.EnsurePersistent(questionType);
                QuestionTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionType);
                Assert.AreEqual(51, questionType.Name.Length);
                var results = questionType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(questionType.IsTransient());
                Assert.IsFalse(questionType.IsValid());
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
            var questionType = GetValid(9);
            questionType.Name = "x";
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var questionType = GetValid(9);
            questionType.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, questionType.Name.Length);
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region HasOptions Tests

        /// <summary>
        /// Tests the HasOptions is false saves.
        /// </summary>
        [TestMethod]
        public void TestHasOptionsIsFalseSaves()
        {
            #region Arrange
            var questionType = GetValid(9);
            questionType.HasOptions = false;
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HasOptions is true saves.
        /// </summary>
        [TestMethod]
        public void TestHasOptionsIsTrueSaves()
        {
            #region Arrange
            var questionType = GetValid(9);
            questionType.HasOptions = true;
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        #endregion HasOptions Tests

        #region ExtendedProperty Tests

        /// <summary>
        /// Tests the ExtendedProperty is false saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyIsFalseSaves()
        {
            #region Arrange
            var questionType = GetValid(9);
            questionType.ExtendedProperty = false;
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ExtendedProperty is true saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyIsTrueSaves()
        {
            #region Arrange
            var questionType = GetValid(9);
            questionType.ExtendedProperty = true;
            #endregion Arrange

            #region Act
            QuestionTypeRepository.DbContext.BeginTransaction();
            QuestionTypeRepository.EnsurePersistent(questionType);
            QuestionTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionType.IsTransient());
            Assert.IsTrue(questionType.IsValid());
            #endregion Assert
        }

        #endregion ExtendedProperty Tests

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
            expectedFields.Add(new NameAndType("ExtendedProperty", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("HasOptions", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]",
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
                 
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionType));

        }

        #endregion Reflection of Database
    }
}
