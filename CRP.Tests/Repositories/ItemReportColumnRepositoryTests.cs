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
    public class ItemReportColumnRepositoryTests : AbstractRepositoryTests<ItemReportColumn, int>
    {
        protected IRepository<ItemReport> ItemReportRepository { get; set; }

        
        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReportColumnRepositoryTests"/> class.
        /// </summary>
        public ItemReportColumnRepositoryTests()
        {
            ItemReportRepository = new Repository<ItemReport>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ItemReportColumn GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ItemReportColumn(counter);
            rtValue.ItemReport = Repository.OfType<ItemReport>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ItemReportColumn> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<ItemReportColumn>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ItemReportColumn entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ItemReportColumn entity, ARTAction action)
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
            Repository.OfType<ItemReportColumn>().DbContext.BeginTransaction();
            LoadUsers(1);
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadItemReport(1);
            LoadRecords(5);
            Repository.OfType<ItemReportColumn>().DbContext.CommitTransaction();
        }

        

        #endregion Init and Overrides
    }
}
