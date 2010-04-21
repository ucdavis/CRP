﻿using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    [TestClass]
    public partial class ItemRepositoryTests : AbstractRepositoryTests<Item, int>
    {
        protected IRepository<Item> ItemRepository { get; set; }


        #region Init and Overrides
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepositoryTests"/> class.
        /// </summary>
        public ItemRepositoryTests()
        {
            ItemRepository = new Repository<Item>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Item GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Item(counter);
            rtValue.Unit = Repository.OfType<Unit>().GetById(1);
            rtValue.ItemType = Repository.OfType<ItemType>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Item> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Item>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Item entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Item entity, ARTAction action)
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
        /// Item Requires Unit
        /// Item Requires ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Item>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadRecords(5);
            Repository.OfType<Item>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

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
            expectedFields.Add(new NameAndType("Available", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("CostPerItem", "System.Decimal", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Coupons", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Coupon]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Editors", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Editor]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("Expiration", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("ExtendedPropertyAnswers", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ExtendedPropertyAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Image", "System.Byte[]", new List<string>()));
            expectedFields.Add(new NameAndType("ItemType", "CRP.Core.Domain.ItemType", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Link", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LinkLink", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("MapLink", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]",
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Private", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Quantity", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuantityName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("QuestionSets", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.ItemQuestionSets]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("RestrictedKey", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Tags", "System.Collections.Generic.ICollection`1[CRP.Core.Domain.Tag]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            })); 
            expectedFields.Add(new NameAndType("Unit", "CRP.Core.Domain.Unit", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Item));

        }
        #endregion reflection
    }
}