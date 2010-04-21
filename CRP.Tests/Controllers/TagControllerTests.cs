using CRP.Controllers;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Tag Controller Tests
    /// </summary>
    [TestClass]
    public class TagControllerTests : ControllerTestBase<TagController>
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
            Controller = new TestControllerBuilder().CreateController<TagController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping ignore parameter.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingIgnoreParameter()
        {
            "~/Tag/Index/Test".ShouldMapTo<TagController>(a => a.Index("Test"), true);
        }

        #endregion Route Tests
    }
}
