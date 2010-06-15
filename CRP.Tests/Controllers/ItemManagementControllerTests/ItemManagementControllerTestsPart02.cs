using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
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
            "~/ItemManagement/List".ShouldMapTo<ItemManagementController>(a => a.List(null));
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
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create(new Item(), new ExtendedPropertyParameter[1], new string[1], string.Empty), true);
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
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5, new Item(), new ExtendedPropertyParameter[1], new string[1], string.Empty), true);
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


        /// <summary>
        /// Tests the toggle transaction is active mapping.
        /// </summary>
        [TestMethod]
        public void TestToggleTransactionIsActiveMapping()
        {
            "~/ItemManagement/ToggleTransactionIsActive/5".ShouldMapTo<ItemManagementController>(a => a.ToggleTransactionIsActive(5, null, null));	
        }
        #endregion Route Tests
    }
}
