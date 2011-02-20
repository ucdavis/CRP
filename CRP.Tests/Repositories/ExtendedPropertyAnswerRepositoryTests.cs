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
    public class ExtendedPropertyAnswerRepositoryTests : AbstractRepositoryTests<ExtendedPropertyAnswer, int>
    {
        protected IRepository<ExtendedPropertyAnswer> ExtendedPropertyAnswerRepository { get; set; }

        
        #region Init and Overrides
        public ExtendedPropertyAnswerRepositoryTests()
        {
            ExtendedPropertyAnswerRepository = new Repository<ExtendedPropertyAnswer>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ExtendedPropertyAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ExtendedPropertyAnswer(counter);
            rtValue.ExtendedProperty = Repository.OfType<ExtendedProperty>().GetById(1);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ExtendedPropertyAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ExtendedPropertyAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ExtendedPropertyAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ExtendedPropertyAnswer entity, ARTAction action)
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
        /// ExtendedPropertyAnswer Requires an ExtendedProperty
        /// ExtendedProperty requires ItemTypes
        /// ExtendedProperty requires QuestionTypes
        /// ExtendedPropertyAnswer Requires Items
        /// Items requires Units and ItemTypes
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ExtendedPropertyAnswer>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);
            LoadUnits(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<ExtendedPropertyAnswer>().DbContext.CommitTransaction();
        }

        //private void LoadExtendedProperty(int entriesToAdd)
        //{
        //    for (int i = 0; i < entriesToAdd; i++)
        //    {
        //        var validEntity = CreateValidEntities.ExtendedProperty(entriesToAdd);
        //        validEntity.ItemType = Repository.OfType<ItemType>().GetById(1);
        //        validEntity.QuestionType = Repository.OfType<QuestionType>().GetById(1);
        //        Repository.OfType<ExtendedProperty>().EnsurePersistent(validEntity);
        //    }
        //}

        #endregion Init and Overrides

        #region Answer Tests

        [TestMethod, Ignore]
        public void TestExtendedPropertyAnswerWithNullAnswerSaves()
        {
            #region Arrange
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Answer = null;
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert		
        }

        [TestMethod, Ignore]
        public void TestExtendedPropertyAnswerWithEmptyStringAnswerSaves()
        {
            #region Arrange
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Answer = string.Empty;
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert
        }

        [TestMethod, Ignore]
        public void TestExtendedPropertyAnswerWithSpacesOnlyAnswerSaves()
        {
            #region Arrange
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Answer = " ";
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestExtendedPropertyAnswerWithOneCharacterAnswerSaves()
        {
            #region Arrange
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Answer = "x";
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestExtendedPropertyAnswerWithLongAnswerSaves()
        {
            #region Arrange
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Answer = "x".RepeatTimes(250);
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(250, extendedPropertyAnswer.Answer.Length);
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert
        }
        #endregion Answer Tests

        #region Item Tests

        #region InvalidTests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyAnswerWithNullItemDoesNotSave()
        {
            ExtendedPropertyAnswer extendedPropertyAnswer = null;
            try
            {
                #region Arrange
                extendedPropertyAnswer = GetValid(9);
                extendedPropertyAnswer.Item = null;
                #endregion Arrange

                #region Act
                ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
                ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
                ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedPropertyAnswer);
                var results = extendedPropertyAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be null");
                Assert.IsTrue(extendedPropertyAnswer.IsTransient());
                Assert.IsFalse(extendedPropertyAnswer.IsValid());
                #endregion Assert

                throw;
            }		
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestExtendedPropertyAnswerWithNewItemDoesNotSave()
        {
            ExtendedPropertyAnswer extendedPropertyAnswer = null;
            try
            {
                #region Arrange
                extendedPropertyAnswer = GetValid(9);
                extendedPropertyAnswer.Item = new Item();
                #endregion Arrange

                #region Act
                ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
                ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
                ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(extendedPropertyAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Item, Entity: CRP.Core.Domain.Item", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion InvalidTests

        #region Valid Test

        [TestMethod]
        public void TestExtendedPropertyAnswerWithValidDataSaves()
        {
            #region Arrange
            LoadItems(3);
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.Item = Repository.OfType<Item>().GetNullableById(3);
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert		
        }
        #endregion Valid Test
        #endregion Item Tests

        #region ExtendedProperty Tests

        #region InvalidTests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyAnswerWithNullExtendedPropertyDoesNotSave()
        {
            ExtendedPropertyAnswer extendedPropertyAnswer = null;
            try
            {
                #region Arrange
                extendedPropertyAnswer = GetValid(9);
                extendedPropertyAnswer.ExtendedProperty = null;
                #endregion Arrange

                #region Act
                ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
                ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
                ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedPropertyAnswer);
                var results = extendedPropertyAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ExtendedProperty: may not be null");
                Assert.IsTrue(extendedPropertyAnswer.IsTransient());
                Assert.IsFalse(extendedPropertyAnswer.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestExtendedPropertyAnswerWithNewExtendedPropertyDoesNotSave()
        {
            ExtendedPropertyAnswer extendedPropertyAnswer = null;
            try
            {
                #region Arrange
                extendedPropertyAnswer = GetValid(9);
                extendedPropertyAnswer.ExtendedProperty = new ExtendedProperty();
                #endregion Arrange

                #region Act
                ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
                ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
                ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(extendedPropertyAnswer);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.ExtendedPropertyAnswer.ExtendedProperty", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion InvalidTests

        #region Valid Test

        [TestMethod]
        public void TestExtendedPropertyAnswerWithValidExtendedPropertySaves()
        {
            #region Arrange
            LoadExtendedProperty(3);
            var extendedPropertyAnswer = GetValid(9);
            extendedPropertyAnswer.ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(3);
            #endregion Arrange

            #region Act
            ExtendedPropertyAnswerRepository.DbContext.BeginTransaction();
            ExtendedPropertyAnswerRepository.EnsurePersistent(extendedPropertyAnswer);
            ExtendedPropertyAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedPropertyAnswer.IsTransient());
            Assert.IsTrue(extendedPropertyAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Test
        #endregion Item Tests

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
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>{
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ExtendedProperty", "CRP.Core.Domain.ExtendedProperty", new List<string>{
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>{
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ExtendedPropertyAnswer));

        }



        #endregion Reflection of Database.
    }
}
