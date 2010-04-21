using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ApplicationManagementControllerTests :ControllerTestBase<ApplicationManagementController>
    {
        #region Init

        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<ApplicationManagementController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ApplicationManagement/Index".ShouldMapTo<ApplicationManagementController>(a => a.Index());
        }

        /// <summary>
        /// Tests the list item types mapping.
        /// </summary>
        [TestMethod]
        public void TestListItemTypesMapping()
        {
            "~/ApplicationManagement/ListItemTypes".ShouldMapTo<ApplicationManagementController>(a => a.ListItemTypes());
        }

        /// <summary>
        /// Tests the create item type mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeMapping()
        {
            "~/ApplicationManagement/CreateItemType".ShouldMapTo<ApplicationManagementController>(a => a.CreateItemType());
        }

        /// <summary>
        /// Tests the create item type with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithParametersMapping()
        {
            "~/ApplicationManagement/CreateItemType/1".ShouldMapTo<ApplicationManagementController>
                (a => a.CreateItemType(new ItemType(),new ExtendedProperty[1]),true);
        }

        /// <summary>
        /// Tests the edit item type mapping.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeMapping()
        {
            "~/ApplicationManagement/EditItemType/5".ShouldMapTo<ApplicationManagementController>
                (a => a.EditItemType(5));
        }
        /// <summary>
        /// Tests the edit item type with parameteres mapping.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeWithParameteresMapping()
        {
            "~/ApplicationManagement/EditItemType/".ShouldMapTo<ApplicationManagementController>
                (a => a.EditItemType(new ItemType()), true);
        }

        /// <summary>
        /// Tests the toggle active mapping.
        /// </summary>
        [TestMethod]
        public void TestToggleActiveMapping()
        {
            "~/ApplicationManagement/ToggleActive/5".ShouldMapTo<ApplicationManagementController>(a => a.ToggleActive(5));
        }
        #endregion Route Tests
    }
}
