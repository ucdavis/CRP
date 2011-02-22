using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests : AbstractRepositoryTests<User, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override User GetValid(int? counter)
        {
            return CreateValidEntities.User(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<User> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<User>().Queryable.Where(a => a.FirstName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(User entity, int counter)
        {
            Assert.AreEqual("FirstName" + counter, entity.FirstName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(User entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.FirstName);
                    break;
                case ARTAction.Restore:
                    entity.FirstName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.FirstName;
                    entity.FirstName = updateValue;
                    break;
                case ARTAction.CompareNotUpdated:
                    Assert.AreEqual(RestoreValue, entity.FirstName);
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<User>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<User>().DbContext.CommitTransaction();
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

        [TestMethod]
        [ExpectedException(typeof(NHibernate.HibernateException))]
        public override void CanDeleteEntity()
        {
            try
            {
                base.CanDeleteEntity();
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("Attempted to delete an object of immutable class: [CRP.Core.Domain.User]", ex.Message);
                throw;
            }

        }

        #endregion CRUD Tests
        //Don't really need any validation tests for Catbert tables
    }
}
