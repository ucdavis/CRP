using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class TransactionAnswerRepositoryTests : AbstractRepositoryTests<TransactionAnswer, int >
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override TransactionAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.TransactionAnswer(counter);
            rtValue.Transaction = Repository.OfType<Transaction>().GetById(1);
            rtValue.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
            rtValue.Question = Repository.OfType<Question>().GetById(1);

            return rtValue;
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<TransactionAnswer> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<TransactionAnswer>().Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(TransactionAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(TransactionAnswer entity, ARTAction action)
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
        /// TransactionAnswer requires Transaction
        ///     Transaction requires Items
        ///         Items requires Units and ItemTypes
        /// TransactionAnswer requires QuestionSet
        /// TransactionAnswer requires Question
        ///     QuestionRequires QuestionSet and QuestionType 
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<TransactionAnswer>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadQuestionSets(1);
            LoadQuestionTypes(1);
            LoadQuestions(1);
            LoadTransactions(1);
            LoadRecords(5);
            Repository.OfType<TransactionAnswer>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        //TODO: Other tests
    }
}
