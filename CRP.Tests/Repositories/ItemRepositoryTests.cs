using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
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
    public class ItemRepositoryTests : AbstractRepositoryTests<Item, int>
    {
        protected IRepository<Item> ItemRepository { get; set; }


        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepositoryTests"/> class.
        /// </summary>
        public ItemRepositoryTests()
        {
            ItemRepository = new Repository<Item>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Item GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Item(counter);
            rtValue.Unit = Repository.OfType<Unit>().GetById(1);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Item> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Item>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Item entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Item entity, ARTAction action)
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
        /// Item Requires Unit
        /// Item Requires ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadRecords(5);
            Repository.OfType<Item>().DbContext.CommitTransaction();
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
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Name = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
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
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Name = string.Empty;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
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
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Name = " ";
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
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
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Name = "x".RepeatTimes(101);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(101, item.Name.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 100");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
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
            var item = GetValid(9);
            item.Name = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, item.Name.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Description Tests

        /// <summary>
        /// Tests the description with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Description = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }

        /// <summary>
        /// Tests the description with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Description = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Description = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, item.Description.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Description Tests

        #region CostPerItem Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCostPerItemValueOfNegativeOnePennyDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.CostPerItem = -0.01m;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(-0.01m, item.CostPerItem);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CostPerItem: must be zero or more");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the cost per item value of negative one hundred does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCostPerItemValueOfNegativeOneHundredDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.CostPerItem = -100m;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(-100m, item.CostPerItem);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CostPerItem: must be zero or more");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        
        #endregion Invalid Tests

        #region Valid Tests
               
        /// <summary>
        /// Tests the cost per item value of zero saves.
        /// </summary>
        [TestMethod]
        public void TestCostPerItemValueOfZeroSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.CostPerItem = 0;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the cost per item large value saves.
        /// </summary>
        [TestMethod]
        public void TestCostPerItemLargeValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.CostPerItem = 999999999999m;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion CostPerItem Tests

        #region Quantity Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityValueOfNegativeOneNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Quantity = -1;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(-1, item.Quantity);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Quantity: must be greater than or equal to 0");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the quantity with zero value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWithZeroValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Quantity = 0;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }
        /// <summary>
        /// Tests the quantity with large value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityWithLargeValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Quantity = int.MaxValue;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Quantity Tests

        #region QuantityName Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the quantity name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuantityNameWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.QuantityName = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(51, item.QuantityName.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityName: length must be between 0 and 50");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the quantity name with null value saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityNameWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.QuantityName = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the quantity name with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityNameWithSpacesOnlySaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.QuantityName = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the quantity name with fifty characters saves.
        /// </summary>
        [TestMethod]
        public void TestQuantityNameWithFiftyCharactersSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.QuantityName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, item.QuantityName.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion QuantityName Tests

        #region Expiration Tests

        /// <summary>
        /// Tests the expiration with null value saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Expiration = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(item.Expiration);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the expiration with current date saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithCurrentDateSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Expiration = DateTime.Now;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the expiration with current date saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithPastDateSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Expiration = DateTime.Now.AddMonths(-5);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the expiration with future date saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithFutureDateSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Expiration = DateTime.Now.AddMonths(5);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Expiration Tests

        #region Image Tests

        /// <summary>
        /// Tests the image with null value saves.
        /// </summary>
        [TestMethod]
        public void TestImageWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Image = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(item.Image);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the image with new value saves.
        /// </summary>
        [TestMethod]
        public void TestImageWithNewValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Image = new byte[0];
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.Image);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the image with value saves.
        /// </summary>
        [TestMethod]
        public void TestImageWithValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Image = new byte[]{1,2,3,4,5,6,7,8,9,1,2,3,4};
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.Image);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Image Tests

        #region Link Tests

        /// <summary>
        /// Tests the link with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLinkWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Link = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestLinkWithSpacesOnlySaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Link = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the link with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLinkWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Link = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the link with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLinkWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Link = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the link with large value saves.
        /// </summary>
        [TestMethod]
        public void TestLinkWithLargeValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Link = "x".RepeatTimes(2000);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Link Tests

        #region ItemType Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the item type with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTypeWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.ItemType = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemType: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }		
        }

        /// <summary>
        /// Tests the item type with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestItemTypeWithNewValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.ItemType = new ItemType();
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.ItemType, Entity: CRP.Core.Domain.ItemType", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Test

        /// <summary>
        /// Tests the item type saves with valid value.
        /// </summary>
        [TestMethod]
        public void TestItemTypeSavesWithValidValue()
        {
            #region Arrange
            LoadItemTypes(3);
            var item = GetValid(9);
            item.ItemType = Repository.OfType<ItemType>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.ItemType);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }
        #endregion Valid Test

        #endregion ItemType Tests
 
        #region Unit Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the unit with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUnitWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Unit = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Unit: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
       
        /// <summary>
        /// Tests the unit with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestUnitWithNewValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Unit = new Unit();
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Item.Unit", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        
        #region Valid Test

        /// <summary>
        /// Tests the unit saves with valid value.
        /// </summary>
        [TestMethod]
        public void TestUnitSavesWithValidValue()
        {
            #region Arrange
            LoadUnits(3);
            var item = GetValid(9);
            item.Unit = Repository.OfType<Unit>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.ItemType);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Test

        #endregion Unit Tests
        
        #region DateCreated Tests

        /// <summary>
        /// Tests the date created defaults to current date time.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedDefaultsToCurrentDateTime()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            var item = GetValid(9);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(fakeDate, item.DateCreated);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the date created with past date saves.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateSaves()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            var item = GetValid(9);
            item.DateCreated = fakeDate.AddYears(-2);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreNotEqual(fakeDate, item.DateCreated);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the date created with future date saves.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithFutureDateSaves()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 01, 01, 11, 01, 31);
            SystemTime.Now = () => fakeDate;
            var item = GetValid(9);
            item.DateCreated = DateTime.Now.AddYears(5);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreNotEqual(fakeDate, item.DateCreated);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        
        #endregion DateCreated Tests

        #region Available Tests
        /// <summary>
        /// Tests the available when true saves.
        /// </summary>
        [TestMethod]
        public void TestAvailableWhenTrueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = true;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the Available when false saves.
        /// </summary>
        [TestMethod]
        public void TestAvailableWhenFalseSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = false;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Available Tests

        #region Private Tests
        /// <summary>
        /// Tests the priavte when true saves.
        /// </summary>
        [TestMethod]
        public void TestPrivateWhenTrueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Private = true;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the Private when false saves.
        /// </summary>
        [TestMethod]
        public void TestPrivateWhenFalseSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Private = false;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Private Tests

        #region RestrictedKey Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the restricted key to long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRestrictedKeyToLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.RestrictedKey = "x".RepeatTimes(11);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RestrictedKey: length must be between 0 and 10");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the restricted key with null value saves.
        /// </summary>
        [TestMethod]
        public void TestRestrictedKeyWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.RestrictedKey = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the restricted key with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestRestrictedKeyWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.RestrictedKey = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the restricted key with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestRestrictedKeyWithSpacesOnlySaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.RestrictedKey = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the restricted key with laong value saves.
        /// </summary>
        [TestMethod]
        public void TestRestrictedKeyWithLaongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.RestrictedKey = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, item.RestrictedKey.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        
        #endregion RestrictedKey Tests

        #region MapLink Tests

        /// <summary>
        /// Tests the MapLink with null value saves.
        /// </summary>
        [TestMethod]
        public void TestMapLinkWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapLink = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapLink with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestMapLinkWithSpacesOnlySaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapLink = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapLink with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestMapLinkWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapLink = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapLink with one character saves.
        /// </summary>
        [TestMethod]
        public void TestMapLinkWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapLink = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapLink with large value saves.
        /// </summary>
        [TestMethod]
        public void TestMapLinkWithLargeValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapLink = "x".RepeatTimes(2000);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion MapLink Tests

        #region LinkLink Tests

        /// <summary>
        /// Tests the LinkLink with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLinkLinkWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.LinkLink = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LinkLink with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestLinkLinkWithSpacesOnlySaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.LinkLink = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LinkLink with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLinkLinkWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.LinkLink = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LinkLink with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLinkLinkWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.LinkLink = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LinkLink with large value saves.
        /// </summary>
        [TestMethod]
        public void TestLinkLinkWithLargeValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.LinkLink = "x".RepeatTimes(2000);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion LinkLink Tests

        #region Tags Collection Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the tags with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTagsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Tags = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Tags: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the tags with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestTagsWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Tags = new List<Tag>();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }
        /// <summary>
        /// Tests the tags for mapping problem.
        /// </summary>
        [TestMethod]
        public void TestTagsForMappingProblem()
        {
            #region Arrange
            Repository.OfType<Tag>().DbContext.BeginTransaction();
            var tags = CreateValidEntities.Tag(1);
            Repository.OfType<Tag>().EnsurePersistent(tags);
            Repository.OfType<Tag>().DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            
            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(Repository.OfType<Tag>().GetById(1));
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the tags creates tag item if it is not saved.
        /// </summary>
        [TestMethod]
        public void TestTagsCreatesTagItemIfItIsNotSaved()
        {
            #region Arrange
            var tags = CreateValidEntities.Tag(1);
            Assert.AreEqual(0, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(tags);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the tags does not remove tag from database when removed from list.
        /// </summary>
        [TestMethod]
        public void TestTagsDoesNotRemoveTagFromDatabaseWhenRemovedFromList()
        {
            #region Arrange
            Repository.OfType<Tag>().DbContext.BeginTransaction();
            var tags = CreateValidEntities.Tag(1);
            Repository.OfType<Tag>().EnsurePersistent(tags);
            Repository.OfType<Tag>().DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(Repository.OfType<Tag>().GetById(1));

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Arrange

            #region Act
            item.RemoveTag(Repository.OfType<Tag>().GetById(1));
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, item.Tags.Count);
            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the tags does not remove tag from database when removed from list even if it was saved by that item.
        /// </summary>
        [TestMethod]
        public void TestTagsDoesNotRemoveTagFromDatabaseWhenRemovedFromListEvenIfItWasSavedByThatItem()
        {
            #region Arrange

            var tags = CreateValidEntities.Tag(1);
            Assert.AreEqual(0, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(tags);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Arrange

            #region Act
            item.RemoveTag(Repository.OfType<Tag>().GetById(1));
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, item.Tags.Count);
            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the tags does not remove tag from database when item is removed.
        /// </summary>
        [TestMethod]
        public void TestTagsDoesNotRemoveTagFromDatabaseWhenItemIsRemoved()
        {
            #region Arrange
            Repository.OfType<Tag>().DbContext.BeginTransaction();
            var tags = CreateValidEntities.Tag(1);
            Repository.OfType<Tag>().EnsurePersistent(tags);
            Repository.OfType<Tag>().DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(Repository.OfType<Tag>().GetById(1));

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the when different tag is removed does not remove other tag.
        /// </summary>
        [TestMethod]
        public void TestWhenDifferentTagIsRemovedDoesNotRemoveOtherTag()
        {
            #region Arrange
            Repository.OfType<Tag>().DbContext.BeginTransaction();
            var tag1 = CreateValidEntities.Tag(1);
            var tag2 = CreateValidEntities.Tag(2);
            Repository.OfType<Tag>().EnsurePersistent(tag1);
            Repository.OfType<Tag>().EnsurePersistent(tag2);
            Repository.OfType<Tag>().DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(tag1);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<Tag>().GetAll().Count);
            Assert.AreEqual(1, item.Tags.Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Arrange

            #region Act
            item.RemoveTag(tag2); //Tag is not in list
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<Tag>().GetAll().Count);
            Assert.AreEqual(1, item.Tags.Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert		
        }


        /// <summary>
        /// Tests the two tags are added.
        /// </summary>
        [TestMethod]
        public void TestTwoTagsAreAdded()
        {
            #region Arrange
            var tag1 = CreateValidEntities.Tag(1);
            var tag2 = CreateValidEntities.Tag(2);
            Assert.AreEqual(0, Repository.OfType<Tag>().GetAll().Count);

            var item = GetValid(null);
            item.Tags = new List<Tag>();
            item.AddTag(tag2);
            item.AddTag(tag1);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, item.Tags.Count);
            Assert.AreEqual(2, Repository.OfType<Tag>().GetAll().Count);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }


        /// <summary>
        /// Tests the tags when tag not valid does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTagsWhenTagNotValidDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                var tag1 = CreateValidEntities.Tag(1);
                tag1.Name = null;
                item = GetValid(9);
                item.Tags = new List<Tag>();
                item.AddTag(tag1);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemTags: One or more tags is not valid");
                Assert.Inconclusive("Review if we want this message or use [Valid] on the tags collection. Need to see what it would look like in the UI");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }		
        }

        #endregion CRUD Tests
        #endregion Tags Collection Tests

        //TODO: Test that remove Editor from item cascades to editors.

        //TODO: Other tests



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
            expectedFields.Add(new NameAndType("Available", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("CostPerItem", "System.Decimal", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Expiration", "System.DateTime", new List<string>())); 
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Image", "System.Byte[]", new List<string>()));
            expectedFields.Add(new NameAndType("ItemType", "CRP.Core.Domain.ItemType", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Link", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LinkLink", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("MapLink", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]",
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Private", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Quantity", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuantityName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("RestrictedKey", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Tags", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Tag]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("Unit", "CRP.Core.Domain.Unit", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Item));

        }
        #endregion reflection
    }
}
