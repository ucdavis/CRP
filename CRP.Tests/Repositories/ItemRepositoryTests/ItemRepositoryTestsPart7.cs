using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {

        #region ExtendedPropertyAnswers Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the extended property answers with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExtendedPropertyAnswersWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.ExtendedPropertyAnswers = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ExtendedPropertyAnswers: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the extended property answers with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswersWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }


        /// <summary>
        /// Tests the extended property answer with value saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswerWithValueSaves()
        {
            #region Arrange
            LoadItemTypes(1); //Don't really need this here because it is already done in the init
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);

            var extendedPropertyAnswer = CreateValidEntities.ExtendedPropertyAnswer(1);
            extendedPropertyAnswer.ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(1);
            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            item.AddExtendedPropertyAnswer(extendedPropertyAnswer);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the extended property answer with several values saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswerWithSeveralValuesSaves()
        {
            #region Arrange
            LoadItemTypes(1); //Don't really need this here because it is already done in the init
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);

            var extendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            for (int i = 0; i < 5; i++)
            {
                extendedPropertyAnswers.Add(CreateValidEntities.ExtendedPropertyAnswer(i + 1));
                extendedPropertyAnswers[i].ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(1);
                extendedPropertyAnswers[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);

            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[1]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[2]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[4]);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(3, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);
            Assert.AreEqual(3, item.ExtendedPropertyAnswers.Count);
            foreach (var extendedPropertyAnswer in Repository.OfType<ExtendedPropertyAnswer>().GetAll())
            {
                Assert.AreSame(item, extendedPropertyAnswer.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the extended property answer with several values one invalid still saves.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswerWithSeveralValuesOneInvalidStillSaves()
        {
            #region Arrange
            LoadItemTypes(1); //Don't really need this here because it is already done in the init
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);

            var extendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            for (int i = 0; i < 5; i++)
            {
                extendedPropertyAnswers.Add(CreateValidEntities.ExtendedPropertyAnswer(i + 1));
                extendedPropertyAnswers[i].ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(1);
                extendedPropertyAnswers[i].Item = null;
            }
            extendedPropertyAnswers[2].Answer = " "; //Invalid but we don't have the [Valid] attribute on it so it will save anyway.

            Assert.AreEqual(0, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);

            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[1]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[2]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[4]);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(3, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);
            Assert.AreEqual(3, item.ExtendedPropertyAnswers.Count);
            foreach (var extendedPropertyAnswer in Repository.OfType<ExtendedPropertyAnswer>().GetAll())
            {
                Assert.AreSame(item, extendedPropertyAnswer.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the extended property answers cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswersCascadesWithRemove()
        {
            #region Arrange
            LoadItemTypes(1); //Don't really need this here because it is already done in the init
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);

            var extendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            for (int i = 0; i < 5; i++)
            {
                extendedPropertyAnswers.Add(CreateValidEntities.ExtendedPropertyAnswer(i + 1));
                extendedPropertyAnswers[i].ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(1);
                extendedPropertyAnswers[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);

            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[1]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[2]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            item.RemoveExtendedPropertyAnswer(extendedPropertyAnswers[1]);
            item.RemoveExtendedPropertyAnswer(extendedPropertyAnswers[4]);
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(1, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);
            Assert.AreEqual(1, item.ExtendedPropertyAnswers.Count);
            Assert.AreSame(extendedPropertyAnswers[2], Repository.OfType<ExtendedPropertyAnswer>().GetAll().First());
            foreach (var extendedPropertyAnswer in Repository.OfType<ExtendedPropertyAnswer>().GetAll())
            {
                Assert.AreSame(item, extendedPropertyAnswer.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the extended property answers cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestExtendedPropertyAnswersCascadesWithRemoveItem()
        {
            #region Arrange
            LoadItemTypes(1); //Don't really need this here because it is already done in the init
            LoadQuestionTypes(1);
            LoadExtendedProperty(1);

            var extendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            for (int i = 0; i < 5; i++)
            {
                extendedPropertyAnswers.Add(CreateValidEntities.ExtendedPropertyAnswer(i + 1));
                extendedPropertyAnswers[i].ExtendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(1);
                extendedPropertyAnswers[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);

            var item = GetValid(9);
            item.ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[1]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[2]);
            item.AddExtendedPropertyAnswer(extendedPropertyAnswers[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<ExtendedPropertyAnswer>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests
        #endregion ExtendedPropertyAnswers Collection Tests

        #region Coupons Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the coupons with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCouponsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Coupons = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Coupons: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the coupons with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestsCouponsWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Coupons = new List<Coupon>();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the coupons with value saves.
        /// </summary>
        [TestMethod]
        public void TestCouponsWithValueSaves()
        {
            #region Arrange
            var coupon = CreateValidEntities.Coupon(1);            
            var item = GetValid(9);
            item.Coupons = new List<Coupon>();
            item.AddCoupon(coupon);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests        

        #region CRUD Tests
        [TestMethod]
        public void TestCouponsWithSeveralValuesSaves()
        {
            #region Arrange
            var coupons = new List<Coupon>();
            for (int i = 0; i < 5; i++)
            {
                coupons.Add(CreateValidEntities.Coupon(i + 1));
                coupons[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Coupon>().GetAll().Count);

            var item = GetValid(9);
            item.Coupons = new List<Coupon>();
            item.AddCoupon(coupons[1]);
            item.AddCoupon(coupons[2]);
            item.AddCoupon(coupons[4]);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(3, Repository.OfType<Coupon>().GetAll().Count);
            Assert.AreEqual(3, item.Coupons.Count);
            foreach (var coupon in Repository.OfType<Coupon>().GetAll())
            {
                Assert.AreSame(item, coupon.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the coupons cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestCouponsCascadesWithRemove()
        {
            #region Arrange
            var coupons = new List<Coupon>();
            for (int i = 0; i < 5; i++)
            {
                coupons.Add(CreateValidEntities.Coupon(i + 1));
                coupons[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Coupon>().GetAll().Count);

            var item = GetValid(9);
            item.Coupons = new List<Coupon>();
            item.AddCoupon(coupons[1]);
            item.AddCoupon(coupons[2]);
            item.AddCoupon(coupons[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            item.RemoveCoupon(coupons[1]);
            item.RemoveCoupon(coupons[4]);
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(1, Repository.OfType<Coupon>().GetAll().Count);
            Assert.AreEqual(1, item.Coupons.Count);
            Assert.AreSame(coupons[2], Repository.OfType<Coupon>().GetAll().First());
            foreach (var coupon in Repository.OfType<Coupon>().GetAll())
            {
                Assert.AreSame(item, coupon.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the coupons cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestCouponsCascadesWithRemoveItem()
        {
            #region Arrange
            var coupons = new List<Coupon>();
            for (int i = 0; i < 5; i++)
            {
                coupons.Add(CreateValidEntities.Coupon(i + 1));
                coupons[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Coupon>().GetAll().Count);

            var item = GetValid(9);
            item.Coupons = new List<Coupon>();
            item.AddCoupon(coupons[1]);
            item.AddCoupon(coupons[2]);
            item.AddCoupon(coupons[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<Coupon>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Coupons Collection Tests

        #region Editors Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the Editors with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEditorsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Editors = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Editors: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the Editors with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestsEditorsWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Editors = new List<Editor>();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Editors with value saves.
        /// </summary>
        [TestMethod]
        public void TestEditorsWithValueSaves()
        {
            #region Arrange
            LoadUsers(1);
            var editor = CreateValidEntities.Editor(1);
            editor.User = Repository.OfType<User>().GetById(1);
            var item = GetValid(9);
            item.Editors = new List<Editor>();
            item.AddEditor(editor);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the editors with several values saves.
        /// </summary>
        [TestMethod]
        public void TestEditorsWithSeveralValuesSaves()
        {
            #region Arrange
            LoadUsers(1);
            var editors = new List<Editor>();
            for (int i = 0; i < 5; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = Repository.OfType<User>().GetById(1);
                editors[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Editor>().GetAll().Count);

            var item = GetValid(9);
            item.Editors = new List<Editor>();
            item.AddEditor(editors[1]);
            item.AddEditor(editors[2]);
            item.AddEditor(editors[4]);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(3, Repository.OfType<Editor>().GetAll().Count);
            Assert.AreEqual(3, item.Editors.Count);
            foreach (var editor in Repository.OfType<Editor>().GetAll())
            {
                Assert.AreSame(item, editor.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the Editors cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestEditorsCascadesWithRemove()
        {
            #region Arrange
            LoadUsers(1);
            var editors = new List<Editor>();
            for (int i = 0; i < 5; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = Repository.OfType<User>().GetById(1);
                editors[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Editor>().GetAll().Count);

            var item = GetValid(9);
            item.Editors = new List<Editor>();
            item.AddEditor(editors[1]);
            item.AddEditor(editors[2]);
            item.AddEditor(editors[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            item.RemoveEditor(editors[1]);
            item.RemoveEditor(editors[4]);
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(1, Repository.OfType<Editor>().GetAll().Count);
            Assert.AreEqual(1, item.Editors.Count);
            Assert.AreSame(editors[2], Repository.OfType<Editor>().GetAll().First());
            foreach (var editor in Repository.OfType<Editor>().GetAll())
            {
                Assert.AreSame(item, editor.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the Editors cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestEditorsCascadesWithRemoveItem()
        {
            #region Arrange
            LoadUsers(1);
            var editors = new List<Editor>();
            for (int i = 0; i < 5; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = Repository.OfType<User>().GetById(1);
                editors[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Editor>().GetAll().Count);

            var item = GetValid(9);
            item.Editors = new List<Editor>();
            item.AddEditor(editors[1]);
            item.AddEditor(editors[2]);
            item.AddEditor(editors[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<Editor>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Editors Collection Tests

        #region QuestionSets Collection Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the question sets with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.QuestionSets = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionSets: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the question sets with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        
        /// <summary>
        /// Tests the question sets with transaction value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithTransactionValueSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddTransactionQuestionSet(questionSet);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the question sets with quantity value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithQuantityValueSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddQuantityQuestionSet(questionSet);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the question sets with quantity and transaction values saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithQuantityAndTransactionValuesSaves()
        {
            #region Arrange
            var questionSet1 = CreateValidEntities.QuestionSet(1);
            var questionSet2 = CreateValidEntities.QuestionSet(2);
            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddQuantityQuestionSet(questionSet1);
            item.AddTransactionQuestionSet(questionSet2);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }
        
        #endregion Valid Tests        

        #region CRUD Tests
        /// <summary>
        /// Tests the question sets with several values saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsWithSeveralValuesSaves()
        {
            #region Arrange
            var questionSets = new List<QuestionSet>();
            for (int i = 0; i < 7; i++)
            {
                questionSets.Add(CreateValidEntities.QuestionSet(i + 1));
                questionSets[i].Items = null;
            }

            Assert.AreEqual(0, Repository.OfType<QuestionSet>().GetAll().Count);

            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddTransactionQuestionSet(questionSets[1]);
            item.AddQuantityQuestionSet(questionSets[2]);
            item.AddTransactionQuestionSet(questionSets[4]);
            item.AddTransactionQuestionSet(questionSets[5]);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(4, Repository.OfType<QuestionSet>().GetAll().Count);
            Assert.AreEqual(4, item.QuestionSets.Count);
            foreach (var itemQuestionSet in Repository.OfType<ItemQuestionSet>().GetAll())
            {
                Assert.AreSame(item, itemQuestionSet.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the question sets cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsCascadesWithRemove()
        {
            #region Arrange
            var questionSets = new List<QuestionSet>();
            for (int i = 0; i < 7; i++)
            {
                questionSets.Add(CreateValidEntities.QuestionSet(i + 1));
                questionSets[i].Items = null;
            }

            Assert.AreEqual(0, Repository.OfType<QuestionSet>().GetAll().Count);

            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddTransactionQuestionSet(questionSets[1]);
            item.AddQuantityQuestionSet(questionSets[2]);
            item.AddTransactionQuestionSet(questionSets[4]);
            item.AddTransactionQuestionSet(questionSets[5]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            var itemQuestionSets = Repository.OfType<ItemQuestionSet>().GetAll().ToList();
            #endregion Arrange

            #region Act
            item.RemoveQuestionSet(itemQuestionSets[1]);
            item.RemoveQuestionSet(itemQuestionSets[2]);
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(2, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            Assert.AreEqual(2, item.QuestionSets.Count);
            foreach (var itemQuestionSet in Repository.OfType<ItemQuestionSet>().GetAll())
            {
                Assert.AreSame(item, itemQuestionSet.Item);
            }
            #endregion Assert
        }

        /// <summary>
        /// Tests the question sets cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestQuestionSetsCascadesWithRemoveItem()
        {
            #region Arrange
            var questionSets = new List<QuestionSet>();
            for (int i = 0; i < 7; i++)
            {
                questionSets.Add(CreateValidEntities.QuestionSet(i + 1));
                questionSets[i].Items = null;
            }

            Assert.AreEqual(0, Repository.OfType<QuestionSet>().GetAll().Count);

            var item = GetValid(9);
            item.QuestionSets = new List<ItemQuestionSet>();
            item.AddTransactionQuestionSet(questionSets[1]);
            item.AddQuantityQuestionSet(questionSets[2]);
            item.AddTransactionQuestionSet(questionSets[4]);
            item.AddTransactionQuestionSet(questionSets[5]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<ItemQuestionSet>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion QuestionSets Collection Tests


    }
}
