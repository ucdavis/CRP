using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class EditorRepositoryTests : AbstractRepositoryTests<Editor, int>
    {
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }

        

        #region Init and Overrides
        public EditorRepositoryTests()
        {
            Editors = new List<Editor>();
            EditorRepository = new Repository<Editor>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Editor GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Editor(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            var notNullCounter = 0;
            if(counter != null)
            {
                notNullCounter = (int)counter;
            }
            rtValue.User = Repository.OfType<User>().GetById(notNullCounter);
            if(counter!=null && counter == 3)
            {
                rtValue.Owner = true;
            }
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Editor> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Editor>().Queryable.Where(a => a.Owner);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Editor entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Editor entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Owner);
                    break;
                case ARTAction.Restore:
                    entity.Owner = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Owner;
                    entity.Owner = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Editor Requires Item.
        /// Item Requires Unit.
        /// Item requires ItemType
        /// Editor requires user
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Editor>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadUsers(5);
            LoadRecords(5);  //Note: Each of these records has a different user assigned to it if we want to use that for other tests.
            Repository.OfType<Editor>().DbContext.CommitTransaction();
        }

        


        #endregion Init and Overrides

        #region Item Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestEditorWhenItemIsNewDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.Item = new Item();
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(editor);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Editor.Item", ex.Message);
                #endregion Assert

                throw;
            }	
        }
        
        #endregion Invalid Tests

        #region Valid Test

        [TestMethod]
        public void TestEditorWithValidItemSaves()
        {
            #region Arrange
            LoadItems(3);
            var editor   = GetValid(9);
            editor.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert				
        }
        #endregion Valid Test
        #endregion Item Tests

        #region User Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestEditorWhenUserIsNewDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.User = new User();
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(editor);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Editor.User", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Test

        [TestMethod]
        public void TestEditorWithValidUserSaves()
        {
            #region Arrange
            LoadUsers(3);
            var editor = GetValid(9);
            editor.User = Repository.OfType<User>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }
        #endregion Valid Test
        #endregion User Tests

        #region Owner Tests

        /// <summary>
        /// Tests the Owner when true saves.
        /// </summary>
        [TestMethod]
        public void TestOwnerWhenTrueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Owner = true;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the Owner when false saves.
        /// </summary>
        [TestMethod]
        public void TestOwnerWhenFalseSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Owner = false;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }
        #endregion Owner Tests

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
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>()));
            expectedFields.Add(new NameAndType("Owner", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("User", "CRP.Core.Domain.User", new List<string>()));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Editor));

        }



        #endregion Reflection of Database.
    }
}
