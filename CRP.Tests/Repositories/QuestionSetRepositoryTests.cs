using System;
using System.Collections.Generic;
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
        /// A Query which will return a single record
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

        /// <summary>
        /// Tests the question set linked to an item will not delete.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.ObjectDeletedException))]
        public void TestQuestionSetLinkedToAnItemWillNotDelete()
        {
            try
            {
                Repository.OfType<Item>().DbContext.BeginTransaction();
                LoadUnits(1);
                LoadItemTypes(1);
                LoadItems(2);
                Repository.OfType<Item>().DbContext.CommitTransaction();

                var questionSetToDelete = CreateValidEntities.QuestionSet(10);
                SetupDataToTestCascadeDelete(questionSetToDelete);

                Repository.OfType<Item>().DbContext.BeginTransaction();
                var item = Repository.OfType<Item>().GetById(1);
                item.AddTransactionQuestionSet(questionSetToDelete);
                Repository.OfType<Item>().DbContext.CommitTransaction();

                Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
                Repository.OfType<QuestionSet>().Remove(questionSetToDelete);
                Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("deleted object would be re-saved by cascade (remove deleted object from associations)[CRP.Core.Domain.QuestionSet#6]", ex.Message);
                throw;
            }
        }
        


        #endregion CRUD Cascade Tests

        //TODO: Other tests

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
            questionToAddQuestionOption.QuestionType.HasOptions = true;
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


        [TestMethod]
        public void TestItemTypesMappingProblem()
        {
            LoadItemTypes(1);
            var itemTypeQuestionSet = CreateValidEntities.ItemTypeQuestionSet(1);
            var validEntity = GetValid(null);
            validEntity.ItemTypes = new List<ItemTypeQuestionSet>();
            validEntity.ItemTypes.Add(itemTypeQuestionSet);
            itemTypeQuestionSet.QuestionSet = validEntity;
            itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableByID(1);

            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            Repository.OfType<QuestionSet>().EnsurePersistent(validEntity);
            Assert.IsFalse(validEntity.IsTransient());
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();


            /*
             * This was the mapping file that was wrong (class file):
    <bag name="ItemTypes" table="ItemTypeQuestionSets" cascade="all-delete-orphan" inverse="true">
      <key column="QuestionSetId" />
      <many-to-many column="ItemTypeId" class="CRP.Core.Domain.ItemType, CRP.Core" />
    </bag>
             
             */

            /* before the mapping fix, this generated the following exception:
Test method CRP.Tests.Repositories.QuestionSetRepositoryTests.TestItemTypesMappingProblem threw exception:  NHibernate.PropertyAccessException: Exception occurred getter of CRP.Core.Domain.ItemType.ExtendedProperties --->  System.Reflection.TargetException: Object does not match target type..
at System.Reflection.RuntimeMethodInfo.CheckConsistency(Object target)
at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
at System.Reflection.RuntimePropertyInfo.GetValue(Object obj, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
at System.Reflection.RuntimePropertyInfo.GetValue(Object obj, Object[] index)
at NHibernate.Properties.BasicPropertyAccessor.BasicGetter.Get(Object target)
--- End of inner exception stack trace ---
at NHibernate.Properties.BasicPropertyAccessor.BasicGetter.Get(Object target)
at NHibernate.Tuple.Entity.AbstractEntityTuplizer.GetPropertyValue(Object entity, Int32 i)
at NHibernate.Persister.Entity.AbstractEntityPersister.GetPropertyValue(Object obj, Int32 i, EntityMode entityMode)
at NHibernate.Engine.Cascade.CascadeOn(IEntityPersister persister, Object parent, Object anything)
at NHibernate.Event.Default.AbstractSaveEventListener.CascadeBeforeSave(IEventSource source, IEntityPersister persister, Object entity, Object anything)
at NHibernate.Event.Default.AbstractSaveEventListener.PerformSaveOrReplicate(Object entity, EntityKey key, IEntityPersister persister, Boolean useIdentityColumn, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.AbstractSaveEventListener.PerformSave(Object entity, Object id, IEntityPersister persister, Boolean useIdentityColumn, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.AbstractSaveEventListener.SaveWithGeneratedId(Object entity, String entityName, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.EntityIsTransient(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.PerformSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.OnSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Impl.SessionImpl.FireSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Impl.SessionImpl.SaveOrUpdate(String entityName, Object obj)
at NHibernate.Engine.CascadingAction.SaveUpdateCascadingAction.Cascade(IEventSource session, Object child, String entityName, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeToOne(Object child, IType type, CascadeStyle style, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeAssociation(Object child, IType type, CascadeStyle style, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeProperty(Object child, IType type, CascadeStyle style, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeCollectionElements(Object child, CollectionType collectionType, CascadeStyle style, IType elemType, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeCollection(Object child, CascadeStyle style, Object anything, CollectionType type)
at NHibernate.Engine.Cascade.CascadeAssociation(Object child, IType type, CascadeStyle style, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeProperty(Object child, IType type, CascadeStyle style, Object anything, Boolean isCascadeDeleteEnabled)
at NHibernate.Engine.Cascade.CascadeOn(IEntityPersister persister, Object parent, Object anything)
at NHibernate.Event.Default.AbstractSaveEventListener.CascadeAfterSave(IEventSource source, IEntityPersister persister, Object entity, Object anything)
at NHibernate.Event.Default.AbstractSaveEventListener.PerformSaveOrReplicate(Object entity, EntityKey key, IEntityPersister persister, Boolean useIdentityColumn, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.AbstractSaveEventListener.PerformSave(Object entity, Object id, IEntityPersister persister, Boolean useIdentityColumn, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.AbstractSaveEventListener.SaveWithGeneratedId(Object entity, String entityName, Object anything, IEventSource source, Boolean requiresImmediateIdAccess)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.EntityIsTransient(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.PerformSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Event.Default.DefaultSaveOrUpdateEventListener.OnSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Impl.SessionImpl.FireSaveOrUpdate(SaveOrUpdateEvent event)
at NHibernate.Impl.SessionImpl.SaveOrUpdate(Object obj)
at UCDArch.Data.NHibernate.RepositoryWithTypedId`2.EnsurePersistent(T entity, Boolean forceSave)
at UCDArch.Data.NHibernate.RepositoryWithTypedId`2.EnsurePersistent(T entity)
at CRP.Tests.Repositories.QuestionSetRepositoryTests.TestItemTypesMappingProblem() in QuestionSetRepositoryTests.cs: line 233
             */
        }

    }
}
