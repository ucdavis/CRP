using CRP.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;


namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Item Controller Tests
    /// </summary>
    [TestClass]
    public class ItemControllerTests : ControllerTestBase<ItemController>
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
            Controller = new TestControllerBuilder().CreateController<ItemController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Item/Index".ShouldMapTo<ItemController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/Item/List".ShouldMapTo<ItemController>(a => a.List());
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Item/Details/5".ShouldMapTo<ItemController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the get image mapping.
        /// </summary>
        [TestMethod]
        public void TestGetImageMapping()
        {
            "~/Item/GetImage/5".ShouldMapTo<ItemController>(a => a.GetImage(5));
        }
        #endregion Route Tests
    }
}
