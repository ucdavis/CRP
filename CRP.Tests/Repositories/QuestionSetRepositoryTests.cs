using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class QuestionSetRepositoryTests : AbstractRepositoryTests<QuestionSet, int >
    {
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }


        #region Init and Overrides
        public QuestionSetRepositoryTests()
        {
            QuestionSetRepository = new Repository<QuestionSet>();
            SchoolRepository = new RepositoryWithTypedId<School, string>();
        }

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

            #region Ok, now that it is setup, delete it and make sure related questions are deleted.
            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            Repository.OfType<QuestionSet>().Remove(questionSetToDelete);
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();
            //NHibernateSessionManager.Instance.GetSession().Evict(questionSetToDelete); //Don't think I need this

            Assert.IsNull(Repository.OfType<QuestionSet>().GetNullableById(6));
            Assert.AreEqual(5, Repository.OfType<QuestionSet>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<QuestionType>().GetAll().Count());
            Assert.AreEqual(2, Repository.OfType<QuestionOption>().GetAll().Count());
            foreach (var question in Repository.OfType<Question>().GetAll().ToList())
            {
                Assert.AreNotEqual(question.QuestionSet.Id, 6);
            }

            #endregion Ok, now that it is setup, delete it and make sure related questions are deleted.
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

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Name = null;
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Name = " ";
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Name = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionSet);
                Assert.AreEqual(51, questionSet.Name.Length);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.Name = "x";
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, questionSet.Name.Length);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region CollegeReusable Tests

        /// <summary>
        /// Tests the CollegeReusable when true saves.
        /// </summary>
        [TestMethod]
        public void TestCollegeReusableWhenTrueSaves()
        {
            #region Arrange
            LoadSchools(1);
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = true;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = false;
            questionSet.School = SchoolRepository.GetNullableById("1");
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(questionSet.CollegeReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the CollegeReusable when false saves.
        /// </summary>
        [TestMethod]
        public void TestCollegeReusableWhenFalseSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = true;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.CollegeReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion CollegeReusable Tests

        #region SystemReusable Tests

        /// <summary>
        /// Tests the SystemReusable when true saves.
        /// </summary>
        [TestMethod]
        public void TestSystemReusableWhenTrueSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = true;
            questionSet.UserReusable = false;

            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(questionSet.SystemReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the SystemReusable when false saves.
        /// </summary>
        [TestMethod]
        public void TestSystemReusableWhenFalseSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = true;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.SystemReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion SystemReusable Tests

        #region SystemReusable Tests

        /// <summary>
        /// Tests the UserReusable when true saves.
        /// </summary>
        [TestMethod]
        public void TestUserReusableWhenTrueSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = true;

            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(questionSet.UserReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the UserReusable when false saves.
        /// </summary>
        [TestMethod]
        public void TestUserReusableWhenFalseSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = true;
            questionSet.UserReusable = false;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.UserReusable);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion UserReusable Tests

        #region School Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSchoolWithNewValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.School = new School();
                questionSet.CollegeReusable = true;
                questionSet.UserReusable = false;
                questionSet.SystemReusable = false;
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.School, Entity: CRP.Core.Domain.School", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestWhenSchoolIsNullSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = true;
            questionSet.School = null;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(questionSet.School);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestWhenSchoolIsNotNullSaves()
        {
            #region Arrange
            LoadSchools(1);
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = true;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = false;
            questionSet.School = SchoolRepository.GetNullableById("1");
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(questionSet.School);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert	
        }
        #endregion Valid Tests

        #region CRUD Tests

        [TestMethod]
        public void TestDeleteQuestionSetDoesNotCascadeToSchool()
        {
            #region Arrange
            SchoolRepository.DbContext.BeginTransaction();
            LoadSchools(3);
            SchoolRepository.DbContext.CommitTransaction();
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = true;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = false;
            questionSet.School = SchoolRepository.GetNullableById("1");

            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, SchoolRepository.GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.Remove(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, SchoolRepository.GetAll().Count);
            #endregion Assert		
        }
        #endregion CRUD Tests
        #endregion School Tests

        #region User Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestUserWithNewValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.User = new User();

                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.User, Entity: CRP.Core.Domain.User", ex.Message);
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestWhenUserIsNullSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.User = null;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(questionSet.User);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestWhenUserIsNotNullSaves()
        {
            #region Arrange
            LoadUsers(1);
            var questionSet = GetValid(9);
            questionSet.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(questionSet.User);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests

        [TestMethod]
        public void TestDeleteQuestionSetDoesNotCascadeToUser()
        {
            #region Arrange
            LoadUsers(3);
            var questionSet = GetValid(9);
            questionSet.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<User>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.Remove(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, Repository.OfType<User>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion User Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive when true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveWhenTrueSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.IsActive = true;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(questionSet.IsActive);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the IsActive when false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveWhenFalseSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.IsActive = false;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.IsActive);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion IsActive Tests

        #region Questions Collections Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionsWithNullValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Questions = null;

                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Questions: may not be empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }		
        }
       
        #endregion Invalid Tests
        #region valid Tests

        [TestMethod]
        public void TestQuestionsWithNewButEmptyListSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.Questions = new List<Question>();
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, questionSet.Questions.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestQuestionsWithNonEmptyListSaves()
        {
            #region Arrange
            LoadQuestionTypes(1);
            LoadQuestions(1);
            var questionSet = GetValid(9);
            questionSet.AddQuestion(Repository.OfType<Question>().GetNullableById(1));
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, questionSet.Questions.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion valid Tests
        #region CRUD Tests

        [TestMethod]
        public void TestQuestionCascadesSave()
        {
            #region Arrange
            LoadQuestionTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddQuestion(CreateValidEntities.Question(1));
            questionSet.AddQuestion(CreateValidEntities.Question(2));
            questionSet.Questions.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            questionSet.Questions.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            Assert.AreEqual(0, Repository.OfType<Question>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert	
        }


        [TestMethod]
        public void TestRemoveQuestionCascadesDelete()
        {
            #region Arrange
            LoadQuestionTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddQuestion(CreateValidEntities.Question(1));
            questionSet.AddQuestion(CreateValidEntities.Question(2));
            questionSet.AddQuestion(CreateValidEntities.Question(3));
            questionSet.Questions.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            questionSet.Questions.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            questionSet.Questions.ElementAt(2).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);

            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Question>().GetAll().Count);
            #endregion Arrange

            #region Act
            questionSet.RemoveQuestion(questionSet.Questions.ElementAt(1));
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, questionSet.Questions.Count);
            Assert.AreEqual(2, Repository.OfType<Question>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert		
        }
        [TestMethod]
        public void TestRemoveQuestionSetCascadesDelete()
        {
            #region Arrange
            LoadQuestionTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddQuestion(CreateValidEntities.Question(1));
            questionSet.AddQuestion(CreateValidEntities.Question(2));
            questionSet.AddQuestion(CreateValidEntities.Question(3));
            questionSet.Questions.ElementAt(0).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            questionSet.Questions.ElementAt(1).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);
            questionSet.Questions.ElementAt(2).QuestionType = Repository.OfType<QuestionType>().GetNullableById(1);

            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            Assert.AreEqual(3, Repository.OfType<Question>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.Remove(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, Repository.OfType<Question>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion Questions Collections Tests

        #region Items Collections Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemsWithNullValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.Items = null;

                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Items: may not be empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }

        #endregion Invalid Tests
        #region valid Tests

        [TestMethod]
        public void TestItemsWithNewButEmptyListSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.Items = new List<ItemQuestionSet>();
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, questionSet.Items.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestItemsWithNonEmptyListSaves()
        {
            #region Arrang
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            var questionSet = GetValid(9);
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(1));
            questionSet.Items.ElementAt(0).Item = Repository.OfType<Item>().GetNullableById(1);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, questionSet.Items.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion valid Tests
        #region CRUD Tests

        [TestMethod]
        public void TestItemsCascadesSave()
        {
            #region Arrange
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            var questionSet = GetValid(9);
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(1));
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(2));
            foreach (var itemQuestionSet in questionSet.Items)
            {
                itemQuestionSet.Item = Repository.OfType<Item>().GetNullableById(1);
            }
            Assert.AreEqual(0, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestRemoveItemsDoesNotCascadeDelete()
        {
            #region Arrange
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            var questionSet = GetValid(9);
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(1));
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(2));
            questionSet.AddItems(CreateValidEntities.ItemQuestionSet(3));
            foreach (var itemQuestionSet in questionSet.Items)
            {
                itemQuestionSet.Item = Repository.OfType<Item>().GetNullableById(1);
            }

            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();

            Assert.AreEqual(3, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            #endregion Arrange

            #region Act
            questionSet.RemoveItems(questionSet.Items.ElementAt(1));
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, questionSet.Items.Count);
            Assert.AreEqual(3, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
       
        #endregion CRUD Tests
        #endregion Items Collections Tests

        #region ItemTypes Collections Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTypesWithNullValueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.ItemTypes = null;

                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ItemTypes: may not be empty");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }

        #endregion Invalid Tests
        #region valid Tests

        [TestMethod]
        public void TestItemTypesWithNewButEmptyListSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.ItemTypes = new List<ItemTypeQuestionSet>();
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, questionSet.ItemTypes.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestItemTypesWithNonEmptyListSaves()
        {
            #region Arrange
            LoadItemTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(1));
            foreach (var itemTypeQuestionSet in questionSet.ItemTypes)
            {
                itemTypeQuestionSet.QuestionSet = questionSet;
                itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableById(1);
            }
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, questionSet.ItemTypes.Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }
        #endregion valid Tests
        #region CRUD Tests

        [TestMethod]
        public void TestItemTypesCascadesSave()
        {
            #region Arrange
            LoadItemTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(1));
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(2));
            
            foreach (var itemTypeQuestionSet in questionSet.ItemTypes)
            {
                itemTypeQuestionSet.QuestionSet = questionSet;
                itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableById(1);
            }
            Assert.AreEqual(0, Repository.OfType<ItemTypeQuestionSet>().GetAll().Count);
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, Repository.OfType<ItemTypeQuestionSet>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestRemoveItemTypesDoesNotCascadeDelete()
        {
            #region Arrange
            LoadItemTypes(1);
            var questionSet = GetValid(9);
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(1));
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(2));
            questionSet.AddItemTypes(CreateValidEntities.ItemTypeQuestionSet(3));
 
            foreach (var itemTypeQuestionSet in questionSet.ItemTypes)
            {
                itemTypeQuestionSet.QuestionSet = questionSet;
                itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableById(1);
            }
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();

            Assert.AreEqual(3, Repository.OfType<ItemTypeQuestionSet>().GetAll().Count);
            #endregion Arrange

            #region Act
            questionSet.RemoveItemTypes(questionSet.ItemTypes.ElementAt(1));
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, questionSet.ItemTypes.Count);
            Assert.AreEqual(3, Repository.OfType<ItemTypeQuestionSet>().GetAll().Count);
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert
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
            itemTypeQuestionSet.ItemType = Repository.OfType<ItemType>().GetNullableById(1);

            Repository.OfType<QuestionSet>().DbContext.BeginTransaction();
            Repository.OfType<QuestionSet>().EnsurePersistent(validEntity);
            Assert.IsFalse(validEntity.IsTransient());
            Repository.OfType<QuestionSet>().DbContext.CommitTransaction();


            #region Description of how this problem happened
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
            #endregion Description of how this problem happened
        }
        #endregion CRUD Tests
        #endregion Items Collections Tests

        #region CollegeReusableSchool Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCollegeReusableAndSchoolIsNullDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                questionSet = GetValid(9);
                questionSet.CollegeReusable = true;
                questionSet.SystemReusable = false;
                questionSet.UserReusable = false;
                questionSet.School = null;
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CollegeReusableSchool: Must have school if college reusable");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }

        #endregion CollegeReusableSchool Tests

        #region Reusability Tests

        [TestMethod]
        public void TestReusabilityWhenAllAreFalseSaves()
        {
            #region Arrange
            var questionSet = GetValid(9);
            questionSet.CollegeReusable = false;
            questionSet.SystemReusable = false;
            questionSet.UserReusable = false;
            #endregion Arrange

            #region Act
            QuestionSetRepository.DbContext.BeginTransaction();
            QuestionSetRepository.EnsurePersistent(questionSet);
            QuestionSetRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionSet.IsTransient());
            Assert.IsTrue(questionSet.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReusabilityWhenAllAreTrueDoesNotSave()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                LoadSchools(1);
                questionSet = GetValid(9);
                questionSet.CollegeReusable = true;
                questionSet.SystemReusable = true;
                questionSet.UserReusable = true;
                questionSet.School = SchoolRepository.GetNullableById("1");
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReusabilityWhenTwoAreTrueDoesNotSave1()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                LoadSchools(1);
                questionSet = GetValid(9);
                questionSet.CollegeReusable = false;
                questionSet.SystemReusable = true;
                questionSet.UserReusable = true;
                questionSet.School = SchoolRepository.GetNullableById("1");
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReusabilityWhenTwoAreTrueDoesNotSave2()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                LoadSchools(1);
                questionSet = GetValid(9);
                questionSet.CollegeReusable = true;
                questionSet.SystemReusable = false;
                questionSet.UserReusable = true;
                questionSet.School = SchoolRepository.GetNullableById("1");
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReusabilityWhenTwoAreTrueDoesNotSave3()
        {
            QuestionSet questionSet = null;
            try
            {
                #region Arrange
                LoadSchools(1);
                questionSet = GetValid(9);
                questionSet.CollegeReusable = true;
                questionSet.SystemReusable = true;
                questionSet.UserReusable = false;
                questionSet.School = SchoolRepository.GetNullableById("1");
                #endregion Arrange

                #region Act
                QuestionSetRepository.DbContext.BeginTransaction();
                QuestionSetRepository.EnsurePersistent(questionSet);
                QuestionSetRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(questionSet);
                var results = questionSet.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reusability: Only one reusable flag may be set to true");
                Assert.IsTrue(questionSet.IsTransient());
                Assert.IsFalse(questionSet.IsValid());
                #endregion Assert
                throw;
            }
        }
        #endregion Reusability Tests

        #region Reflection of Database

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange

            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("CollegeReusable", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("CollegeReusableSchool", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Must have school if college reusable\")]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Items", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ItemQuestionSet]", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ItemTypes", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ItemTypeQuestionSet]", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Questions", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Question]", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Reusability", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Only one reusable flag may be set to true\")]"
            }));
            expectedFields.Add(new NameAndType("School", "CRP.Core.Domain.School", new List<string>()));
            expectedFields.Add(new NameAndType("SystemReusable", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("User", "CRP.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("UserReusable", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionSet));

        }

        #endregion Reflection of Database

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
            Assert.IsNotNull(Repository.OfType<QuestionSet>().GetNullableById(6), "Setup Data Error in test");
            Assert.AreEqual(5, Repository.OfType<QuestionOption>().GetAll().Count(), "Setup Data Error in test");
            Assert.AreEqual(2, Repository.OfType<QuestionType>().GetAll().Count(), "Setup Data Error in test");
            #endregion Setup QuestionSet with linked Questions
        }




    }
}
