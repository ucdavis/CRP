using System;
using System.Collections.Generic;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
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
        /// Tests the restricted key with long value saves.
        /// </summary>
        [TestMethod]
        public void TestRestrictedKeyWithLongValueSaves()
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
                results.AssertErrorsAre("Tags: may not be null");
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
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion CRUD Tests
        #endregion Tags Collection Tests
    }
}
