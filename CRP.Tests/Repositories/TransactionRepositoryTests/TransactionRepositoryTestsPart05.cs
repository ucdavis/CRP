using System;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region OpenIDUser Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestOpenIDUserWithNewValueDoesNotSave()
        {
            Transaction transaction = null;
            try
            {
                #region Arrange
                transaction = GetValid(9);
                transaction.OpenIDUser = new OpenIdUser();
                #endregion Arrange

                #region Act
                TransactionRepository.DbContext.BeginTransaction();
                TransactionRepository.EnsurePersistent(transaction);
                TransactionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(transaction);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.OpenIdUser, Entity: CRP.Core.Domain.OpenIdUser", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the open ID user with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserWithNullValueSaves()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.OpenIDUser = null;
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the open ID user with existing value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserWithExistingValueSaves()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = OpenIDUserRepository.GetNullableByID("2");
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreSame(OpenIDUserRepository.GetNullableByID("2"), transaction.OpenIDUser);
            Assert.IsNotNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region CRUD Tests
        /// <summary>
        /// Tests the open ID user does not cascade when saved.
        /// </summary>
        [TestMethod]
        public void TestOpenIDUserDoesNotCascadeWhenSaved()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = CreateValidEntities.OpenIdUser(9);
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            Assert.IsNotNull(transaction.OpenIDUser);
            Assert.IsFalse(transaction.IsTransient());
            Assert.IsTrue(transaction.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the delete transaction does not cascade to open id user.
        /// </summary>
        [TestMethod]
        public void TestDeleteTransactionDoesNotCascadeToOpenIdUser()
        {
            #region Arrange
            OpenIDUserRepository.DbContext.BeginTransaction();
            LoadOpenIDUsers(3);
            OpenIDUserRepository.DbContext.CommitTransaction();
            var transaction = GetValid(9);
            transaction.OpenIDUser = CreateValidEntities.OpenIdUser(9);
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);

            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.EnsurePersistent(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Arrange

            #region Act
            TransactionRepository.DbContext.BeginTransaction();
            TransactionRepository.Remove(transaction);
            TransactionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, OpenIDUserRepository.GetAll().Count);
            #endregion Assert
        }

        #endregion CRUD Tests
        #endregion OpenIDUser Tests
    }
}
