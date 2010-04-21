using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class DisplayProfileControllerTests : ControllerTestBase<DisplayProfileController>
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
            Controller = new TestControllerBuilder().CreateController<DisplayProfileController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/DisplayProfile/Index".ShouldMapTo<DisplayProfileController>(a => a.Index());            
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/DisplayProfile/List".ShouldMapTo<DisplayProfileController>(a => a.List());
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMapping()
        {
            "~/DisplayProfile/Create".ShouldMapTo<DisplayProfileController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/DisplayProfile/Create/".ShouldMapTo<DisplayProfileController>(a => a.Create(new DisplayProfile()),true);
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/DisplayProfile/Edit/5".ShouldMapTo<DisplayProfileController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/DisplayProfile/Edit/5".ShouldMapTo<DisplayProfileController>(a => a.Edit(5, new DisplayProfile()), true);
        }

        /// <summary>
        /// Tests the get logo mapping.
        /// </summary>
        [TestMethod]
        public void TestGetLogoMapping()
        {
            "~/DisplayProfile/GetLogo/5".ShouldMapTo<DisplayProfileController>(a => a.GetLogo(5));
        }
        #endregion Route Tests
    }
}
