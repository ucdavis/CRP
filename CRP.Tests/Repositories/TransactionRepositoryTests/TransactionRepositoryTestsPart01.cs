using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    [TestClass]
    public partial class TransactionRepositoryTests : AbstractRepositoryTests<Transaction, int >
    {
        public IRepository<Transaction> TransactionRepository { get; set; }
        public IRepositoryWithTypedId<OpenIdUser, string> OpenIDUserRepository { get; set; }
      
        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepositoryTests"/> class.
        /// </summary>
        public TransactionRepositoryTests()
        {
            TransactionRepository = new Repository<Transaction>();
            OpenIDUserRepository = new RepositoryWithTypedId<OpenIdUser, string>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Transaction GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Transaction(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            if (counter != null && counter == 3)
            {
                rtValue.Check = true;
                rtValue.Credit = false;
            }
            else
            {
                rtValue.Check = false;
                rtValue.Credit = true;
            }
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Transaction> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Transaction>().Queryable.Where(a => a.Check);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Transaction entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Transaction entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Check);
                    break;
                case ARTAction.Restore:
                    entity.Check = BoolRestoreValue;
                    entity.Credit = !entity.Check;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Check;
                    entity.Check = updateValue;
                    entity.Credit = !entity.Check;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Transaction Requires Item
        ///     Item requires Unit and ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides


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
            expectedFields.Add(new NameAndType("Amount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("AmountTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Check", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ChildTransactions", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Transaction]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CorrectionAmount", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Amount must be less than zero.\")]"
            }));
            expectedFields.Add(new NameAndType("CorrectionReason", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CorrectionTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("CorrectionTotalAmount", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"The total of all correction amounts must not exceed the donation amounts\")]"
            }));
            expectedFields.Add(new NameAndType("CreatedBy", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Credit", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Donation", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("DonationTotal", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("OpenIDUser", "CRP.Core.Domain.OpenIdUser", new List<string>()));
            expectedFields.Add(new NameAndType("Paid", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ParentTransaction", "CRP.Core.Domain.Transaction", new List<string>()));
            expectedFields.Add(new NameAndType("PaymentLogs", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.PaymentLog]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("PaymentType", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Payment type was not selected.\")]"
            }));
            expectedFields.Add(new NameAndType("Quantity", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityAnswers", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.QuantityAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("RegularAmount", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Amount must be zero or more.\")]"
            }));
            expectedFields.Add(new NameAndType("Total", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaid", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaidByCheck", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TotalPaidByCredit", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionAnswers", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.TransactionAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("TransactionDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("TransactionNumber", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("UncorrectedDonationTotal", "System.Decimal", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Transaction));

        }

        #endregion Reflection of Database
    }
}
