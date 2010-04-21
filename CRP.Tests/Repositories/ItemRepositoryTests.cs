using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ItemRepositoryTests : AbstractRepositoryTests<Item, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Item GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Item(counter);
            rtValue.Unit = Repository.OfType<Unit>().GetById(1);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Item> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Item>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Item entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Item entity, ARTAction action)
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
        /// Item Requires Unit
        /// Item Requires ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadRecords(5);
            Repository.OfType<Item>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        //TODO: Test that remove Editor from item cascades to editors.

        //TODO: Other tests
    }
}
