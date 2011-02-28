using System;
using System.Collections.Generic;
using System.Threading;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;
using System.Linq;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region Coupon Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Coupon with A value of new does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCouponWithAValueOfNewDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.Coupon = CreateValidEntities.Coupon(5);
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(transaction);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Coupon, Entity: CRP.Core.Domain.Coupon", ex.Message);
                throw;
            }	
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCouponWithANullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Coupon = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.Coupon);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestCouponWithAnExistingValueSaves()
        {
            #region Arrange
            Repository.OfType<Coupon>().DbContext.BeginTransaction();
            var coupon = CreateValidEntities.Coupon(5);
            coupon.Item = Repository.OfType<Item>().Queryable.First();
            Repository.OfType<Coupon>().EnsurePersistent(coupon);
            Repository.OfType<Coupon>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Coupon = coupon;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Coupon);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteTransactionDoesNotCascadeToCoupon()
        {
            #region Arrange
            Repository.OfType<Coupon>().DbContext.BeginTransaction();
            var coupon = CreateValidEntities.Coupon(5);
            coupon.Item = Repository.OfType<Item>().Queryable.First();
            Repository.OfType<Coupon>().EnsurePersistent(coupon);
            var couponId = coupon.Id;
            Repository.OfType<Coupon>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Coupon = coupon;

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitTransaction();

            var transactionId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(coupon);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TransactionRepository.GetNullableById(transactionId);
            Assert.IsNotNull(record);
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.Remove(record);
            TransactionRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(coupon);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(TransactionRepository.GetNullableById(transactionId));
            Assert.IsNotNull(Repository.OfType<Coupon>().GetNullableById(couponId));
            #endregion Assert		
        }


        [TestMethod]
        public void TestTransactionWithExistingCouponIsRead()
        {
            #region Arrange
            Repository.OfType<Coupon>().DbContext.BeginTransaction();
            var coupon = CreateValidEntities.Coupon(5);
            coupon.Item = Repository.OfType<Item>().Queryable.First();
            Repository.OfType<Coupon>().EnsurePersistent(coupon);
            var couponId = coupon.Id;
            Repository.OfType<Coupon>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Coupon = coupon;

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(record);
            TransactionRepository.DbContext.CommitTransaction();

            var transactionId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(coupon);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TransactionRepository.GetNullableById(transactionId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.IsNotNull(record.Coupon);
            Assert.AreSame(record.Coupon, Repository.OfType<Coupon>().GetNullableById(couponId));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion Coupon Tests
    }
}
