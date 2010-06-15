using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Web.ActionResults;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {

        #region Index Tests

        /// <summary>
        /// Tests the index redirects to list.
        /// </summary>
        [TestMethod]
        public void TestIndexRedirectsToList()
        {
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
        }


        #endregion Index Tests

        #region List Tests

        /// <summary>
        /// Tests the list returns view.
        /// </summary>
        [TestMethod]
        public void TestListReturnsView()
        {
            ControllerRecordFakes.FakeItems(Items, 2);
            ControllerRecordFakes.FakeUsers(Users, 2);
            FakeEditors(1);
            Users[1].LoginID = "UserName";
            Editors[0].User = Users[1];
            Items[1].AddEditor(Editors[0]);

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();

            var result = Controller.List(null)
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.FirstOrDefault().Name);
        }

        /// <summary>
        /// Tests the list when not admin only returns list of items where current user is an editor.
        /// </summary>
        [TestMethod]
        public void TestListWhenNotAdminOnlyReturnsListOfItemsWhereCurrentUserIsAnEditor()
        {
            #region Arrange
            ArrangeItemsAndOwners();
            #endregion Arrange

            #region Act
            var result = Controller.List(null)
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains(Items[1]));
            Assert.IsTrue(result.Contains(Items[2]));
            Assert.IsTrue(result.Contains(Items[3]));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the list when admin returns list of all items.
        /// </summary>
        [TestMethod]
        public void TestListWhenAdminReturnsListOfAllItems()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true); //Set the Is role Admin to true
            ArrangeItemsAndOwners();
            #endregion Arrange

            #region Act
            var result = Controller.List(null)
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Count());
            foreach (var item in Items)
            {
                Assert.IsTrue(result.Contains(item));
            }
            #endregion Assert
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
