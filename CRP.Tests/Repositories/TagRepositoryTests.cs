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
    public class TagRepositoryTests : AbstractRepositoryTests<Tag, int >
    {

        protected IRepository<Tag> TagRepository { get; set; }


        #region Init and Overrides
        public TagRepositoryTests()
        {
            TagRepository = new Repository<Tag>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Tag GetValid(int? counter)
        {
            return CreateValidEntities.Tag(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Tag> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Tag>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Tag entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Tag entity, ARTAction action)
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
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Tag>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<Tag>().DbContext.CommitTransaction();
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
            Tag tag = null;
            try
            {
                #region Arrange
                tag = GetValid(9);
                tag.Name = null;
                #endregion Arrange

                #region Act
                TagRepository.DbContext.BeginTransaction();
                TagRepository.EnsurePersistent(tag);
                TagRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(tag);
                var results = tag.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(tag.IsTransient());
                Assert.IsFalse(tag.IsValid());
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
            Tag tag = null;
            try
            {
                #region Arrange
                tag = GetValid(9);
                tag.Name = string.Empty;
                #endregion Arrange

                #region Act
                TagRepository.DbContext.BeginTransaction();
                TagRepository.EnsurePersistent(tag);
                TagRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(tag);
                var results = tag.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(tag.IsTransient());
                Assert.IsFalse(tag.IsValid());
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
            Tag tag = null;
            try
            {
                #region Arrange
                tag = GetValid(9);
                tag.Name = " ";
                #endregion Arrange

                #region Act
                TagRepository.DbContext.BeginTransaction();
                TagRepository.EnsurePersistent(tag);
                TagRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(tag);
                var results = tag.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(tag.IsTransient());
                Assert.IsFalse(tag.IsValid());
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
            Tag tag = null;
            try
            {
                #region Arrange
                tag = GetValid(9);
                tag.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                TagRepository.DbContext.BeginTransaction();
                TagRepository.EnsurePersistent(tag);
                TagRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(tag);
                Assert.AreEqual(50 + 1, tag.Name.Length);
                var results = tag.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(tag.IsTransient());
                Assert.IsFalse(tag.IsValid());
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
            var tag = GetValid(9);
            tag.Name = "x";
            #endregion Arrange

            #region Act
            TagRepository.DbContext.BeginTransaction();
            TagRepository.EnsurePersistent(tag);
            TagRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(tag.IsTransient());
            Assert.IsTrue(tag.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var tag = GetValid(9);
            tag.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            TagRepository.DbContext.BeginTransaction();
            TagRepository.EnsurePersistent(tag);
            TagRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, tag.Name.Length);
            Assert.IsFalse(tag.IsTransient());
            Assert.IsTrue(tag.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Items Collection Tests (Enumeration)
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemsWithNullValueDoesNotSave()
        {
            Tag tag = null;
            try
            {
                #region Arrange
                tag = GetValid(9);
                tag.Items = null;
                #endregion Arrange

                #region Act
                TagRepository.DbContext.BeginTransaction();
                TagRepository.EnsurePersistent(tag);
                TagRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(tag);
                var results = tag.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Items: may not be empty");
                Assert.IsTrue(tag.IsTransient());
                Assert.IsFalse(tag.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the Items with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestItemsWithEmptyListSaves()
        {
            #region Arrange
            var tag = GetValid(9);
            tag.Items = new List<Item>();
            #endregion Arrange

            #region Act
            TagRepository.DbContext.BeginTransaction();
            TagRepository.EnsurePersistent(tag);
            TagRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(tag.IsTransient());
            Assert.IsTrue(tag.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests

        #endregion Items Collection Tests (Enumeration)

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
            
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Items", "System.Collections.Generic.IEnumerable`1[CRP.Core.Domain.Item]", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Tag));

        }

        #endregion Reflection of Database
    }
}
