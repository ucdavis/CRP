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
    public class TemplateRepositoryTests : AbstractRepositoryTests<Template, int>
    {
        protected IRepository<Template> TemplateRepository { get; set; }

        #region Init and Overrides

        public TemplateRepositoryTests()
        {
            TemplateRepository = new Repository<Template>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Template GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Template(counter);
            rtValue.Item = Repository.OfType<Item>().GetByID(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Template> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Template>().Queryable.Where(a => a.Text.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Template entity, int counter)
        {
            Assert.AreEqual("Text" + counter, entity.Text);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Template entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Text);
                    break;
                case ARTAction.Restore:
                    entity.Text = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Text;
                    entity.Text = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Template>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Template>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Text Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Text with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithNullValueDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Text = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Text: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithEmptyStringDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Text = string.Empty;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Text: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithSpacesOnlyDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Text = " ";
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Text: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Text with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneCharacterSaves()
        {
            #region Arrange
            var template = GetValid(9);
            template.Text = "x";
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithLongValueSaves()
        {
            #region Arrange
            var template = GetValid(9);
            template.Text = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, template.Text.Length);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Item Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestItemWithNewValueDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Item = new Item();
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(template);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Item, Entity: CRP.Core.Domain.Item", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Test
        /// <summary>
        /// Tests the item with different value saves.
        /// </summary>
        [TestMethod]
        public void TestItemWithDifferentValueSaves()
        {
            #region Arrange
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(3);
            var template = GetValid(9);
            template.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreSame(Repository.OfType<Item>().GetNullableByID(3), template.Item);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }

        #endregion Valid Test
        #endregion Item Tests

        #region Default Tests

        /// <summary>
        /// Tests the Default is false saves.
        /// </summary>
        [TestMethod]
        public void TestDefaultIsFalseSaves()
        {
            #region Arrange

            var template = GetValid(9);
            template.Default = false;

            #endregion Arrange

            #region Act

            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(template.Default);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Default is true saves.
        /// </summary>
        [TestMethod]
        public void TestDefaultIsTrueSaves()
        {
            #region Arrange

            var template = GetValid(9);
            template.Default = true;

            #endregion Arrange

            #region Act

            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(template.Default);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());

            #endregion Assert
        }

        #endregion Default Tests

        #region ItemAndDefault Tests
        /// <summary>
        /// Tests the item with null value and default is true saves.
        /// </summary>
        [TestMethod]
        public void TestItemWithNullValueAndDefaultIsTrueSaves()
        {
            #region Arrange
            var template = GetValid(9);
            template.Item = null;
            template.Default = true;
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }
        /// <summary>
        /// Tests the item with null value and not default does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemWithNullValueAndNotDefaultDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Item = null;
                template.Default = false;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                //results.AssertErrorsAre("Item: may not be empty");
                results.AssertErrorsAre("ItemAndDefault: Item may not be empty when default not selected");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        #endregion ItemAndDefault Tests

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
            expectedFields.Add(new NameAndType("Default", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>()));
            expectedFields.Add(new NameAndType("ItemAndDefault", "System.Boolean", new List<string>
            { 
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Item may not be empty when default not selected\")]"
            }));
            expectedFields.Add(new NameAndType("Text", "System.String", new List<string>
            { 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Template));

        }

        #endregion Reflection of Database
    }
}
