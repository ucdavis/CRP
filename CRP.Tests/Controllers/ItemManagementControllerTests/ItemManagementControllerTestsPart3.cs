using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Web.ActionResults;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }


        #endregion Index Tests

        #region List Tests

        [TestMethod]
        public void TestListReturnsView()
        {
            FakeItems(2);
            FakeUsers(2);
            FakeEditors(1);
            Users[1].LoginID = "UserName";
            Editors[0].User = Users[1];
            Items[1].AddEditor(Editors[0]);

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();

            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.FirstOrDefault().Name);
        }


        #endregion List Tests

        #region GetExtendedProperties Tests

        /// <summary>
        /// Tests the get extended properties return json false when id not found.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesReturnJsonFalseWhenIdNotFound()
        {
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            var result = Controller.GetExtendedProperties(2)
                .AssertResultIs<JsonNetResult>();
            Assert.AreEqual(false, result.Data);
        }

        /// <summary>
        /// Tests the get extended properties returns json net result with extended properties enumeration for item.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesReturnsJsonNetResultWithExtendedPropertiesEnumerationForItem()
        {
            FakeItemTypes(2);
            FakeExtendedProperties(3);

            ItemTypes[1].AddExtendedProperty(ExtendedProperties[0]);
            ItemTypes[1].AddExtendedProperty(ExtendedProperties[2]);

            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();

            var result = Controller.GetExtendedProperties(2)
                .AssertResultIs<JsonNetResult>();
            Assert.AreNotEqual(false, result.Data);
            var extendedProperties = (List<ExtendedProperty>)result.Data;
            Assert.AreEqual(2, extendedProperties.Count);
            Assert.AreEqual(ExtendedProperties[0].Name, extendedProperties[0].Name);
            Assert.AreEqual(ExtendedProperties[2].Name, extendedProperties[1].Name);


        }



        #endregion GetExtendedProperties Tests
    }
}
