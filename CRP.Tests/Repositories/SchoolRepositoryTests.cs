using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class SchoolRepositoryTests : AbstractRepositoryTests<School,string>
    {
        #region Init and Overrides

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
            }
        }
        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<School> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<School>().Queryable.Where(a => a.LongDescription.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<School>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<School>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        //TODO: Other Tests
    }
}
