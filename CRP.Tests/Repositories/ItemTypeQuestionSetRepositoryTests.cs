using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ItemTypeQuestionSetRepositoryTests : AbstractRepositoryTests<ItemTypeQuestionSet, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemTypeQuestionSet GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemTypeQuestionSet(counter);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.QuantityLevel = true;
                rtValue.TransactionLevel = false;
            }
            else
            {
                rtValue.QuantityLevel = false;
                rtValue.TransactionLevel = true;
            }
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemTypeQuestionSet> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemTypeQuestionSet>().Queryable.Where(a => a.QuantityLevel);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemTypeQuestionSet entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemTypeQuestionSet entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.QuantityLevel);
                    break;
                case ARTAction.Restore:
                    entity.QuantityLevel = BoolRestoreValue;
                    entity.TransactionLevel = !BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.QuantityLevel;
                    entity.QuantityLevel = updateValue;
                    entity.TransactionLevel = !updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ItemTypeQuestionSet>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionSets(1);
            LoadRecords(5);
            Repository.OfType<ItemTypeQuestionSet>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region CRUD Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public override void CanDeleteEntity()
        {
            try
            {
                base.CanDeleteEntity();
            }
            catch(Exception ex)
            {
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.QuestionSet#1]", ex.Message);
                throw;
            }
           
        }

        #endregion CRUD Tests

        //TODO: Other tests
    }
}
