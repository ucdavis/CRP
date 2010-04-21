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
    public class OpenIdUserRepositoryTests : AbstractRepositoryTests<OpenIdUser, string>
    {
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }

        
        #region Init and Overrides       

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIdUserRepositoryTests"/> class.
        /// </summary>
        public OpenIdUserRepositoryTests()
        {
            OpenIdUserRepository = new RepositoryWithTypedId<OpenIdUser, string>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OpenIdUser GetValid(int? counter)
        {
            return CreateValidEntities.OpenIdUser(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OpenIdUser> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<OpenIdUser>().Queryable.Where(a => a.LastName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OpenIdUser entity, int counter)
        {
            Assert.AreEqual("LastName" + counter, entity.LastName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OpenIdUser entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.LastName);
                    break;
                case ARTAction.Restore:
                    entity.LastName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.LastName;
                    entity.LastName = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OpenIdUserRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OpenIdUserRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Loads the records for CRUD Tests.
        /// </summary>
        /// <param name="entriesToAdd"></param>
        protected override void LoadRecords(int entriesToAdd)
        {
            EntriesAdded += entriesToAdd;
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = GetValid(i + 1);
                validEntity.UserId = (i + 1).ToString();
                OpenIdUserRepository.EnsurePersistent(validEntity);
            }
        }

        #endregion Init and Overrides

        #region Email Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.Email = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.Email.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: length must be between 0 and 255");                
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the email with null value saves.
        /// </summary>
        [TestMethod] public void TestEmailWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Email = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the email with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Email = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the email with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Email = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Email = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.Email.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Email Tests

        #region FirstName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FirstName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.FirstName = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.FirstName.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FirstName: length must be between 0 and 255");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FirstName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.FirstName = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.FirstName = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.FirstName = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.FirstName = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.FirstName.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion FirstName Tests

        #region LastName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the LastName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLastNameWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.LastName = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.LastName.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("LastName: length must be between 0 and 255");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the LastName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.LastName = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.LastName = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.LastName = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.LastName = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.LastName.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion LastName Tests

        #region StreetAddress
        #region Invalid Tests

        /// <summary>
        /// Tests the StreetAddress with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStreetAddressWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.StreetAddress = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.StreetAddress.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("StreetAddress: length must be between 0 and 255");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the StreetAddress with null value saves.
        /// </summary>
        [TestMethod]
        public void TestStreetAddressWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.StreetAddress = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the StreetAddress with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestStreetAddressWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.StreetAddress = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the StreetAddress with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStreetAddressWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.StreetAddress = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the StreetAddress with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStreetAddressWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.StreetAddress = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.StreetAddress.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion StreetAddress

        #region Address2
        #region Invalid Tests

        /// <summary>
        /// Tests the Address2 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress2WithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.Address2 = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.Address2.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address2: length must be between 0 and 255");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Address2 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Address2 = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Address2 = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Address2 = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Address2 = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.Address2.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Address2

        #region City Tests
         #region Invalid Tests

        /// <summary>
        /// Tests the City with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.City = "x".RepeatTimes(256);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(256, openIdUser.City.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("City: length must be between 0 and 255");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the City with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.City = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.City = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.City = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.City = "x".RepeatTimes(255);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(255, openIdUser.City.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion City Tests

        #region State Tests
        #region Invalid Tests

        /// <summaryState
        /// Tests the LastName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.State = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(51, openIdUser.State.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("State: length must be between 0 and 50");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the State with null value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.State = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.State = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.State = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.State = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, openIdUser.State.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion State Tests

        #region Zip Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Zip with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.Zip = "x".RepeatTimes(11);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(11, openIdUser.Zip.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Zip: length must be between 0 and 10");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Zip with null value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Zip = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Zip = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with one character saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Zip = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with long value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Zip = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, openIdUser.Zip.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Zip Tests

        #region PhoneNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the PhoneNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneNumberWithTooLongValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.PhoneNumber = "x".RepeatTimes(21);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.AreEqual(21, openIdUser.PhoneNumber.Length);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("PhoneNumber: length must be between 0 and 20");
                Assert.IsFalse(openIdUser.IsValid());
                Assert.IsFalse(openIdUser.IsTransient()); //This is false because we have specifically assiged the userId which sets the id value
                #endregion Assert

                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the PhoneNumber with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithNullValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.PhoneNumber = null;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithEmptyStringSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.PhoneNumber = string.Empty;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithOneCharacterSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.PhoneNumber = "x";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithLongValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.PhoneNumber = "x".RepeatTimes(20);
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(20, openIdUser.PhoneNumber.Length);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion PhoneNumber Tests

        #region Transactions Collection Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the transactions with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionsWithNullValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.Transactions = null;
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Transactions: may not be empty");
                Assert.IsFalse(openIdUser.IsTransient()); //Because we assign it directly.
                Assert.IsFalse(openIdUser.IsValid());
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transactions with new value saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionsWithNewValueSaves()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.Transactions = new List<Transaction>();
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the transactions with valid values saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionsWithValidValuesSaves()
        {
            #region Arrange
            LoadTransactions(3);
            var openIdUser = GetValid(9);
            openIdUser.Transactions = new List<Transaction>();
            openIdUser.Transactions.Add(Repository.OfType<Transaction>().GetById(1));
            openIdUser.Transactions.Add(Repository.OfType<Transaction>().GetById(3));
            openIdUser.Transactions.ElementAt(0).OpenIDUser = openIdUser;
            openIdUser.Transactions.ElementAt(1).OpenIDUser = openIdUser;
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert		
        }

        #endregion Valid Tests

        #region CRUD Test

        /// <summary>
        /// Tests the delete transactions does not cascade.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionsDoesNotCascade()
        {
            #region Arrange

            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            OpenIdUserRepository.DbContext.BeginTransaction();
            LoadTransactions(3);
            OpenIdUserRepository.DbContext.CommitTransaction();
            var openIdUser = GetValid(9);
            openIdUser.Transactions = new List<Transaction>();
            openIdUser.Transactions.Add(Repository.OfType<Transaction>().GetById(1));
            openIdUser.Transactions.Add(Repository.OfType<Transaction>().GetById(2));
            openIdUser.Transactions.Add(Repository.OfType<Transaction>().GetById(3));
            openIdUser.Transactions.ElementAt(0).OpenIDUser = openIdUser;
            openIdUser.Transactions.ElementAt(1).OpenIDUser = openIdUser;
            openIdUser.Transactions.ElementAt(2).OpenIDUser = openIdUser;
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            foreach (var transaction in Repository.OfType<Transaction>().GetAll())
            {
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
            }
            Repository.OfType<Transaction>().DbContext.CommitTransaction();

            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();

            Assert.AreEqual(3, Repository.OfType<Transaction>().GetAll().Count);
            Assert.AreEqual(3, openIdUser.Transactions.Count);
            #endregion Arrange

            #region Act
            openIdUser.Transactions.Remove(openIdUser.Transactions.ElementAt(1));
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<Transaction>().GetAll().Count);
            Assert.AreEqual(2, openIdUser.Transactions.Count);
            #endregion Assert		
        }
        #endregion CRUD Test

        #endregion Transactions Collection Tests

        #region UserId Test

        #region Valid Test
        
        /// <summary>
        /// Tests the setting user id sets id value.
        /// </summary>
        [TestMethod]
        public void TestSettingUserIdSetsIdValue()
        {
            #region Arrange
            var openIdUser = GetValid(9);
            openIdUser.UserId = "SomeValue";
            #endregion Arrange

            #region Act
            OpenIdUserRepository.DbContext.BeginTransaction();
            OpenIdUserRepository.EnsurePersistent(openIdUser);
            OpenIdUserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("SomeValue", openIdUser.Id);
            Assert.IsFalse(openIdUser.IsTransient());
            Assert.IsTrue(openIdUser.IsValid());
            #endregion Assert	
        }
        #endregion Valid Test

        #region InvalidTest

        /// <summary>
        /// Tests the setting user id to null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSettingUserIdToNullDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.UserId = null;
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: may not be null or empty");
                Assert.IsFalse(openIdUser.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the setting user id to empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSettingUserIdToEmptyStringDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                openIdUser = GetValid(9);
                openIdUser.UserId = string.Empty;
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                                Assert.IsNotNull(openIdUser);
                var results = openIdUser.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: may not be null or empty");
                Assert.IsFalse(openIdUser.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.NonUniqueObjectException))]
        public void TestSettingUserIdToDuplicateValueDoesNotSave()
        {
            OpenIdUser openIdUser = null;
            try
            {
                #region Arrange
                var foundOpenIdUser = OpenIdUserRepository.GetAll().ElementAt(1);
                openIdUser = GetValid(2);
                openIdUser.UserId = "2";
                Assert.AreEqual(openIdUser.Id, foundOpenIdUser.Id);
                #endregion Arrange

                #region Act
                OpenIdUserRepository.DbContext.BeginTransaction();
                OpenIdUserRepository.EnsurePersistent(openIdUser);
                OpenIdUserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(openIdUser);
                Assert.IsNotNull(ex);
                Assert.AreEqual("a different object with the same identifier value was already associated with the session: 2, of entity: CRP.Core.Domain.OpenIdUser", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion InvalidTest
        #endregion UserId Test

        #region Reflection of Database

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange

            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Address2", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("City", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("FirstName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LastName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("PhoneNumber", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)20)]"
            }));
            expectedFields.Add(new NameAndType("State", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));            
            expectedFields.Add(new NameAndType("StreetAddress", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)255)]"
            }));
            expectedFields.Add(new NameAndType("Transactions", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Transaction]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("UserId", "System.String", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Zip", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)10)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OpenIdUser));

        }
        #endregion reflection
    }
}
