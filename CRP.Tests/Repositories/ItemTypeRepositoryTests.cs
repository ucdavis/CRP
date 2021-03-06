﻿using System;
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
    /// ItemType Repository Tests
    /// </summary>
    [TestClass]
    public class ItemTypeRepositoryTests : AbstractRepositoryTests<ItemType, int >
    {
        /// <summary>
        /// Gets or sets the item type repository.
        /// </summary>
        /// <value>The item type repository.</value>
        public IRepository<ItemType> ItemTypeRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypeRepositoryTests"/> class.
        /// </summary>
        public ItemTypeRepositoryTests()
        {
            ItemTypeRepository = new Repository<ItemType>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemType GetValid(int? counter)
        {
            return CreateValidEntities.ItemType(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemType> GetQuery(int numberAtEnd)
        {
            return ItemTypeRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemType entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemType entity, ARTAction action)
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
            ItemTypeRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Validation Tests

        #region Name Tests

        #region Invalid Name Tests

        /// <summary>
        /// Tests the name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.Name = null;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with spaces only value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.Name = " ";

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with empty value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.Name = string.Empty;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
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
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.Name = "X".RepeatTimes(51);
                Assert.AreEqual(51, itemType.Name.Length, "Name must be 51 characters for this test.");

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        #endregion Invalid Name Tests

        #region Valid Name Tests

        /// <summary>
        /// Tests the name with 50 characters saves.
        /// </summary>
        [TestMethod]
        public void TestNameWith50CharactersSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.Name = "X".RepeatTimes(50);
            Assert.AreEqual(50, itemType.Name.Length, "Name must be 50 characters for this test.");

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Tests the name with 1 character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWith1CharacterSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.Name = "X";
            Assert.AreEqual(1, itemType.Name.Length, "Name must be 1 characters for this test.");

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        #endregion Valid Name Tests

        #endregion Name Tests

        #region IsActive Valid Tests

        /// <summary>
        /// Tests the is active is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.IsActive = true;

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Tests the is active is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.IsActive = false;

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        #endregion IsActive Valid Tests

        #region ExtendedProperties Tests

        #region Invalid ExtendedProperties Tests

        /// <summary>
        /// Tests the extended properties with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertiesWithNullValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.ExtendedProperties = null;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ExtendedProperties: may not be null");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        #endregion Invalid ExtendedProperties Tests

        #region Valid ExtendedProperties Tests

        /// <summary>
        /// Tests the extended property is new saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyIsNewSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.ExtendedProperties = new List<ExtendedProperty>();

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }
        #endregion Valid ExtendedProperties Tests

        #region CRUD Cascade Tests

        [TestMethod]
        public void TestExtendedPropertiesAreCreatedWhenItemTypeSaves()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes(1);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(1));
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(2));
            itemType.ExtendedProperties.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetById(1);
            itemType.ExtendedProperties.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetById(1);
            Assert.AreEqual(0, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Arrange

            #region Act
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveExtendedPropertyCascadesWhenItemTypeSaves()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes(1);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(1));
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(2));
            itemType.ExtendedProperties.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetById(1);
            itemType.ExtendedProperties.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetById(1);

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
  
            Assert.AreEqual(2, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Arrange

            #region Act
            itemType.ExtendedProperties.Remove(itemType.ExtendedProperties.ElementAt(0));
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, itemType.ExtendedProperties.Count);
            Assert.AreEqual(1, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveItemTypeCascadesToExtendedProperty()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes(1);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(1));
            itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(2));
            itemType.ExtendedProperties.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetById(1);
            itemType.ExtendedProperties.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetById(1);

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Arrange

            #region Act
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.Remove(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, Repository.OfType<ExtendedProperty>().GetAll().Count());
            #endregion Assert
        }

        #endregion CRUD Cascade Tests

        #endregion ExtendedProperties Tests

        #region QuestionSets Tests

        #region Invalid QuestionSets Tests

        /// <summary>
        /// Tests the question sets with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithNullValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.QuestionSets = null;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSets: may not be null");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        #endregion Invalid QuestionSets Tests

        #region ValidQuestionSets Tests

        /// <summary>
        /// Tests the question set with new value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetWithNewValueSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.QuestionSets = new List<ItemTypeQuestionSet>();

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }



        #endregion ValidQuestionSets Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the question sets with populated values saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithPopulatedValuesSaves1()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(2));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<QuestionSet>().GetAll().Count());
        }

        /// <summary>
        /// Tests the question sets with populated values saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithPopulatedValuesSaves2()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<QuestionSet>().GetAll().Count());
        }

        /// <summary>
        /// Tests the question sets with populated values saves.
        /// Both Transaction and Quantity can be populated at the same time.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithPopulatedValuesSaves3()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();

            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count());
            Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(1).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(1).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(2).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(2).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(3).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(3).QuantityLevel);
        }

        [TestMethod]
        public void TestRemoveQuestionSetCascadesToQuestionSets1()
        {
            #region Arrange
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            var itemTypeQuestionSets = Repository.OfType<ItemTypeQuestionSet>().GetAll().ToList();
            Assert.AreEqual(4, itemTypeQuestionSets.Count);
            #endregion Arrange

            #region Act
            itemType.RemoveQuestionSet(itemTypeQuestionSets[0]);
            //itemType.QuestionSets.Remove(itemTypeQuestionSets[0]);
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<QuestionSet>().GetAll().Count());
            //Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            //Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(1).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(1).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(2).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(2).QuantityLevel);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveQuestionSetCascadesToQuestionSets2()
        {
            #region Arrange
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            var questionSets = Repository.OfType<QuestionSet>().GetAll().ToList();
            Assert.AreEqual(4, questionSets.Count);
            #endregion Arrange

            #region Act
            itemType.RemoveQuestionSet(questionSets[0]);
            //itemType.QuestionSets.Remove(itemTypeQuestionSets[0]);
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<QuestionSet>().GetAll().Count());
            //Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            //Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(1).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(1).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(2).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(2).QuantityLevel);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveQuestionSetCascadesToQuestionSets3()
        {
            #region Arrange
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            var questionSets = Repository.OfType<QuestionSet>().GetAll().ToList();
            Assert.AreEqual(4, questionSets.Count);
            #endregion Arrange

            #region Act
            itemType.RemoveQuestionSet(CreateValidEntities.QuestionSet(99)); //not found, ignore

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count());
            Assert.IsTrue(itemType.QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(0).QuantityLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(1).TransactionLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(1).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(2).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(2).QuantityLevel);
            Assert.IsFalse(itemType.QuestionSets.ElementAt(3).TransactionLevel);
            Assert.IsTrue(itemType.QuestionSets.ElementAt(3).QuantityLevel);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveItemTypeCascadesToQuestionSets()
        {
            #region Arrange
            var itemType = CreateValidEntities.ItemType(null);
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
            itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
            itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            var questionSets = Repository.OfType<QuestionSet>().GetAll().ToList();
            Assert.AreEqual(4, questionSets.Count);
            #endregion Arrange

            #region Act
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.Remove(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, Repository.OfType<QuestionSet>().GetAll().Count());
            #endregion Assert
        }

        #endregion CRUD Tests

        #endregion QuestionSets Tests

        #region Items Tests

        #region Invalid Items Tests

        /// <summary>
        /// Tests the items with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemsWithNullValueDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.Items = null;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Items: may not be null");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        #endregion Invalid Items Tests

        #region Valid Items Tests

        /// <summary>
        /// Tests the items is new saves.
        /// </summary>
        [TestMethod]
        public void TestItemsIsNewSaves()
        {
            var itemType = CreateValidEntities.ItemType(null);
            itemType.Items = new List<Item>();

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Tests the items is populated saves.
        /// </summary>
        [TestMethod]
        public void TestItemsIsPopulatedSaves()
        {
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(2);
            Repository.OfType<Item>().DbContext.CommitTransaction();

            var itemType = CreateValidEntities.ItemType(null);
            itemType.Items.Add(Repository.OfType<Item>().GetById(1));
            itemType.Items.Add(Repository.OfType<Item>().GetById(2));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction(); 

            Assert.AreEqual(2, Repository.OfType<Item>().GetAll().Count());
        }

        #endregion Valid Items Tests

        #region CRUD Tests


        [TestMethod]
        public void TestRemoveItemsDoesNotCascade()
        {
            #region Arrange
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(4);
            Repository.OfType<Item>().DbContext.CommitTransaction();

            var itemType = CreateValidEntities.ItemType(null);
            itemType.Items.Add(Repository.OfType<Item>().GetById(1));
            itemType.Items.Add(Repository.OfType<Item>().GetById(2));
            itemType.Items.Add(Repository.OfType<Item>().GetById(4));

            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, itemType.Items.Count);
            Assert.AreEqual(4, Repository.OfType<Item>().GetAll().Count());
            #endregion Arrange

            #region Act
            itemType.Items.Remove(Repository.OfType<Item>().GetById(2));
            ItemTypeRepository.DbContext.BeginTransaction();
            ItemTypeRepository.EnsurePersistent(itemType);
            ItemTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, itemType.Items.Count);
            Assert.AreEqual(4, Repository.OfType<Item>().GetAll().Count());
            #endregion Assert		
        }
        #endregion CRUD Tests
        #endregion Items Tests

        #region ItemTypeExtendedProperties Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestWhenExtendedPropertyHasInvalidValueItemTypeDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                Repository.OfType<QuestionType>().DbContext.BeginTransaction();
                LoadQuestionTypes(1);
                Repository.OfType<QuestionType>().DbContext.CommitTransaction();
                itemType = CreateValidEntities.ItemType(null);
                itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(1));
                itemType.AddExtendedProperty(CreateValidEntities.ExtendedProperty(2));
                itemType.ExtendedProperties.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetById(1);
                itemType.ExtendedProperties.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetById(1);

                itemType.ExtendedProperties.ElementAt(0).Name = " ";

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemTypeExtendedProperties: One or more Extended Properties is not valid");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }	
        }

        #endregion ItemTypeExtendedProperties Tests

        #region ItemTypeQuestionSets Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestWhenQuestionSetsHasInvalidValueItemTypeDoesNotSave()
        {
            ItemType itemType = null;
            try
            {
                itemType = CreateValidEntities.ItemType(null);
                itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(1));
                itemType.AddTransactionQuestionSet(CreateValidEntities.QuestionSet(2));
                itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(3));
                itemType.AddQuantityQuestionSet(CreateValidEntities.QuestionSet(4));

                itemType.QuestionSets.ElementAt(1).QuantityLevel = true;

                ItemTypeRepository.DbContext.BeginTransaction();
                ItemTypeRepository.EnsurePersistent(itemType);
                ItemTypeRepository.DbContext.CommitTransaction();

            }
            catch (Exception)
            {
                Assert.IsNotNull(itemType);
                var results = itemType.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemTypeQuestionSets: One or more Question Sets is not valid");
                Assert.IsTrue(itemType.IsTransient());
                Assert.IsFalse(itemType.IsValid());
                throw;
            }
        }

        #endregion ItemTypeQuestionSets Tests

        #endregion Validation Tests


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
            expectedFields.Add(new NameAndType("ExtendedProperties", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ExtendedProperty]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Items", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Item]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ItemTypeExtendedProperties", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more Extended Properties is not valid\")]"
            }));
            expectedFields.Add(new NameAndType("ItemTypeQuestionSets", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more Question Sets is not valid\")]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuestionSets", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ItemTypeQuestionSet]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ItemType));

        }
        #endregion reflection
    }
}
