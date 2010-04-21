using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class ExtendedPropertyAnswerRepositoryTests : AbstractRepositoryTests<ExtendedPropertyAnswer, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ExtendedPropertyAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ExtendedPropertyAnswer(counter);
            rtValue.ExtendedProperty = Repository.OfType<ExtendedProperty>().GetById(1);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ExtendedPropertyAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ExtendedPropertyAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ExtendedPropertyAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ExtendedPropertyAnswer entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Answer);
                    break;
                case ARTAction.Restore:
                    entity.Answer = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Answer;
                    entity.Answer = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// ExtendedPropertyAnswer Requires an ExtendedProperty
        /// ExtendedProperty requires ItemTypes
        /// ExtendedProperty requires QuestionTypes
        /// ExtendedPropertyAnswer Requires Items
        /// Items requires Units and ItemTypes
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<ExtendedPropertyAnswer>().DbContext.BeginTransaction();
            LoadItemTypes(1);
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);
            LoadUnits(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<ExtendedPropertyAnswer>().DbContext.CommitTransaction();
        }

        private void LoadExtendedProperty(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ExtendedProperty(entriesToAdd);
                validEntity.ItemType = Repository.OfType<ItemType>().GetById(1);
                validEntity.QuestionType = Repository.OfType<QuestionType>().GetById(1);
                Repository.OfType<ExtendedProperty>().EnsurePersistent(validEntity);
            }
        }

        


        #endregion Init and Overrides

        //TODO: Other tests
    }
}
