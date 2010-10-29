using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
        #region MapPin Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the MapPins with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMapPinsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.MapPins = null;
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
                results.AssertErrorsAre("MapPins: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the MapPins with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMapPinsWithMoreThanOnePrimaryValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.AddMapPin(new MapPin(item, true, "Lat", "Long"));
                item.AddMapPin(new MapPin(item, true, "Lat2", "Long2"));
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
                results.AssertErrorsAre("MapPinPrimary: Only 1 MapPin can be Primary");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the MapPins with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestsMapPinsWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.MapPins = new List<MapPin>();
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
        /// Tests the MapPins with value saves.
        /// </summary>
        [TestMethod]
        public void TestMapPinsWithValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.AddMapPin(new MapPin(item, true, "Lat", "Long"));
            item.MapPins.ElementAt(0).Title = "Title";
            item.MapPins.ElementAt(0).Description = "Description";
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

        #region CRUD Tests
        /// <summary>
        /// Tests the MapPins with several values saves.
        /// </summary>
        [TestMethod]
        public void TestMapPinsWithSeveralValuesSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.AddMapPin(new MapPin(item, true, "Lat", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat2", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat3", "Long"));
            Assert.AreEqual(0, Repository.OfType<MapPin>().GetAll().Count);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(3, Repository.OfType<MapPin>().GetAll().Count);
            Assert.AreEqual(3, item.MapPins.Count);
            foreach (var mapPin in Repository.OfType<MapPin>().GetAll())
            {
                Assert.AreSame(item, mapPin.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapPins cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestMapPinsCascadesWithRemove()
        {
            #region Arrange
            var item = GetValid(9);
            item.AddMapPin(new MapPin(item, true, "Lat", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat2", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat3", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat4", "Long"));
            Assert.AreEqual(0, Repository.OfType<MapPin>().GetAll().Count);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            Assert.AreEqual(4, Repository.OfType<MapPin>().GetAll().Count);
            #endregion Arrange

            #region Act
            item.RemoveMapPin(item.MapPins.ElementAt(3));
            item.RemoveMapPin(item.MapPins.ElementAt(1));
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(2, Repository.OfType<MapPin>().Queryable.Count());
            Assert.AreEqual(2, item.MapPins.Count);
            #endregion Assert
        }

        /// <summary>
        /// Tests the MapPins cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestMapPinsCascadesWithRemoveItem()
        {
            #region Arrange
            var item = GetValid(9);
            item.AddMapPin(new MapPin(item, true, "Lat", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat2", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat3", "Long"));
            item.AddMapPin(new MapPin(item, false, "Lat4", "Long"));
            Assert.AreEqual(0, Repository.OfType<MapPin>().GetAll().Count);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            Assert.AreEqual(4, Repository.OfType<MapPin>().Queryable.Count());
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<MapPin>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Editors Collection Tests
    }
}
