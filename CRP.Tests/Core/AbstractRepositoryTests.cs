using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.DomainModel;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;

namespace CRP.Tests.Core
{
    [TestClass]
// ReSharper disable InconsistentNaming
    public abstract class AbstractRepositoryTests<T, IdT> : RepositoryTestBase where T : DomainObjectWithTypedId<IdT>
// ReSharper restore InconsistentNaming
    {
        private int _entriesAdded;
        protected string RestoreValue;
        protected bool BoolRestoreValue;
        
        #region Init

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected abstract T GetValid(int? counter);

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected abstract void FoundEntityComparison(T entity, int counter);

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected abstract void UpdateUtility(T entity, ARTAction action);

        /// <summary>
        /// A Qury which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected abstract IQueryable<T> GetQuery(int numberAtEnd);

        /// <summary>
        /// Loads the records for CRUD Tests.
        /// </summary>
        /// <returns></returns>
        protected virtual void LoadRecords(int entriesToAdd)
        {
            _entriesAdded += entriesToAdd;
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = GetValid(i + 1);
                Repository.OfType<T>().EnsurePersistent(validEntity);
            }
        }

        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid entity].
        /// </summary>
        [TestMethod]
        public void CanSaveValidEntity()
        {
            var validEntity = GetValid(null);
            Repository.OfType<T>().EnsurePersistent(validEntity);

            Assert.AreEqual(false, validEntity.IsTransient());
        }


        /// <summary>
        /// Determines whether this instance [can commit valid entity].
        /// </summary>
        [TestMethod]
        public void CanCommitValidEntity()
        {
            var validEntity = GetValid(null);
            Repository.OfType<T>().DbContext.BeginTransaction();
            Repository.OfType<T>().EnsurePersistent(validEntity);
            Assert.IsFalse(validEntity.IsTransient());
            Repository.OfType<T>().DbContext.CommitTransaction();
        }


        /// <summary>
        /// Determines whether this instance [can get all entities].
        /// </summary>
        [TestMethod]
        public void CanGetAllEntities()
        {
            var foundEntities = Repository.OfType<T>().GetAll().ToList();
            Assert.AreEqual(_entriesAdded, foundEntities.Count, "GetAll() returned a different number of records");
            for (int i = 0; i < _entriesAdded; i++)
            {
                FoundEntityComparison(foundEntities[i], i+1);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query entities].
        /// </summary>
        [TestMethod]
        public void CanQueryEntities()
        {
            List<T> foundEntry = GetQuery(3).ToList();
            Assert.AreEqual(1, foundEntry.Count);
            FoundEntityComparison(foundEntry[0], 3);
        }


        /// <summary>
        /// Determines whether this instance [can get entity using get by id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetEntityUsingGetByIdWhereIdIsInt()
        {
            if(typeof(IdT) == typeof(int))
            {
                Assert.IsTrue(_entriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = Repository.OfType<T>().GetById(2);
                FoundEntityComparison(foundEntity, 2);
            }
        }


        /// <summary>
        /// Determines whether this instance [can get entity using get by nullable with valid id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetEntityUsingGetByNullableWithValidIdWhereIdIsInt()
        {
            if (typeof(IdT) == typeof(int))
            {
                Assert.IsTrue(_entriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = Repository.OfType<T>().GetNullableByID(2);
                FoundEntityComparison(foundEntity, 2);
            }
        }

        /// <summary>
        /// Determines whether this instance [can get null value using get by nullable with invalid id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetNullValueUsingGetByNullableWithInvalidIdWhereIdIsInt()
        {
            if (typeof(IdT) == typeof(int))
            {
                var foundEntity = Repository.OfType<T>().GetNullableByID(_entriesAdded+1);
                Assert.IsNull(foundEntity);
            }
        }

        public void CanUpdateEntity(bool doesItAllowUpdate)
        {
            //Get an entity to update
            var foundEntity = Repository.OfType<T>().GetAll()[2];

            //Update and commit entity
            Repository.OfType<T>().DbContext.BeginTransaction();
            UpdateUtility(foundEntity, ARTAction.Update);
            Repository.OfType<T>().EnsurePersistent(foundEntity);
            Repository.OfType<T>().DbContext.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Evict(foundEntity);

            if (doesItAllowUpdate)
            {
                //Compare entity
                var compareEntity = Repository.OfType<T>().GetAll()[2];
                UpdateUtility(compareEntity, ARTAction.Compare);

                //Restore entity, do not commit, then get entity to make sure it isn't restored.            
                UpdateUtility(compareEntity, ARTAction.Restore);
                NHibernateSessionManager.Instance.GetSession().Evict(compareEntity);
                    //For testing at least, this is required to clear the changes from memory.
                var checkNotUpdatedEntity = Repository.OfType<T>().GetAll()[2];
                UpdateUtility(checkNotUpdatedEntity, ARTAction.Compare);
            }
            else
            {
                //Compare entity
                var compareEntity = Repository.OfType<T>().GetAll()[2];
                UpdateUtility(compareEntity, ARTAction.CompareNotUpdated);            
            }
        }

        /// <summary>
        /// Determines whether this instance [can update entity].
        /// Defaults to true unless overridden
        /// </summary>
        [TestMethod]
        public virtual void CanUpdateEntity()
        {
            CanUpdateEntity(true);            
        }


        /// <summary>
        /// Determines whether this instance [can delete entity].
        /// </summary>
        [TestMethod]
        public virtual void CanDeleteEntity()
        {
            var count = Repository.OfType<T>().GetAll().ToList().Count();
            var foundEntity = Repository.OfType<T>().GetAll().ToList()[2];

            //Update and commit entity
            Repository.OfType<T>().DbContext.BeginTransaction();
            Repository.OfType<T>().Remove(foundEntity);
            Repository.OfType<T>().DbContext.CommitTransaction();
            Assert.AreEqual(count - 1, Repository.OfType<T>().GetAll().ToList().Count());
            if (typeof(IdT) == typeof(int))
            {
                foundEntity = Repository.OfType<T>().GetNullableByID(3);
                Assert.IsNull(foundEntity);
            }
        }

        #endregion CRUD Tests

        #region Utilities

        /// <summary>
        /// Loads the units.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadUnits(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Unit(entriesToAdd);
                Repository.OfType<Unit>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the items.
        /// Requires Unit
        /// Requires ItemType
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadItems(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Item(entriesToAdd);
                validEntity.Unit = Repository.OfType<Unit>().GetById(1);
                validEntity.ItemType = Repository.OfType<ItemType>().GetById(1);
                Repository.OfType<Item>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the item types.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadItemTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ItemType(entriesToAdd);
                Repository.OfType<ItemType>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the question types.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadQuestionTypes(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.QuestionType(entriesToAdd);
                Repository.OfType<QuestionType>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the users.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadUsers(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.User(entriesToAdd);
                Repository.OfType<User>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the question set.
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadQuestionSets(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.QuestionSet(entriesToAdd);
                Repository.OfType<QuestionSet>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the transactions.
        /// Requires Item
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadTransactions(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Transaction(entriesToAdd);
                validEntity.Item = Repository.OfType<Item>().GetById(1);
                Repository.OfType<Transaction>().EnsurePersistent(validEntity);
            }
        }

        /// <summary>
        /// Loads the questions.
        /// Requires QuestionSet
        /// Requires QuestionType
        /// </summary>
        /// <param name="entriesToAdd">The entries to add.</param>
        protected void LoadQuestions(int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Question(entriesToAdd);
                validEntity.QuestionSet = Repository.OfType<QuestionSet>().GetById(1);
                validEntity.QuestionType = Repository.OfType<QuestionType>().GetById(1);
                Repository.OfType<Question>().EnsurePersistent(validEntity);
            }
        }
        
        /// <summary>
        /// Abstract Repository Tests Action
        /// </summary>
        public enum ARTAction
        {
            Compare = 1,
            Update,
            Restore,
            CompareNotUpdated
        }
        #endregion Utilities
    }
}
