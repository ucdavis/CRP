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
    public class ItemQuestionSetRepositoryTests : AbstractRepositoryTests<ItemQuestionSet, int >
    {
        protected IRepository<ItemQuestionSet> ItemQuestionSetRepository { get; set; }

        
        #region Init and Overrides
        public ItemQuestionSetRepositoryTests()
        {
            ItemQuestionSetRepository = new Repository<ItemQuestionSet>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemQuestionSet GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemQuestionSet(counter);
            rtValue.Item = Repository.OfType<Item>().GetByID(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetByID(1);
            if (counter != null && counter == 3)
            {
                rtValue.TransactionLevel = true;
            }
            else
            {
                rtValue.TransactionLevel = false;
            }
            rtValue.QuantityLevel = !rtValue.TransactionLevel;
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemQuestionSet> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemQuestionSet>().Queryable.Where(a => a.TransactionLevel);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemQuestionSet entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemQuestionSet entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.TransactionLevel);
                    break;
                case ARTAction.Restore:
                    entity.TransactionLevel = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.TransactionLevel;
                    entity.TransactionLevel = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// ItemQuestionSet Requires Item
        /// ItemQuestionSet Requires QuestionSet
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ItemQuestionSet>().DbContext.BeginTransaction();
            LoadItems(1);
            LoadQuestionSets(1);
            LoadRecords(5);
            Repository.OfType<ItemQuestionSet>().DbContext.CommitTransaction();
        }

        

        #endregion Init and Overrides

        #region Item Tests
        #region InValid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemQuestionSetWhenItemIsNullDoesNotSave()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.Item = null;
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemQuestionSet);
                var results = itemQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be empty");
                Assert.IsTrue(itemQuestionSet.IsTransient());
                Assert.IsFalse(itemQuestionSet.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestExtendedPropertyWhenItemIsNewDoesNotSave()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.Item = new Item();
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(itemQuestionSet);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.ItemQuestionSet.Item", ex.Message);
                #endregion Assert
                throw;
            }
        }

        #endregion InValid Tests

        #region Valid Tests

        [TestMethod]
        public void TestItemQuestionSetWithValidItemSaves()
        {
            #region Arrange
            LoadItems(3);
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Item Tests

        #region QuestionSet Tests

        #region InValid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemQuestionSetWhenQuestionSetIsNullDoesNotSave()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.QuestionSet = null;
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemQuestionSet);
                var results = itemQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSet: may not be empty");
                Assert.IsTrue(itemQuestionSet.IsTransient());
                Assert.IsFalse(itemQuestionSet.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyWhenQuestionSetIsNewDoesNotSave()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemQuestionSet);
                var results = itemQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre(
                    "Name: may not be null or empty",
                    "CollegeReusableSchool: Must have school if college reusable",
                    "Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(itemQuestionSet.IsTransient());
                Assert.IsFalse(itemQuestionSet.IsValid());
                throw;
            }
        }

        #endregion InValid Tests

        #region Valid Tests

        [TestMethod]
        public void TestItemQuestionSetWithValidQuestionSetSaves()
        {
            #region Arrange
            LoadQuestionSets(3);
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.QuestionSet = Repository.OfType<QuestionSet>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests
        #endregion Item Tests

        #region TransactionLevel Tests

        [TestMethod]
        public void TestTransactionLevelWhenTrueSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.TransactionLevel = true;
            itemQuestionSet.QuantityLevel = !itemQuestionSet.TransactionLevel;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the TransactionLevel when false saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionLevelWhenFalseSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.TransactionLevel = false;
            itemQuestionSet.QuantityLevel = !itemQuestionSet.TransactionLevel;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }
        
        #endregion TransactionLevel Tests

        #region QuantityLevel Tests
        [TestMethod]
        public void TestQuantityLevelWhenTrueSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.QuantityLevel = true;
            itemQuestionSet.TransactionLevel = !itemQuestionSet.QuantityLevel;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the TransactionLevel when false saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityLevelWhenFalseSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.QuantityLevel = false;
            itemQuestionSet.TransactionLevel = !itemQuestionSet.QuantityLevel;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }

        #endregion QuantityLevel Tests

        #region Order Tests

        [TestMethod]
        public void TestOrderOfZeroSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Order = 0;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert	
        }
        [TestMethod]
        public void TestOrderOfOneSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Order = 1;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderBigValueSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Order = 999999999;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }

        #endregion Order Tests
/*
        #region Required Tests
        [TestMethod]
        public void TestRequiredWhenTrueSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Required = true;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the Required when false saves.
        /// </summary>
        [TestMethod]
        public void TestRequiredWhenFalseSaves()
        {
            #region Arrange
            var itemQuestionSet = GetValid(9);
            itemQuestionSet.Required = false;
            #endregion Arrange

            #region Act
            ItemQuestionSetRepository.DbContext.BeginTransaction();
            ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
            ItemQuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemQuestionSet.IsTransient());
            Assert.IsTrue(itemQuestionSet.IsValid());
            #endregion Assert
        }
        #endregion Required Tests
*/
        #region TransactionLevelAndQuantityLevel Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionLevelAndQuantityLevelInvalidDoesNotSave1()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.TransactionLevel = true;
                itemQuestionSet.QuantityLevel = itemQuestionSet.TransactionLevel;
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemQuestionSet);
                var results = itemQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionLevelAndQuantityLevel: TransactionLevel or QuantityLevel must be set but not both.");
                Assert.IsTrue(itemQuestionSet.IsTransient());
                Assert.IsFalse(itemQuestionSet.IsValid());
                #endregion Assert
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionLevelAndQuantityLevelInvalidDoesNotSave2()
        {
            ItemQuestionSet itemQuestionSet = null;
            try
            {
                #region Arrange
                itemQuestionSet = GetValid(9);
                itemQuestionSet.TransactionLevel = false;
                itemQuestionSet.QuantityLevel = itemQuestionSet.TransactionLevel;
                #endregion Arrange

                #region Act
                ItemQuestionSetRepository.DbContext.BeginTransaction();
                ItemQuestionSetRepository.EnsurePersistent(itemQuestionSet);
                ItemQuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemQuestionSet);
                var results = itemQuestionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionLevelAndQuantityLevel: TransactionLevel or QuantityLevel must be set but not both.");
                Assert.IsTrue(itemQuestionSet.IsTransient());
                Assert.IsFalse(itemQuestionSet.IsValid());
                #endregion Assert
                throw;
            }
        }

        #endregion TransactionLevelAndQuantityLevel Tests

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
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityLevel", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionSet", "CRP.Core.Domain.QuestionSet", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]",
                "[NHibernate.Validator.Constraints.ValidAttribute()]"
            }));
            //expectedFields.Add(new NameAndType("Required", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionLevel", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionLevelAndQuantityLevel", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"TransactionLevel or QuantityLevel must be set but not both.\")]"
            }));

            


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ItemQuestionSet));

        }
        #endregion reflection
    }
}
