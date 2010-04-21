using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class QuestionSetRepositoryTests : AbstractRepositoryTests<QuestionSet, int >
    {
        #region Init and Overrides

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuestionSet GetValid(int? counter)
        {
            return CreateValidEntities.QuestionSet(counter);
        }

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuestionSet> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<QuestionSet>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuestionSet entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuestionSet entity, ARTAction action)
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
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region CRUD Cascade Tests
        // QuestionSet should cascade delete To Question
        //     Question should cascade delete to QuestionOption
        //     If there is a Transaction related to QuestionSet (and Question) the delete should not happen
        //     No other table should be effected.


        /// <summary>
        /// Tests the question set delete cascades to question and QuestionOption.
        /// QuestionType is not deleted.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetDeleteCascadesToQuestionAndQuestionOption()
        {
            var questionSetToDelete = CreateValidEntities.QuestionSet(6);

            SetupDataToTestCascadeDelete(questionSetToDelete);

            #region Ok, not that it is setup, delete it and make sure related questions are deleted.
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            Repository.OfType<QuestionSet>().Remove(questionSetToDelete);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            //NHibernateSessionManager.Instance.GetSession().Evict(questionSetToDelete); //Don't think I need this

            Assert.IsNull(Repository.OfType<QuestionSet>().GetNullableByID(6));
            Assert.AreEqual(5, Repository.OfType<QuestionSet>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<QuestionType>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<QuestionOption>().GetAll().Count());
            foreach (var question in Repository.OfType<Question>().GetAll().ToList())
            {
                Assert.AreNotEqual(question.QuestionSet.Id, 6);
            }

            #endregion Ok, not that it is setup, delete it and make sure related questions are deleted.

        }
        

        #endregion CRUD Cascade Tests

        /// <summary>
        /// Setup the data to test cascade delete.
        /// This adds a Question Set to test the cascade delete.
        ///     Adds a question with two QuestionOptions
        ///     Adds a question with one QuestionOption
        /// (1 questionSet, 2 Questions, 3 questionOptions to be deleted)
        /// 2 questionTypes are added which do not cascade
        /// 2 Questions each with a question option are added to QuestionSet 1 which will not be deleted.
        /// </summary>
        /// <param name="questionSetToDelete">The question set to delete.</param>
        private void SetupDataToTestCascadeDelete(QuestionSet questionSetToDelete)
        {
            #region Setup QuestionSet with linked Questions
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestionTypes(2);
            LoadQuestions(2);
            Repository.OfType<Question>().DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count(), "Setup Data Error in test");
            Assert.AreEqual(2, Repository.OfType<QuestionType>().GetAll().Count(), "Setup Data Error in test");

            Repository.OfType<Question>().DbContext.BeginTransaction();
            var questionToAddQuestionOption = Repository.OfType<Question>().GetById(1);
            questionToAddQuestionOption.AddOption(CreateValidEntities.QuestionOption(10));
            questionToAddQuestionOption.AddOption(CreateValidEntities.QuestionOption(11));
            questionToAddQuestionOption.Name = "Updated";
            Repository.OfType<Question>().EnsurePersistent(questionToAddQuestionOption);
            Repository.OfType<Question>().DbContext.CommitTransaction();

            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count(), "Setup Data Error in test");

            Assert.AreEqual(2, Repository.OfType<QuestionOption>().GetAll().Count(), "Setup Data Error in test");

            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            var questions = new Question[2];
            questions[0] = CreateValidEntities.Question(10);
            questions[0].QuestionType = Repository.OfType<QuestionType>().GetById(1); //This should not be deleted
            questions[1] = CreateValidEntities.Question(11);
            questions[1].QuestionType = Repository.OfType<QuestionType>().GetById(1); //This should not be deleted

            questions[0].AddOption(CreateValidEntities.QuestionOption(20));
            questions[0].AddOption(CreateValidEntities.QuestionOption(21));
            questions[1].AddOption(CreateValidEntities.QuestionOption(22));


            
            questionSetToDelete.AddQuestion(questions[0]);
            questionSetToDelete.AddQuestion(questions[1]);

            Repository.OfType<QuestionSet>().EnsurePersistent(questionSetToDelete);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();

            Assert.AreEqual(6, Repository.OfType<QuestionSet>().GetAll().Count(), "Setup Data Error in test");
            Assert.AreEqual(4, Repository.OfType<Question>().GetAll().Count(), "Setup Data Error in test");
            Assert.IsNotNull(Repository.OfType<QuestionSet>().GetNullableByID(6), "Setup Data Error in test");
            Assert.AreEqual(5, Repository.OfType<QuestionOption>().GetAll().Count(), "Setup Data Error in test");
            Assert.AreEqual(2, Repository.OfType<QuestionType>().GetAll().Count(), "Setup Data Error in test");
            #endregion Setup QuestionSet with linked Questions
        }

        //TODO: Other tests
    }
}
