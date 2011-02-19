using System;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
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
            item.Image = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 };
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
            item.ItemType = Repository.OfType<ItemType>().GetNullableById(3);
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
    }
}
