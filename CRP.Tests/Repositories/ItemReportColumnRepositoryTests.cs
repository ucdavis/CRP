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
    public class ItemReportColumnRepositoryTests : AbstractRepositoryTests<ItemReportColumn, int>
    {
        protected IRepository<ItemReportColumn> ItemReportColumnRepository { get; set; }

        
        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReportColumnRepositoryTests"/> class.
        /// </summary>
        public ItemReportColumnRepositoryTests()
        {
            ItemReportColumnRepository = new Repository<ItemReportColumn>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemReportColumn GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemReportColumn(counter);
            rtValue.ItemReport = Repository.OfType<ItemReport>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemReportColumn> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemReportColumn>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemReportColumn entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemReportColumn entity, ARTAction action)
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
            Repository.OfType<ItemReportColumn>().DbContext.BeginTransaction();
            LoadUsers(1);
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadItemReport(1);
            LoadRecords(5);
            Repository.OfType<ItemReportColumn>().DbContext.CommitTransaction();
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
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Name = null;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
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
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Name = string.Empty;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
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
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Name = " ";
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Name = "x".RepeatTimes(201);
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                Assert.AreEqual(201, itemReportColumn.Name.Length);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
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
            var itemReportColumn = GetValid(9);
            itemReportColumn.Name = "x";
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert				
        }

        /// <summary>
        /// Tests the name with two hundred character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithTwoHundredCharacterSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, itemReportColumn.Name.Length);
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Name Tests

        #region Format Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFormatWithTooLongValueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Format = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                Assert.AreEqual(51, itemReportColumn.Format.Length);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Format: length must be between 0 and 50");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }		
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the format with null value saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithNullValueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Format = null;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the format with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithEmptyStringSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Format = string.Empty;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestFormatWithSpacesOnlySaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Format = " ";
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the format with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithLongValueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Format = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, itemReportColumn.Format.Length);
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Format Tests

        #region ItemReport Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the item report with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemReportWithNullValueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.ItemReport = null;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemReport: may not be empty");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }		
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestItemReportWithNewValueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.ItemReport = new ItemReport();
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(itemReportColumn);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.ItemReport, Entity: CRP.Core.Domain.ItemReport", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the item report with valid value saves.
        /// </summary>
        [TestMethod]
        public void TestItemReportWithValidValueSaves()
        {
            #region Arrange
            LoadItemReport(3);
            var itemReportColumn = GetValid(9);
            itemReportColumn.ItemReport = Repository.OfType<ItemReport>().GetNullableById(3);
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert	
        }
        #endregion Valid Tests

        #endregion ItemReport Tests

        #region Order Tests

        [TestMethod]
        public void TestOrderOfZeroSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Order = 0;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestOrderOfOneSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Order = 1;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderBigValueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Order = 999999999;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        #endregion Order Tests

        #region Quantity Tests
        /// <summary>
        /// Tests the quantity when true saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWhenTrueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = true;
            itemReportColumn.Property = false;
            itemReportColumn.Transaction = false;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the quantity when false saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWhenFalseSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = false;
            itemReportColumn.Property = true;
            itemReportColumn.Transaction = false;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        #endregion Quantity Tests

        #region Transaction Tests
        /// <summary>
        /// Tests the Transaction when true saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionWhenTrueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = false;
            itemReportColumn.Property = false;
            itemReportColumn.Transaction = true;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Transaction when false saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionWhenFalseSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = false;
            itemReportColumn.Property = true;
            itemReportColumn.Transaction = false;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        #endregion Transaction Tests

        #region Property Tests
        /// <summary>
        /// Tests the Property when true saves.
        /// </summary>
        [TestMethod]
        public void TestPropertyWhenTrueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = false;
            itemReportColumn.Property = true;
            itemReportColumn.Transaction = false;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Property when false saves.
        /// </summary>
        [TestMethod]
        public void TestPropertyWhenFalseSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.Quantity = true;
            itemReportColumn.Property = false;
            itemReportColumn.Transaction = false;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert
        }

        #endregion Property Tests

        #region QuestionSet Tests

        #region Invalid Test
        /// <summary>
        /// Tests the question set with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionSetWithNewValueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.QuestionSet = new QuestionSet();
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(itemReportColumn);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.QuestionSet, Entity: CRP.Core.Domain.QuestionSet", ex.Message);
                throw;
            }
        }
        
        #endregion Invalid Test

        #region Valid Test
        
        /// <summary>
        /// Tests the question set with null value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetWithNullValueSaves()
        {
            #region Arrange
            var itemReportColumn = GetValid(9);
            itemReportColumn.QuestionSet = null;
            #endregion Arrange

            #region Act
            ItemReportColumnRepository.DbContext.BeginTransaction();
            ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
            ItemReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReportColumn.IsValid());
            #endregion Assert		
        }

        #endregion Valid Test

        #endregion QuestionSet Tests

        #region QuantityAndTransactionAndProperty Tests

        /// <summary>
        /// Tests the quantity and transaction and property when all are false does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAndTransactionAndPropertyWhenAllAreFalseDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Transaction = false;
                itemReportColumn.Property = false;
                itemReportColumn.Quantity = false;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityAndTransactionAndProperty: One and only one of these must be selected: Quantity, Transaction, Property");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }		
        }

        /// <summary>
        /// Tests the quantity and transaction and property when all are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAndTransactionAndPropertyWhenAllAreTrueDoesNotSave()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Transaction = true;
                itemReportColumn.Property = true;
                itemReportColumn.Quantity = true;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityAndTransactionAndProperty: One and only one of these must be selected: Quantity, Transaction, Property");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the quantity and transaction and property when two are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAndTransactionAndPropertyWhenTwoAreTrueDoesNotSave1()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Transaction = false;
                itemReportColumn.Property = true;
                itemReportColumn.Quantity = true;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityAndTransactionAndProperty: One and only one of these must be selected: Quantity, Transaction, Property");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the quantity and transaction and property when two are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAndTransactionAndPropertyWhenTwoAreTrueDoesNotSave2()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Transaction = true;
                itemReportColumn.Property = false;
                itemReportColumn.Quantity = true;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityAndTransactionAndProperty: One and only one of these must be selected: Quantity, Transaction, Property");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the quantity and transaction and property when two are true does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityAndTransactionAndPropertyWhenTwoAreTrueDoesNotSave3()
        {
            ItemReportColumn itemReportColumn = null;
            try
            {
                #region Arrange
                itemReportColumn = GetValid(9);
                itemReportColumn.Transaction = true;
                itemReportColumn.Property = true;
                itemReportColumn.Quantity = false;
                #endregion Arrange

                #region Act
                ItemReportColumnRepository.DbContext.BeginTransaction();
                ItemReportColumnRepository.EnsurePersistent(itemReportColumn);
                ItemReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReportColumn);
                var results = itemReportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityAndTransactionAndProperty: One and only one of these must be selected: Quantity, Transaction, Property");
                Assert.IsTrue(itemReportColumn.IsTransient());
                Assert.IsFalse(itemReportColumn.IsValid());
                throw;
            }
        }

        #endregion QuantityAndTransactionAndProperty Tests

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

            expectedFields.Add(new NameAndType("Format", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ItemReport", "CRP.Core.Domain.ItemReport", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]",
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
                 
            }));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Property", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Quantity", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityAndTransactionAndProperty", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One and only one of these must be selected: Quantity, Transaction, Property\")]",     
            }));
            expectedFields.Add(new NameAndType("QuestionSet", "CRP.Core.Domain.QuestionSet", new List<string>()));            
            expectedFields.Add(new NameAndType("Transaction", "System.Boolean", new List<string>()));


            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ItemReportColumn));

        }
        #endregion reflection
    }
}
