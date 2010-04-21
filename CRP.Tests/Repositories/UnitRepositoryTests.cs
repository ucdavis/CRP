using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class UnitRepositoryTests : AbstractRepositoryTests<Unit, int>
    {
        #region Init and Overrides
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Unit GetValid(int? counter)
        {
            return CreateValidEntities.Unit(counter);
        }
        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Unit> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Unit>().Queryable.Where(a => a.FullName.EndsWith(numberAtEnd.ToString()));
        }
        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Unit entity, int counter)
        {
            Assert.AreEqual("FullName" + counter, entity.FullName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Unit entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.FullName);
                    break;
                case ARTAction.Restore:
                    entity.FullName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.FullName;
                    entity.FullName = updateValue;
                    break;
                case ARTAction.CompareNotUpdated:
                    Assert.AreEqual(RestoreValue, entity.FullName);
                    break;
            }
        }

        protected override void LoadData()
        {
            Repository.OfType<Unit>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<Unit>().DbContext.CommitTransaction();
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

        #endregion CRUD Tests

        //TODO: Other Tests
    }
}
