using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ItemQuestionSetRepositoryTests : AbstractRepositoryTests<ItemQuestionSet, int >
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemQuestionSet GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemQuestionSet(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.Required = true;
            }
            else
            {
                rtValue.Required = false;
            }

            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemQuestionSet> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemQuestionSet>().Queryable.Where(a => a.Required);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemQuestionSet entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemQuestionSet entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Required);
                    break;
                case ARTAction.Restore:
                    entity.Required = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Required;
                    entity.Required = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// ItemQuestionSet Requires Item
        /// ItemQuestionSet Requires QuestionSet
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ItemQuestionSet>().DbContext.BeginTransaction();
            LoadItems(1);
            LoadQuestionSets(1);
            LoadRecords(5);
            Repository.OfType<ItemQuestionSet>().DbContext.CommitTransaction();
        }

        

        #endregion Init and Overrides

        #region CRUD Tests


        #endregion CRUD Tests

        //TODO: Other tests
    }
}
