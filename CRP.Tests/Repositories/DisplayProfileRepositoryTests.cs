using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class DisplayProfileRepositoryTests : AbstractRepositoryTests<DisplayProfile, int>
    {
        private IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }

        #region Init and Overrides

        public DisplayProfileRepositoryTests()
        {
            SchoolRepository = new RepositoryWithTypedId<School, string>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override DisplayProfile GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.DisplayProfile(counter);
            rtValue.Unit = Repository.OfType<Unit>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<DisplayProfile> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<DisplayProfile>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(DisplayProfile entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(DisplayProfile entity, ARTAction action)
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
        /// DisplayProfile Requires Units
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadRecords(5);
            Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Validation Moved from the controller Tests

        /// <summary>
        /// Tests the both school and unit populated will not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBothSchoolAndUnitPopulatedWillNotSave()
        {
            DisplayProfile displayProfileRecord = null;
            try
            {
                SchoolRepository.DbContext.BeginTransaction();
                LoadSchools(1);
                SchoolRepository.DbContext.CommitTransaction();

                Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
                displayProfileRecord = CreateValidEntities.DisplayProfile(1);
                displayProfileRecord.Unit = Repository.OfType<Unit>().GetById(1);
                displayProfileRecord.School = SchoolRepository.GetById("1");
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfileRecord);
                Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(displayProfileRecord);
                var results = displayProfileRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnitAndSchool: Unit and School cannot be selected together.");
                Assert.IsTrue(displayProfileRecord.IsTransient());
                Assert.IsFalse(displayProfileRecord.IsValid());
                throw;
            }            
        }

        /// <summary>
        /// Tests that neither school and unit populated will not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNeitherSchoolAndUnitPopulatedWillNotSave()
        {
            DisplayProfile displayProfileRecord = null;
            try
            {
                Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
                displayProfileRecord = CreateValidEntities.DisplayProfile(1);
                displayProfileRecord.Unit = null;
                displayProfileRecord.School = null;
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfileRecord);
                Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(displayProfileRecord);
                var results = displayProfileRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnitOrSchool: A Unit or School must be specified.");
                Assert.IsTrue(displayProfileRecord.IsTransient());
                Assert.IsFalse(displayProfileRecord.IsValid());
                throw;
            }
        }

        

        #endregion Validation Moved from the controller Tests

        //TODO: Other tests

        #region HelperMethods

        private void LoadSchools(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.School(i + 1);
                
                SchoolRepository.EnsurePersistent(validEntity);
            }
        }

        #endregion HelperMethods
    }
}
