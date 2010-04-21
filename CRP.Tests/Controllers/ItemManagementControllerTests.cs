﻿using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// ItemManagement Controller Tests
    /// </summary>
    [TestClass]
    public class ItemManagementControllerTests : ControllerTestBase<ItemManagementController>
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
            Controller = new TestControllerBuilder().CreateController<ItemManagementController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ItemManagement/Index".ShouldMapTo<ItemManagementController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/ItemManagement/List".ShouldMapTo<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create(new Item(),new ExtendedPropertyParameter[1],new string[1] ), true);
        }

        /// <summary>
        /// Tests the get extended properties mapping.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesMapping()
        {
            "~/ItemManagement/GetExtendedProperties/5".ShouldMapTo<ItemManagementController>(a => a.GetExtendedProperties(5));
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5, new Item(),new ExtendedPropertyParameter[1],new string[1]), true);
        }

        /// <summary>
        /// Tests the remove editor with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestRemoveEditorWithParametersMapping()
        {
            "~/ItemManagement/RemoveEditor/5".ShouldMapTo<ItemManagementController>(a => a.RemoveEditor(5, 1), true);
        }

        /// <summary>
        /// Tests the add editor mapping.
        /// </summary>
        [TestMethod]
        public void TestAddEditorMapping()
        {
            "~/ItemManagement/AddEditor/5".ShouldMapTo<ItemManagementController>(a => a.AddEditor(5, null));
        }

        /// <summary>
        /// Tests the save template with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestSaveTemplateWithParametersMapping()
        {
            "~/ItemManagement/SaveTemplate/5".ShouldMapTo<ItemManagementController>(a => a.SaveTemplate(5, "Test"), true);
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/ItemManagement/Details/5".ShouldMapTo<ItemManagementController>(a => a.Details(5));
        }
        #endregion Route Tests
    }
}