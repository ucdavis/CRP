using System;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
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
    }
}
