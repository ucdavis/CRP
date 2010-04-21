using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class TransactionRepositoryTests : AbstractRepositoryTests<Transaction, int >
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Transaction GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Transaction(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.Check = true;
            }
            else
            {
                rtValue.Check = false;
            }
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Transaction> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Transaction>().Queryable.Where(a => a.Check);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Transaction entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Transaction entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Check);
                    break;
                case ARTAction.Restore:
                    entity.Check = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Check;
                    entity.Check = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Transaction Requires Item
        ///     Item requires Unit and ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        //TODO: Other tests
        //TODO: Look at mapping file to see database calculated fields
    }
}
