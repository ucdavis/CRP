using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class OpenIdUserRepositoryTests : AbstractRepositoryTests<OpenIdUser, string>
    {
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }

        
        #region Init and Overrides       

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIdUserRepositoryTests"/> class.
        /// </summary>
        public OpenIdUserRepositoryTests()
        {
            OpenIdUserRepository = new RepositoryWithTypedId<OpenIdUser, string>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OpenIdUser GetValid(int? counter)
        {
            return CreateValidEntities.OpenIdUser(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OpenIdUser> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<OpenIdUser>().Queryable.Where(a => a.LastName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OpenIdUser entity, int counter)
        {
            Assert.AreEqual("LastName" + counter, entity.LastName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OpenIdUser entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.LastName);
                    break;
                case ARTAction.Restore:
                    entity.LastName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.LastName;
                    entity.LastName = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OpenIdUserRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OpenIdUserRepository.DbContext.CommitTransaction();
        }

        /// <summary>
        /// Loads the records for CRUD Tests.
        /// </summary>
        /// <param name="entriesToAdd"></param>
        protected override void LoadRecords(int entriesToAdd)
        {
            EntriesAdded += entriesToAdd;
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = GetValid(i + 1);
                OpenIdUserRepository.EnsurePersistent(validEntity);
            }
        }

        #endregion Init and Overrides

        //TODO: Other tests
    }
}
