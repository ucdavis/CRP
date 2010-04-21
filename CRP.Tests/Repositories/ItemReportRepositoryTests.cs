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
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ItemReportRepositoryTests : AbstractRepositoryTests<ItemReport, int>
    {
        protected IRepository<ItemReport> ItemReportRepository { get; set; }

        
        #region Init and Overrides
        public ItemReportRepositoryTests()
        {
            ItemReportRepository = new Repository<ItemReport>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemReport GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemReport(counter);
            rtValue.User = Repository.OfType<User>().GetById(1);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemReport> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemReport>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemReport entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemReport entity, ARTAction action)
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
            Repository.OfType<ItemReport>().DbContext.BeginTransaction();
            LoadUsers(1);
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<ItemReport>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Name Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Name = null;
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                throw;
            }		
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Name = string.Empty;
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Name = " ";
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Name = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(itemReport);
                Assert.AreEqual(51, itemReport.Name.Length);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
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
            var itemReport = GetValid(9);
            itemReport.Name = "x";
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var itemReport = GetValid(9);
            itemReport.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, itemReport.Name.Length);
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Name Tests

        #region Item Tests
        #region InValid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemReportWhenItemIsNullDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Item = null;
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestWhenItemIsNewDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Item = new Item();
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(itemReport);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Item, Entity: CRP.Core.Domain.Item", ex.Message);
                #endregion Assert
                throw;
            }
        }

        #endregion InValid Tests

        #region Valid Tests

        [TestMethod]
        public void TestItemReportWithValidItemSaves()
        {
            #region Arrange
            LoadItems(3);
            var itemReport = GetValid(9);
            itemReport.Item = Repository.OfType<Item>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion Item Tests

        #region User Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIsNullDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.User = null;
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("User: may not be empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestUserIsNewDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.User = new User();
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(itemReport);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.User, Entity: CRP.Core.Domain.User", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Test

        [TestMethod]
        public void TestValidUserSaves()
        {
            #region Arrange
            LoadUsers(3);
            var itemReport = GetValid(9);
            itemReport.User = Repository.OfType<User>().GetNullableByID(3);
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }
        #endregion Valid Test
        #endregion User Tests

        #region SystemReusable Tests

        /// <summary>
        /// Tests the Owner when true saves.
        /// </summary>
        [TestMethod]
        public void TestSystemReusableWhenTrueSaves()
        {
            #region Arrange
            var itemReport = GetValid(9);
            itemReport.SystemReusable = true;
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the Owner when false saves.
        /// </summary>
        [TestMethod]
        public void TestSystemReusableWhenFalseSaves()
        {
            #region Arrange
            var itemReport = GetValid(9);
            itemReport.SystemReusable = false;
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert
        }
        #endregion SystemReusable Tests

        #region Columns Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestColumnsWithNullValueDoesNotSave()
        {
            ItemReport itemReport = null;
            try
            {
                #region Arrange
                itemReport = GetValid(9);
                itemReport.Columns = null;
                #endregion Arrange

                #region Act
                ItemReportRepository.DbContext.BeginTransaction();
                ItemReportRepository.EnsurePersistent(itemReport);
                ItemReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(itemReport);
                var results = itemReport.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Columns: may not be empty");
                Assert.IsTrue(itemReport.IsTransient());
                Assert.IsFalse(itemReport.IsValid());
                #endregion Assert

                throw;
            }		
        }
        
        #endregion Invalid Tests

        #region Valid Tests
        [TestMethod]
        public void TestForMappingProblem()
        {
            #region Arrange
            var itemReportColumn = CreateValidEntities.ItemReportColumn(1);
            var itemReport = GetValid(null);
            itemReport.Columns = new List<ItemReportColumn>();
            itemReport.Columns.Add(itemReportColumn);
            itemReportColumn.ItemReport = itemReport;
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            #endregion Assert
        }

        [TestMethod]
        public void TestColumnsWithNewValueSaves()
        {
            #region Arrange
            var itemReport = GetValid(9);
            itemReport.Columns = new List<ItemReportColumn>();
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests

        #endregion Columns Tests

        #region Add and Remove Methods

        [TestMethod]
        public void TestAddColumnPopulateValuesCorrectly()
        {
            #region Arrange
            var itemReportColumn = CreateValidEntities.ItemReportColumn(9);
            var itemReport = GetValid(99);
            itemReport.AddReportColumn(itemReportColumn);
           
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReport);
            ItemReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(itemReport.IsTransient());
            Assert.IsFalse(itemReportColumn.IsTransient());
            Assert.IsTrue(itemReport.IsValid());
            Assert.AreSame(itemReport.Columns.ToList()[0], itemReportColumn);
            Assert.AreEqual(1, itemReport.Columns.ToList()[0].Order);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveColumnCascadesToItemReportColumn()
        {
            #region Arrange
            Assert.AreEqual(0, Repository.OfType<ItemReportColumn>().GetAll().Count);
            
            //Create 5 report columns to attach to two different ItemReports
            var itemReportColumns = new List<ItemReportColumn>();
            var itemReports = new List<ItemReport>();
            for (int i = 0; i < 5; i++)
            {
                itemReportColumns.Add(CreateValidEntities.ItemReportColumn(i + 1));
            }
            itemReports.Add(GetValid(90));
            itemReports.Add(GetValid(91));

            itemReports[0].AddReportColumn(itemReportColumns[0]);
            itemReports[1].AddReportColumn(itemReportColumns[1]);
            itemReports[1].AddReportColumn(itemReportColumns[2]);
            itemReports[0].AddReportColumn(itemReportColumns[3]);
            itemReports[0].AddReportColumn(itemReportColumns[4]);
            Assert.AreEqual(0, Repository.OfType<ItemReportColumn>().GetAll().Count);

            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReports[0]);
            ItemReportRepository.EnsurePersistent(itemReports[1]);
            ItemReportRepository.DbContext.CommitTransaction();

            Assert.AreEqual(5, Repository.OfType<ItemReportColumn>().GetAll().Count);
            //Ok, we have got to this point and 5 ItemReportColumns have been persisted to the database.
            #endregion Arrange

            #region Act            
            ItemReportRepository.DbContext.BeginTransaction();
            itemReports[1].RemoveReportColumn(itemReports[1].Columns.ToList()[0]);
            ItemReportRepository.EnsurePersistent(itemReports[1]);
            ItemReportRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<ItemReportColumn>().GetAll().Count);
            Assert.AreEqual(1, itemReports[1].Columns.Count);
            #endregion Assert		
        }       
        #endregion Add and Remove Methods

        #region CRUD Cascade Test
        [TestMethod]
        public void TestRemoveItemReportCascadesToItemReportColumns()
        {
            #region Arrange
            Assert.AreEqual(0, Repository.OfType<ItemReportColumn>().GetAll().Count);

            //Create 5 report columns to attach to two different ItemReports
            var itemReportColumns = new List<ItemReportColumn>();
            var itemReports = new List<ItemReport>();
            for (int i = 0; i < 5; i++)
            {
                itemReportColumns.Add(CreateValidEntities.ItemReportColumn(i + 1));
            }
            itemReports.Add(GetValid(90));
            itemReports.Add(GetValid(91));

            itemReports[0].AddReportColumn(itemReportColumns[0]);
            itemReports[1].AddReportColumn(itemReportColumns[1]);
            itemReports[1].AddReportColumn(itemReportColumns[2]);
            itemReports[0].AddReportColumn(itemReportColumns[3]);
            itemReports[0].AddReportColumn(itemReportColumns[4]);
            Assert.AreEqual(0, Repository.OfType<ItemReportColumn>().GetAll().Count);

            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.EnsurePersistent(itemReports[0]);
            ItemReportRepository.EnsurePersistent(itemReports[1]);
            ItemReportRepository.DbContext.CommitTransaction();

            Assert.AreEqual(5, Repository.OfType<ItemReportColumn>().GetAll().Count);
            //Ok, we have got to this point and 5 ItemReportColumns have been persisted to the database.
            #endregion Arrange

            #region Act
            ItemReportRepository.DbContext.BeginTransaction();
            ItemReportRepository.Remove(itemReports[1]);
            ItemReportRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<ItemReportColumn>().GetAll().Count);
            #endregion Assert
        } 
        #endregion CRUD Cascade Test

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


            expectedFields.Add(new NameAndType("Columns", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ItemReportColumn]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]",
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("SystemReusable", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("User", "CRP.Core.Domain.User", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ItemReport));

        }
        #endregion reflection
    }
}
