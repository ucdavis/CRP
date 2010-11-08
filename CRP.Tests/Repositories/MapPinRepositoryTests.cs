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
    /// <summary>
    /// Entity Name:		MapPin
    /// LookupFieldName:	Title
    /// </summary>
    [TestClass]
    public class MapPinRepositoryTests : AbstractRepositoryTests<MapPin, int>
    {
        /// <summary>
        /// Gets or sets the MapPin repository.
        /// </summary>
        /// <value>The MapPin repository.</value>
        public IRepository<MapPin> MapPinRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPinRepositoryTests"/> class.
        /// </summary>
        public MapPinRepositoryTests()
        {
            MapPinRepository = new Repository<MapPin>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override MapPin GetValid(int? counter)
        {
            var rtValue =  CreateValidEntities.MapPin(counter);
            rtValue.Item = Repository.OfType<Item>().GetByID(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<MapPin> GetQuery(int numberAtEnd)
        {
            return MapPinRepository.Queryable.Where(a => a.Title.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(MapPin entity, int counter)
        {
            Assert.AreEqual("Title" + counter, entity.Title);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(MapPin entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Title);
                    break;
                case ARTAction.Restore:
                    entity.Title = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Title;
                    entity.Title = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            MapPinRepository.DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(3);
            LoadRecords(5);
            MapPinRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region IsPrimary Tests

        /// <summary>
        /// Tests the IsPrimary is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsPrimaryIsFalseSaves()
        {
            #region Arrange

            MapPin mapPin = GetValid(9);
            mapPin.IsPrimary = false;

            #endregion Arrange

            #region Act

            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(mapPin.IsPrimary);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsPrimary is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsPrimaryIsTrueSaves()
        {
            #region Arrange

            var mapPin = GetValid(9);
            mapPin.IsPrimary = true;

            #endregion Arrange

            #region Act

            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(mapPin.IsPrimary);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());

            #endregion Assert
        }

        #endregion IsPrimary Tests

        #region Latitude Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Latitude with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLatitudeWithNullValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Latitude = null;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Latitude with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLatitudeWithEmptyStringDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Latitude = string.Empty;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Latitude with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLatitudeWithSpacesOnlyDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Latitude = " ";
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Latitude with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLatitudeWithTooLongValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Latitude = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                Assert.AreEqual(50 + 1, mapPin.Latitude.Length);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Latitude: length must be between 0 and 50");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Latitude with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLatitudeWithOneCharacterSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Latitude = "x";
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Latitude with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLatitudeWithLongValueSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Latitude = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, mapPin.Latitude.Length);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Latitude Tests

        #region Longitude Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Longitude with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLongitudeWithNullValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Longitude = null;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Longitude with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLongitudeWithEmptyStringDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Longitude = string.Empty;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Longitude with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLongitudeWithSpacesOnlyDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Longitude = " ";
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Longitude with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLongitudeWithTooLongValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Longitude = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                Assert.AreEqual(50 + 1, mapPin.Longitude.Length);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Longitude: length must be between 0 and 50");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Longitude with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLongitudeWithOneCharacterSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Longitude = "x";
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Longitude with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLongitudeWithLongValueSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Longitude = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, mapPin.Longitude.Length);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Longitude Tests

        #region Title Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Title with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTitleWithNullValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Title = null;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Title: may not be null or empty");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Title with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTitleWithEmptyStringDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Title = string.Empty;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Title: may not be null or empty");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Title with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTitleWithSpacesOnlyDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Title = " ";
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Title: may not be null or empty");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Title with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTitleWithTooLongValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Title = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                Assert.AreEqual(50 + 1, mapPin.Title.Length);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Title: length must be between 0 and 50");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Title with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTitleWithOneCharacterSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Title = "x";
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Title with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTitleWithLongValueSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Title = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, mapPin.Title.Length);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Title Tests

        #region Description Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Description with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithTooLongValueDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Description = "x".RepeatTimes((250 + 1));
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                Assert.AreEqual(250 + 1, mapPin.Description.Length);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: length must be between 0 and 250");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Description with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithNullValueSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Description = null;
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithEmptyStringSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Description = string.Empty;
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneSpaceSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Description = " ";
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneCharacterSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Description = "x";
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var mapPin = GetValid(9);
            mapPin.Description = "x".RepeatTimes(250);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(250, mapPin.Description.Length);
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Description Tests

        #region MapPosition Tests

        /// <summary>
        /// Tests the longitude and latitude with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLongitudeAndLatitudeWithSpacesOnlyDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Longitude = " ";
                mapPin.Latitude = " ";
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MapPosition: Select map to position the pointer.");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                throw;
            }
        }

        #endregion MapPosition Tests

        #region Item Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestMapPinWhenItemIsNewDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Item = new Item();
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(mapPin);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.MapPin.Item", ex.Message);
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMapPinWhenItemIsNullDoesNotSave()
        {
            MapPin mapPin = null;
            try
            {
                #region Arrange
                mapPin = GetValid(9);
                mapPin.Item = null;
                #endregion Arrange

                #region Act
                MapPinRepository.DbContext.BeginTransaction();
                MapPinRepository.EnsurePersistent(mapPin);
                MapPinRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(mapPin);
                var results = mapPin.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be empty");
                Assert.IsTrue(mapPin.IsTransient());
                Assert.IsFalse(mapPin.IsValid());
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Test

        [TestMethod]
        public void TestMapPinWithValidItemSaves()
        {
            #region Arrange
            LoadItems(3);
            var mapPin = GetValid(9);
            mapPin.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.EnsurePersistent(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsTransient());
            Assert.IsTrue(mapPin.IsValid());
            #endregion Assert
        }
        #endregion Valid Test
        #endregion Item Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithNoParametersDefaultsExpectedValues()
        {
            #region Arrange
            var mapPin = new MapPin();
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert
            Assert.IsFalse(mapPin.IsPrimary);
            Assert.IsNull(mapPin.Item);
            Assert.IsNull(mapPin.Description);
            Assert.IsNull(mapPin.Latitude);
            Assert.IsNull(mapPin.Longitude);
            Assert.IsNull(mapPin.Title);
            #endregion Assert		
        }

        [TestMethod]
        public void TestConstructorWithParametersDefaultsExpectedValues()
        {
            #region Arrange
            var mapPin = new MapPin(CreateValidEntities.Item(9), true, "Lat", "Long");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert
            Assert.IsTrue(mapPin.IsPrimary);
            Assert.IsNotNull(mapPin.Item);
            Assert.AreEqual("Name9", mapPin.Item.Name);
            Assert.IsNull(mapPin.Description);
            Assert.AreEqual("Lat", mapPin.Latitude);
            Assert.AreEqual("Long", mapPin.Longitude);
            Assert.IsNull(mapPin.Title);
            #endregion Assert
        }
        #endregion Constructor Tests

        #region Cascade Tests

        [TestMethod]
        public void TestRemoveMapPinDoesNotCascadeToItem()
        {
            #region Arrange
            var itemCount = Repository.OfType<Item>().Queryable.Count();
            Assert.IsTrue(itemCount > 1);
            var mapPin = Repository.OfType<MapPin>().GetByID(2);
            Assert.IsNotNull(mapPin);
            #endregion Arrange

            #region Act
            MapPinRepository.DbContext.BeginTransaction();
            MapPinRepository.Remove(mapPin);
            MapPinRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(Repository.OfType<MapPin>().GetNullableByID(2));
            Assert.AreEqual(itemCount, Repository.OfType<Item>().GetAll().Count());
            #endregion Assert		
        }
        #endregion Cascade Tests
        
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
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)250)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));            
            expectedFields.Add(new NameAndType("IsPrimary", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Latitude", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Longitude", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("MapPosition", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Select map to position the pointer.\")]"
            }));
            expectedFields.Add(new NameAndType("Title", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(MapPin));

        }

        #endregion Reflection of Database.	
		
		
    }
}