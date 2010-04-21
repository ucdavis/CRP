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
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                 ""
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OpenIdUser));

        }
        #endregion reflection
    }
}
