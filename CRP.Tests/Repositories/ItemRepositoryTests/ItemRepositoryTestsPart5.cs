using System;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
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
            item.Unit = Repository.OfType<Unit>().GetNullableById(3);
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
        /// Tests the private when true saves.
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
    }
}
