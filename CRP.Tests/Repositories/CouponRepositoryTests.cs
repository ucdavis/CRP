using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class CouponRepositoryTests :AbstractRepositoryTests<Coupon, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Coupon GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Coupon(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Coupon> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Coupon>().Queryable.Where(a => a.Email.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Coupon entity, int counter)
        {
            Assert.AreEqual("email@test.edu" + counter, entity.Email);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Coupon entity, ARTAction action)
        {
            const string updateValue = "updated@test.edu";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Email);
                    break;
                case ARTAction.Restore:
                    entity.Email = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Email;
                    entity.Email = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Coupon Requires Item.
        /// Item Requires Unit.
        /// Item requires ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Coupon>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Coupon>().DbContext.CommitTransaction();
        }

        private void LoadItemTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ItemType(entriesToAdd);
                Repository.OfType<ItemType>().EnsurePersistent(validEntity);
            };
        }

        private void LoadUnits(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Unit(entriesToAdd);
                Repository.OfType<Unit>().EnsurePersistent(validEntity);
            };
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        private void LoadItems(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Item(entriesToAdd);
                validEntity.Unit = Repository.OfType<Unit>().GetById(1);
                validEntity.ItemType = Repository.OfType<ItemType>().GetById(1);
                Repository.OfType<Item>().EnsurePersistent(validEntity);
            }
        }

        #endregion Init and Overrides

        //TODO: Other tests
    }
}
