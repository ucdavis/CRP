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
    public class SchoolRepositoryTests : AbstractRepositoryTests<School,string>
    {
        protected IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }


        #region Init and Overrides

        public SchoolRepositoryTests()
        {
            SchoolRepository = new RepositoryWithTypedId<School, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override School GetValid(int? counter)
        {
            return CreateValidEntities.School(counter);
        }
        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(School entity, int counter)
        {
            Assert.AreEqual("LongDescription" + counter, entity.LongDescription);
        }
        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(School entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.LongDescription);
                    break;
                case ARTAction.Restore:
                    entity.LongDescription = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.LongDescription;
                    entity.LongDescription = updateValue;
                    break;
                case ARTAction.CompareNotUpdated:
                    Assert.AreEqual(RestoreValue, entity.LongDescription);
                    break;
            }
        }
        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<School> GetQuery(int numberAtEnd)
        {
            return SchoolRepository.Queryable.Where(a => a.LongDescription.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            SchoolRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SchoolRepository.DbContext.CommitTransaction();
        }

        protected override void LoadRecords(int entriesToAdd)
        {
            EntriesAdded += entriesToAdd;
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = GetValid(i + 1);
                SchoolRepository.EnsurePersistent(validEntity);
            }
        }

        #endregion Init and Overrides

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can update entity].
        /// Defaults to true unless overridden
        /// </summary>
        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false); //Mutable is false for this table
        }

        public override void CanGetEntityUsingGetByIdWhereIdIsInt()
        {
            //Don't Run Test
        }

        public override void CanGetEntityUsingGetByNullableWithValidIdWhereIdIsInt()
        {
            //Don't run Test
        }

        public override void CanGetNullValueUsingGetByNullableWithInvalidIdWhereIdIsInt()
        {
            //Don't run this test
        }

        #endregion CRUD Tests

        //TODO: Other Tests
    }
}
