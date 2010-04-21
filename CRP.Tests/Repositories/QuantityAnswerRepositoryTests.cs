using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class QuantityAnswerRepositoryTests : AbstractRepositoryTests<QuantityAnswer, int>
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuantityAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.QuantityAnswer(counter);
            rtValue.Transaction = Repository.OfType<Transaction>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            rtValue.Question = Repository.OfType<Question>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuantityAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<QuantityAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuantityAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuantityAnswer entity, ARTAction action)
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
        /// QuantityAnswer Requires Transaction
        ///     Transaction Requires Item
        ///         Item requires Unit and ItemType
        /// QuantityAnswer Requires QuestionSet
        /// QuantityAnswer Requires Question
        ///     Question Requires QuestionSet and QuestionType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<QuantityAnswer>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadTransactions(1);
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestionSets(1);
            LoadQuestions(1);
            LoadRecords(5);
            Repository.OfType<QuantityAnswer>().DbContext.CommitTransaction();
        }

        

        #endregion Init and Overrides

        //TODO: Other tests
    }
}
