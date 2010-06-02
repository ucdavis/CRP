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
    /// Entity Name:		HelpTopic
    /// LookupFieldName:	Question
    /// </summary>
    [TestClass]
    public class HelpTopicRepositoryTests : AbstractRepositoryTests<HelpTopic, int>
    {
        /// <summary>
        /// Gets or sets the HelpTopic repository.
        /// </summary>
        /// <value>The HelpTopic repository.</value>
        public IRepository<HelpTopic> HelpTopicRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpTopicRepositoryTests"/> class.
        /// </summary>
        public HelpTopicRepositoryTests()
        {
            HelpTopicRepository = new Repository<HelpTopic>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override HelpTopic GetValid(int? counter)
        {
            return CreateValidEntities.HelpTopic(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<HelpTopic> GetQuery(int numberAtEnd)
        {
            return HelpTopicRepository.Queryable.Where(a => a.Question.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(HelpTopic entity, int counter)
        {
            Assert.AreEqual("Question" + counter, entity.Question);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(HelpTopic entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Question);
                    break;
                case ARTAction.Restore:
                    entity.Question = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Question;
                    entity.Question = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            HelpTopicRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            HelpTopicRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Question Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Question with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            HelpTopic helpTopic = null;
            try
            {
                #region Arrange
                helpTopic = GetValid(9);
                helpTopic.Question = null;
                #endregion Arrange

                #region Act
                HelpTopicRepository.DbContext.BeginTransaction();
                HelpTopicRepository.EnsurePersistent(helpTopic);
                HelpTopicRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(helpTopic);
                var results = helpTopic.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null or empty");
                Assert.IsTrue(helpTopic.IsTransient());
                Assert.IsFalse(helpTopic.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Question with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithEmptyStringDoesNotSave()
        {
            HelpTopic helpTopic = null;
            try
            {
                #region Arrange
                helpTopic = GetValid(9);
                helpTopic.Question = string.Empty;
                #endregion Arrange

                #region Act
                HelpTopicRepository.DbContext.BeginTransaction();
                HelpTopicRepository.EnsurePersistent(helpTopic);
                HelpTopicRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(helpTopic);
                var results = helpTopic.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null or empty");
                Assert.IsTrue(helpTopic.IsTransient());
                Assert.IsFalse(helpTopic.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Question with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithSpacesOnlyDoesNotSave()
        {
            HelpTopic helpTopic = null;
            try
            {
                #region Arrange
                helpTopic = GetValid(9);
                helpTopic.Question = " ";
                #endregion Arrange

                #region Act
                HelpTopicRepository.DbContext.BeginTransaction();
                HelpTopicRepository.EnsurePersistent(helpTopic);
                HelpTopicRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(helpTopic);
                var results = helpTopic.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null or empty");
                Assert.IsTrue(helpTopic.IsTransient());
                Assert.IsFalse(helpTopic.IsValid());
                throw;
            }
        }
   
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Question with one character saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithOneCharacterSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Question = "x";
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Question with long value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithLongValueSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Question = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, helpTopic.Question.Length);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Question Tests

        #region Answer Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Answer with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithNullValueSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Answer = null;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithEmptyStringSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Answer = string.Empty;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneSpaceSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Answer = " ";
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneCharacterSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Answer = "x";
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var helpTopic = GetValid(9);
            helpTopic.Answer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, helpTopic.Answer.Length);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Answer Tests

        #region AvailableToPublic Tests

        /// <summary>
        /// Tests the AvailableToPublic is false saves.
        /// </summary>
        [TestMethod]
        public void TestAvailableToPublicIsFalseSaves()
        {
            #region Arrange

            HelpTopic helpTopic = GetValid(9);
            helpTopic.AvailableToPublic = false;

            #endregion Arrange

            #region Act

            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(helpTopic.AvailableToPublic);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AvailableToPublic is true saves.
        /// </summary>
        [TestMethod]
        public void TestAvailableToPublicIsTrueSaves()
        {
            #region Arrange

            var helpTopic = GetValid(9);
            helpTopic.AvailableToPublic = true;

            #endregion Arrange

            #region Act

            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(helpTopic.AvailableToPublic);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());

            #endregion Assert
        }

        #endregion AvailableToPublic Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange

            HelpTopic helpTopic = GetValid(9);
            helpTopic.IsActive = false;

            #endregion Arrange

            #region Act

            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(helpTopic.IsActive);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange

            var helpTopic = GetValid(9);
            helpTopic.IsActive = true;

            #endregion Arrange

            #region Act

            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(helpTopic);
            HelpTopicRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(helpTopic.IsActive);
            Assert.IsFalse(helpTopic.IsTransient());
            Assert.IsTrue(helpTopic.IsValid());

            #endregion Assert
        }

        #endregion IsActive Tests

        #region NumberOfReads Tests

        /// <summary>
        /// Tests the NumberOfReads with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestNumberOfReadsWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NumberOfReads = int.MaxValue;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(record);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.NumberOfReads);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the NumberOfReads with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestNumberOfReadsWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NumberOfReads = int.MinValue;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(record);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.NumberOfReads);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the number of reads with zero value saves.
        /// </summary>
        [TestMethod]
        public void TestNumberOfReadsWithZeroValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NumberOfReads = 0;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(record);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.NumberOfReads);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the number of reads wit value of one saves.
        /// </summary>
        [TestMethod]
        public void TestNumberOfReadsWitValueOfOneSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NumberOfReads = 1;
            #endregion Arrange

            #region Act
            HelpTopicRepository.DbContext.BeginTransaction();
            HelpTopicRepository.EnsurePersistent(record);
            HelpTopicRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, record.NumberOfReads);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion NumberOfReads Tests

        #region Constructor Tests

        /// <summary>
        /// Tests the defaults are set as expected.
        /// </summary>
        [TestMethod]
        public void TestDefaultsAreSetAsExpected()
        {
            #region Arrange
            var record = new HelpTopic();           
            #endregion Arrange

            #region Assert
            Assert.IsFalse(record.AvailableToPublic);
            Assert.IsTrue(record.IsActive);
            Assert.AreEqual(0, record.NumberOfReads);
            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("AvailableToPublic", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("NumberOfReads", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Question", "System.String", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(HelpTopic));

        }

        #endregion Reflection of Database.	
		
		
    }
}