using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class EditorRepositoryTests : AbstractRepositoryTests<Editor, int>
    {
        #region Init and Overrides

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

        private void LoadUsers(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.User(entriesToAdd);
                Repository.OfType<User>().EnsurePersistent(validEntity);
            }
        }

        private void LoadItemTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ItemType(entriesToAdd);
                Repository.OfType<ItemType>().EnsurePersistent(validEntity);
            }
        }

        private void LoadUnits(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Unit(entriesToAdd);
                Repository.OfType<Unit>().EnsurePersistent(validEntity);
            }
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
