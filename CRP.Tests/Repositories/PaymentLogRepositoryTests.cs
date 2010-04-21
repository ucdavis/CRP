using System;
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
    /// Entity Name:  PaymentLog
    /// LookupFieldName: Name
    /// </summary>
    [TestClass]
    public class PaymentLogRepositoryTests : AbstractRepositoryTests<PaymentLog, int>
    {
        /// <summary>
        /// Gets or sets the PaymentLog repository.
        /// </summary>
        /// <value>The PaymentLog repository.</value>
        public IRepository<PaymentLog> PaymentLogRepository { get; set; }
  
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentLogRepositoryTests"/> class.
        /// </summary>
        public PaymentLogRepositoryTests()
        {
            PaymentLogRepository = new Repository<PaymentLog>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override PaymentLog GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.PaymentLog(counter);
            rtValue.Transaction = Repository.OfType<Transaction>().GetNullableByID(1);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<PaymentLog> GetQuery(int numberAtEnd)
        {
            return PaymentLogRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(PaymentLog entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(PaymentLog entity, ARTAction action)
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
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            LoadTransactions(1);

            PaymentLogRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            PaymentLogRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides  
  
        
    }
}