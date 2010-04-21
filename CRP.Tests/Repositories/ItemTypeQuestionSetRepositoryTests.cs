using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ItemTypeQuestionSetRepositoryTests : AbstractRepositoryTests<ItemTypeQuestionSet, int>
    {
        protected IRepository<ItemTypeQuestionSet> ItemTypeQuestionSetRepository { get; set; }
        
        #region Init and Overrides
        public ItemTypeQuestionSetRepositoryTests()
        {
            ItemTypeQuestionSetRepository = new Repository<ItemTypeQuestionSet>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemTypeQuestionSet GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemTypeQuestionSet(counter);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.QuantityLevel = true;
                rtValue.TransactionLevel = false;
            }
            else
            {
                rtValue.QuantityLevel = false;
                rtValue.TransactionLevel = true;
            }
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemTypeQuestionSet> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemTypeQuestionSet>().Queryable.Where(a => a.QuantityLevel);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemTypeQuestionSet entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemTypeQuestionSet entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.QuantityLevel);
                    break;
                case ARTAction.Restore:
                    entity.QuantityLevel = BoolRestoreValue;
                    entity.TransactionLevel = !BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.QuantityLevel;
                    entity.QuantityLevel = updateValue;
                    entity.TransactionLevel = !updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ItemTypeQuestionSet>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionSets(1);
            LoadRecords(5);
            Repository.OfType<ItemTypeQuestionSet>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can delete entity].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public override void CanDeleteEntity()
        {
            try
            {
                base.CanDeleteEntity();
            }
            catch(Exception ex)
            {
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.QuestionSet#1]", ex.Message);
                throw;
            }
           
        }

        #endregion CRUD Tests

        #region ItemType Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTypeWithNullValuesDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSet = null;
            try
            {
                #region Arrange
                itemTypeQuestionSet = GetValid(9);
                itemTypeQuestionSet.ItemType = null;
                #endregion Arrange

                #region Act
                ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
                ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
                ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemTypeQuestionSet);
                var results = itemTypeQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemType: may not be empty");
                Assert.IsTrue(itemTypeQuestionSet.IsTransient());
                Assert.IsFalse(itemTypeQuestionSet.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestWhenItemTypeIsNewDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSet = null;
            try
            {
                #region Arrange
                itemTypeQuestionSet = GetValid(9);
                itemTypeQuestionSet.ItemType = new ItemType();
                #endregion Arrange

                #region Act
                ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
                ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
                ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(itemTypeQuestionSet);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.ItemTypeQuestionSet.ItemType", ex.Message);
                #endregion Assert
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestItemTypeWithValidDataSaves()
        {
            #region Arrange
            LoadItemTypes(3);
            var itemTypeQuestionSet = GetValid(9);
            itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
            ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
            ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemTypeQuestionSet.IsTransient());
            Assert.IsTrue(itemTypeQuestionSet.IsValid());
            #endregion Assert	
        }
        #endregion Valid Tests
        #endregion ItemType Tests

        #region QuestionSet Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetWithNullValuesDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSet = null;
            try
            {
                #region Arrange
                itemTypeQuestionSet = GetValid(9);
                itemTypeQuestionSet.QuestionSet = null;
                #endregion Arrange

                #region Act
                ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
                ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
                ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemTypeQuestionSet);
                var results = itemTypeQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSet: may not be empty");
                Assert.IsTrue(itemTypeQuestionSet.IsTransient());
                Assert.IsFalse(itemTypeQuestionSet.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestWhenQuestionSetIsNewDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSet = null;
            try
            {
                Assert.AreEqual(1, Repository.OfType<QuestionSet>().GetAll().Count);
                #region Arrange
                itemTypeQuestionSet = GetValid(9);
                itemTypeQuestionSet.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
                ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
                ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemTypeQuestionSet);
                var results = itemTypeQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemTypeQuestionSetQuestionSet: QuestionSet not valid");
                    //"Name: may not be null or empty",
                    //"CollegeReusableSchool: Must have school if college reusable",
                    //"Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(itemTypeQuestionSet.IsTransient());
                Assert.IsFalse(itemTypeQuestionSet.IsValid());
                #endregion Assert
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestQuestionSetWithValidDataSaves()
        {
            #region Arrange
            LoadQuestionSets(3);
            var itemTypeQuestionSet = GetValid(9);
            itemTypeQuestionSet.QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
            ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
            ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemTypeQuestionSet.IsTransient());
            Assert.IsTrue(itemTypeQuestionSet.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion QuestionSet Tests

        #region TransactionLevelQuantityLevel Tests

        #region InValid Tests Tests

        /// <summary>
        /// Tests the item type question set where quantity level and transaction level are both true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTypeQuestionSetWhereQuantityLevelAndTransactionLevelAreBothTrueDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSetRecord = null;
            try
            {
                itemTypeQuestionSetRecord = CreateValidEntities.ItemTypeQuestionSet(null);
                itemTypeQuestionSetRecord.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
                itemTypeQuestionSetRecord.ItemType = Repository.OfType<ItemType>().GetById(1);
                itemTypeQuestionSetRecord.QuantityLevel = true;
                itemTypeQuestionSetRecord.TransactionLevel = true;


                Repository.OfType<ItemTypeQuestionSet>().EnsurePersistent(itemTypeQuestionSetRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemTypeQuestionSetRecord);
                var results = itemTypeQuestionSetRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionLevelQuantityLevel: TransactionLevel must be different from QuantityLevel");
                Assert.IsTrue(itemTypeQuestionSetRecord.IsTransient());
                Assert.IsFalse(itemTypeQuestionSetRecord.IsValid());

                throw;
            }
        }

        /// <summary>
        /// Tests the item type question set where quantity level and transaction level are both false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTypeQuestionSetWhereQuantityLevelAndTransactionLevelAreBothFalseDoesNotSave()
        {
            ItemTypeQuestionSet itemTypeQuestionSetRecord = null;
            try
            {
                itemTypeQuestionSetRecord = CreateValidEntities.ItemTypeQuestionSet(null);
                itemTypeQuestionSetRecord.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
                itemTypeQuestionSetRecord.ItemType = Repository.OfType<ItemType>().GetById(1);
                itemTypeQuestionSetRecord.QuantityLevel = false;
                itemTypeQuestionSetRecord.TransactionLevel = false;


                Repository.OfType<ItemTypeQuestionSet>().EnsurePersistent(itemTypeQuestionSetRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemTypeQuestionSetRecord);
                var results = itemTypeQuestionSetRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionLevelQuantityLevel: TransactionLevel must be different from QuantityLevel");
                Assert.IsTrue(itemTypeQuestionSetRecord.IsTransient());
                Assert.IsFalse(itemTypeQuestionSetRecord.IsValid());

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestTransactionLevelIsTrueAndQuantityLevelIsFalseSaves()
        {
            #region Arrange
            LoadQuestionSets(3);
            var itemTypeQuestionSet = GetValid(9);
            itemTypeQuestionSet.TransactionLevel = true;
            itemTypeQuestionSet.QuantityLevel = false;
            #endregion Arrange

            #region Act
            ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
            ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
            ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemTypeQuestionSet.IsTransient());
            Assert.IsTrue(itemTypeQuestionSet.IsValid());
            #endregion Assert	
        }

        [TestMethod]
        public void TestTransactionLevelIsFalseAndQuantityLevelIsTrueSaves()
        {
            #region Arrange
            LoadQuestionSets(3);
            var itemTypeQuestionSet = GetValid(9);
            itemTypeQuestionSet.TransactionLevel = false;
            itemTypeQuestionSet.QuantityLevel = true;
            #endregion Arrange

            #region Act
            ItemTypeQuestionSetRepository.DbContext.BeginTransaction();
            ItemTypeQuestionSetRepository.EnsurePersistent(itemTypeQuestionSet);
            ItemTypeQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemTypeQuestionSet.IsTransient());
            Assert.IsTrue(itemTypeQuestionSet.IsValid());
            #endregion Assert
        }
        
        #endregion Valid Tests


        #endregion TransactionLevelQuantityLevel Tests

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
            expectedFields.Add(new NameAndType("ItemTypeQuestionSetQuestionSet", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"QuestionSet not valid\")]"
            }));
            expectedFields.Add(new NameAndType("QuantityLevel", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionSet", "CRP.Core.Domain.QuestionSet", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            expectedFields.Add(new NameAndType("TransactionLevel", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionLevelQuantityLevel", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"TransactionLevel must be different from QuantityLevel\")]"
            }));

            
            

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ItemTypeQuestionSet));

        }
        #endregion reflection
    }
}
