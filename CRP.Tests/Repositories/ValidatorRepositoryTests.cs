using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories
{
    /// <summary>
    /// Entity Name:  Validator
    /// LookupFieldName: Name
    /// </summary>
    [TestClass]
    public class ValidatorRepositoryTests : AbstractRepositoryTests<Validator, int>
    {
        /// <summary>
        /// Gets or sets the Validator repository.
        /// </summary>
        /// <value>The Validator repository.</value>
        public IRepository<Validator> ValidatorRepository { get; set; }
  
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorRepositoryTests"/> class.
        /// </summary>
        public ValidatorRepositoryTests()
        {
            ValidatorRepository = new Repository<Validator>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Validator GetValid(int? counter)
        {
            return CreateValidEntities.Validator(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Validator> GetQuery(int numberAtEnd)
        {
            return ValidatorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Validator entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Validator entity, ARTAction action)
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
        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false); //Mutable is false for this table
        }
        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ValidatorRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ValidatorRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides  
  
        
    }
}