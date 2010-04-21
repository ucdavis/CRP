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
    /// <summary>
    /// Check Repository Tests
    /// </summary>
    [TestClass]
    public class CheckRepositoryTests : AbstractRepositoryTests<Check, int>
    {
        /// <summary>
        /// Gets or sets the check repository.
        /// </summary>
        /// <value>The check repository.</value>
        public IRepository<Check> CheckRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckRepositoryTests"/> class.
        /// </summary>
        public CheckRepositoryTests()
        {
            CheckRepository = new Repository<Check>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Check GetValid(int? counter)
        {
            return CreateValidEntities.Check(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Check> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Check>().Queryable.Where(a => a.Payee.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Check entity, int counter)
        {
            Assert.AreEqual("Payee" + counter, entity.Payee);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Check entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Payee);
                    break;
                case ARTAction.Restore:
                    entity.Payee = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Payee;
                    entity.Payee = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Check>().DbContext.BeginTransaction();
            LoadRecords(5);
            Repository.OfType<Check>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Validation Tests

        #region Payee Tests

        #region Payee Invalid Tests

        /// <summary>
        /// Tests the payee with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPayeeWithTooLongValueDoesNotSave()
        {
            Check check = null;
            try
            {
                check = CreateValidEntities.Check(null);
                check.Payee = "X".RepeatTimes(201);
                Assert.AreEqual(201, check.Payee.Length, "Payee must be 201 characters for this test.");

                CheckRepository.DbContext.BeginTransaction();
                CheckRepository.EnsurePersistent(check);
                CheckRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(check);
                var results = check.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Payee: length must be between 0 and 200");
                Assert.IsTrue(check.IsTransient());
                Assert.IsFalse(check.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the payee with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPayeeWithNullValueDoesNotSave()
        {
            Check check = null;
            try
            {
                check = CreateValidEntities.Check(null);
                check.Payee = null;

                CheckRepository.DbContext.BeginTransaction();
                CheckRepository.EnsurePersistent(check);
                CheckRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(check);
                var results = check.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Payee: may not be null or empty");
                Assert.IsTrue(check.IsTransient());
                Assert.IsFalse(check.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the payee with spaces only value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPayeeWithSpacesOnlyValueDoesNotSave()
        {
            Check check = null;
            try
            {
                check = CreateValidEntities.Check(null);
                check.Payee = " ";

                CheckRepository.DbContext.BeginTransaction();
                CheckRepository.EnsurePersistent(check);
                CheckRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(check);
                var results = check.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Payee: may not be null or empty");
                Assert.IsTrue(check.IsTransient());
                Assert.IsFalse(check.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the payee with empty value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPayeeWithEmptyValueDoesNotSave()
        {
            Check check = null;
            try
            {
                check = CreateValidEntities.Check(null);
                check.Payee = string.Empty;

                CheckRepository.DbContext.BeginTransaction();
                CheckRepository.EnsurePersistent(check);
                CheckRepository.DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(check);
                var results = check.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Payee: may not be null or empty");
                Assert.IsTrue(check.IsTransient());
                Assert.IsFalse(check.IsValid());
                throw;
            }
        }

        #endregion Payee Invalid Tests

        #region Payee Valid Tests

        /// <summary>
        /// Tests the payee with 200 characters saves.
        /// </summary>
        [TestMethod]
        public void TestPayeeWith200CharactersSaves()
        {
            var check = CreateValidEntities.Check(null);
            check.Payee = "X".RepeatTimes(200);
            Assert.AreEqual(200, check.Payee.Length, "Payee must be 200 characters for this test.");

            CheckRepository.DbContext.BeginTransaction();
            CheckRepository.EnsurePersistent(check);
            CheckRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Tests the payee with 1 character saves.
        /// </summary>
        [TestMethod]
        public void TestPayeeWith1CharacterSaves()
        {
            var check = CreateValidEntities.Check(null);
            check.Payee = "X";
            Assert.AreEqual(1, check.Payee.Length, "Payee must be 1 characters for this test.");

            CheckRepository.DbContext.BeginTransaction();
            CheckRepository.EnsurePersistent(check);
            CheckRepository.DbContext.CommitTransaction();
        }
        #endregion Payee Valid Tests

        #endregion Payee Tests

        #endregion Validation Tests

        #region Fields Without Validation Tests

        /// <summary>
        /// Tests the can save with all values populated.
        /// </summary>
        [TestMethod]
        public void TestCanSaveWithAllValuesPopulated()
        {
            #region Arrange

            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            var transaction = CreateValidEntities.Transaction(1);
            transaction.Item = Repository.OfType<Item>().GetNullableByID(1);
            Repository.OfType<Transaction>().EnsurePersistent(transaction);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();

            

            var check = CreateValidEntities.Check(1);
            check.Amount = 20000.01m;
            check.CheckNumber = 200;
            check.DateReceived = DateTime.Now;
            check.Notes = "Some Notes";
            check.Payee = "Payee";
            //check.Transaction = transaction;
            transaction.AddCheck(check);
            #endregion Arrange

            #region Act
            CheckRepository.DbContext.BeginTransaction();
            CheckRepository.EnsurePersistent(check);
            CheckRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert

            Assert.IsNotNull(check.Transaction);
            Assert.AreSame(transaction, check.Transaction);

            #endregion Assert		
        }

        #endregion Fields Without Validation Tests

        #region Reflection of Database.
        
        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange

            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Amount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("CheckNumber", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("DateReceived", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Payee", "System.String", new List<string>
                                                                             {
                                                                                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]",
                                                                                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
                                                                             }));
            expectedFields.Add(new NameAndType("Transaction", "CRP.Core.Domain.Transaction", new List<string>()));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Check));

        }



        #endregion Reflection of Database.
    }
}
