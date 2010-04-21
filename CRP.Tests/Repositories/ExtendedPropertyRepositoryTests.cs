using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ExtendedPropertyRepositoryTests : AbstractRepositoryTests<ExtendedProperty, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ExtendedProperty GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ExtendedProperty(counter);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            rtValue.QuestionType = Repository.OfType<QuestionType>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ExtendedProperty> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ExtendedProperty>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ExtendedProperty entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ExtendedProperty entity, ARTAction action)
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
        /// ExtendedProperty Requires ItemType
        /// ExtendedProperty Requires QuestionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ExtendedProperty>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionTypes(1);
            LoadRecords(5);
            Repository.OfType<ExtendedProperty>().DbContext.CommitTransaction();
        }

        private void LoadItemTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ItemType(entriesToAdd);
                Repository.OfType<ItemType>().EnsurePersistent(validEntity);
            }
        }

        private void LoadQuestionTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.QuestionType(entriesToAdd);
                Repository.OfType<QuestionType>().EnsurePersistent(validEntity);
            }
        }

        #endregion Init and Overrides
    }
}
