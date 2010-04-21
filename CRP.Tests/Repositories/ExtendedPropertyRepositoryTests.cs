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
    public class ExtendedPropertyRepositoryTests : AbstractRepositoryTests<ExtendedProperty, int>
    {
        protected IRepository<ExtendedProperty> ExtendedPropertyRepository { get; set; }

        
        #region Init and Overrides
        public ExtendedPropertyRepositoryTests()
        {
            ExtendedPropertyRepository = new Repository<ExtendedProperty>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ExtendedProperty GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ExtendedProperty(counter);
            rtValue.ItemType = Repository.OfType<ItemType>().GetByID(1);
            rtValue.QuestionType = Repository.OfType<QuestionType>().GetByID(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ExtendedProperty> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ExtendedProperty>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ExtendedProperty entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ExtendedProperty entity, ARTAction action)
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
        /// ExtendedProperty Requires ItemType
        /// ExtendedProperty Requires QuestionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ExtendedProperty>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionTypes(1);
            LoadRecords(5);
            Repository.OfType<ExtendedProperty>().DbContext.CommitTransaction();
        }


        #endregion Init and Overrides

        #region ItemType Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWhenItemTypeIsNullDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.ItemType = null;
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemType: may not be empty");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestExtendedPropertyWhenItemTypeIsNewDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.ItemType = new ItemType();
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.ExtendedProperty.ItemType", ex.Message);
                #endregion Assert
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Test
        [TestMethod]
        public void TestDifferentItemSaves()
        {
            #region Arrange
            LoadItemTypes(3);
            var extendedProperty = GetValid(9);
            extendedProperty.ItemType = Repository.OfType<ItemType>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert	
        }

        #endregion Valid Test
        
        #endregion ItemType Tests

        #region Name Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWithNullNameDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.Name = null;
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWithEmptyNameDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.Name = string.Empty;
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWithSpacesOnlyNameDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.Name = " ";
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWithTooManyCharactersInNameDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.Name = "x".RepeatTimes(101);
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                Assert.AreEqual(101, extendedProperty.Name.Length);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 100");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }
        }
 
        #endregion Invalid Tests

        #region Valid Tests
        [TestMethod]
        public void TestExtendedPropertyWithOneCharacterInNameSaves()
        {
            #region Arrange

            var extendedProperty = GetValid(9);
            extendedProperty.Name = "x";
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestExtendedPropertyWithOneHundredCharactersInNameSaves()
        {
            #region Arrange

            var extendedProperty = GetValid(9);
            extendedProperty.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, extendedProperty.Name.Length);
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion Name Tests

        #region QuestionType Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWhenQuestionTypeIsNullDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.QuestionType = null;
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                var results = extendedProperty.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionType: may not be empty");
                Assert.IsTrue(extendedProperty.IsTransient());
                Assert.IsFalse(extendedProperty.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestExtendedPropertyWhenQuestionTypeIsNewDoesNotSave()
        {
            ExtendedProperty extendedProperty = null;
            try
            {
                #region Arrange
                extendedProperty = GetValid(9);
                extendedProperty.QuestionType = new QuestionType();
                #endregion Arrange

                #region Act
                ExtendedPropertyRepository.DbContext.BeginTransaction();
                ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
                ExtendedPropertyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(extendedProperty);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.ExtendedProperty.QuestionType", ex.Message);
                #endregion Assert
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Test
        [TestMethod]
        public void TestExtendedPropertyWithValidQuestionTypeSaves()
        {
            #region Arrange
            LoadQuestionTypes(3);
            var extendedProperty = GetValid(9);
            extendedProperty.QuestionType = Repository.OfType<QuestionType>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }

        #endregion Valid Test
        #endregion QuestionType Tests

        #region Order Tests

        #region Valid Tests
        [TestMethod]
        public void TestExtendedPropertyWhenOrderIsZeroSaves()
        {
            #region Arrange
            var extendedProperty = GetValid(9);
            extendedProperty.Order = 0;
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestExtendedPropertyWhenOrderIsOneSaves()
        {
            #region Arrange
            var extendedProperty = GetValid(9);
            extendedProperty.Order = 1;
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestExtendedPropertyWhenOrderIsLargeSaves()
        {
            #region Arrange
            var extendedProperty = GetValid(9);
            extendedProperty.Order = 999999999;
            #endregion Arrange

            #region Act
            ExtendedPropertyRepository.DbContext.BeginTransaction();
            ExtendedPropertyRepository.EnsurePersistent(extendedProperty);
            ExtendedPropertyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(extendedProperty.IsTransient());
            Assert.IsTrue(extendedProperty.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        
        #endregion Order Testss
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
            
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
           
            expectedFields.Add(new NameAndType("ItemType", "CRP.Core.Domain.ItemType", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]",
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]",
                "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionType", "CRP.Core.Domain.QuestionType", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]",
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

           
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ExtendedProperty));

        }



        #endregion Reflection of Database.
    }
}
