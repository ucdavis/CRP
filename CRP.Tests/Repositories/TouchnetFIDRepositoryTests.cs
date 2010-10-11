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
    /// Entity Name:		TouchnetFID
    /// LookupFieldName:	FID
    /// </summary>
    [TestClass]
    public class TouchnetFIDRepositoryTests : AbstractRepositoryTests<TouchnetFID, int>
    {
        /// <summary>
        /// Gets or sets the TouchnetFID repository.
        /// </summary>
        /// <value>The TouchnetFID repository.</value>
        public IRepository<TouchnetFID> TouchnetFIDRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchnetFIDRepositoryTests"/> class.
        /// </summary>
        public TouchnetFIDRepositoryTests()
        {
            TouchnetFIDRepository = new Repository<TouchnetFID>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override TouchnetFID GetValid(int? counter)
        {
            if(counter == null)
            {
                counter = 99;
            }
            return CreateValidEntities.TouchnetFID((int)counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<TouchnetFID> GetQuery(int numberAtEnd)
        {
            return TouchnetFIDRepository.Queryable.Where(a => a.FID.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(TouchnetFID entity, int counter)
        {
            Assert.AreEqual("Description" + counter, entity.Description);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(TouchnetFID entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Description);
                    break;
                case ARTAction.Restore:
                    entity.Description = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.FID;
                    entity.Description = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            TouchnetFIDRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            TouchnetFIDRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region FID Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FID with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFIDWithNullValueDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.FID = null;
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID: may not be null or empty");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FID with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFIDWithEmptyStringDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.FID = string.Empty;
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID: may not be null or empty", "FID: length must be between 3 and 3");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FID with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFIDWithSpacesOnlyDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.FID = " ";
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID: may not be null or empty", "FID: length must be between 3 and 3");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FID with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFIDWithTooLongValueDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.FID = "x".RepeatTimes((3 + 1));
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                Assert.AreEqual(3 + 1, touchnetFID.FID.Length);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID: length must be between 3 and 3");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FID with too short value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFIDWithTooShortValueDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.FID = "x".RepeatTimes((2));
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                Assert.AreEqual(2, touchnetFID.FID.Length);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID: length must be between 3 and 3");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FID with three character saves.
        /// </summary>
        [TestMethod]
        public void TestFIDWithThreeCharacterSaves()
        {
            #region Arrange
            var touchnetFID = GetValid(9);
            touchnetFID.FID = "xxx";
            #endregion Arrange

            #region Act
            TouchnetFIDRepository.DbContext.BeginTransaction();
            TouchnetFIDRepository.EnsurePersistent(touchnetFID);
            TouchnetFIDRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(touchnetFID.IsTransient());
            Assert.IsTrue(touchnetFID.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FID with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFIDWithLongValueSaves()
        {
            #region Arrange
            var touchnetFID = GetValid(9);
            touchnetFID.FID = "x".RepeatTimes(3);
            #endregion Arrange

            #region Act
            TouchnetFIDRepository.DbContext.BeginTransaction();
            TouchnetFIDRepository.EnsurePersistent(touchnetFID);
            TouchnetFIDRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, touchnetFID.FID.Length);
            Assert.IsFalse(touchnetFID.IsTransient());
            Assert.IsTrue(touchnetFID.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FID Tests
        #region Description Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Description with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithNullValueDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.Description = null;
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithEmptyStringDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.Description = string.Empty;
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithSpacesOnlyDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.Description = " ";
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithTooLongValueDoesNotSave()
        {
            TouchnetFID touchnetFID = null;
            try
            {
                #region Arrange
                touchnetFID = GetValid(9);
                touchnetFID.Description = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                TouchnetFIDRepository.DbContext.BeginTransaction();
                TouchnetFIDRepository.EnsurePersistent(touchnetFID);
                TouchnetFIDRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(touchnetFID);
                Assert.AreEqual(100 + 1, touchnetFID.Description.Length);
                var results = touchnetFID.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: length must be between 0 and 100");
                Assert.IsTrue(touchnetFID.IsTransient());
                Assert.IsFalse(touchnetFID.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Description with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneCharacterSaves()
        {
            #region Arrange
            var touchnetFID = GetValid(9);
            touchnetFID.Description = "x";
            #endregion Arrange

            #region Act
            TouchnetFIDRepository.DbContext.BeginTransaction();
            TouchnetFIDRepository.EnsurePersistent(touchnetFID);
            TouchnetFIDRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(touchnetFID.IsTransient());
            Assert.IsTrue(touchnetFID.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var touchnetFID = GetValid(9);
            touchnetFID.Description = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            TouchnetFIDRepository.DbContext.BeginTransaction();
            TouchnetFIDRepository.EnsurePersistent(touchnetFID);
            TouchnetFIDRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, touchnetFID.Description.Length);
            Assert.IsFalse(touchnetFID.IsTransient());
            Assert.IsTrue(touchnetFID.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Description Tests
      

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
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("FID", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)3, (Int32)3)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(TouchnetFID));

        }

        #endregion Reflection of Database.	
		
		
    }
}