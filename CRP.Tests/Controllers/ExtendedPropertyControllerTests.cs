using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ExtendedPropertyControllerTests : ControllerTestBase<ExtendedPropertyController>
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
            Controller = new TestControllerBuilder().CreateController<ExtendedPropertyController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ExtendedProperty/Index".ShouldMapTo<ExtendedPropertyController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameter()
        {
            "~/ExtendedProperty/Create/5".ShouldMapTo<ExtendedPropertyController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/ExtendedProperty/Create/5".ShouldMapTo<ExtendedPropertyController>(a => a.Create(5, new ExtendedProperty()), true);
        }

        /// <summary>
        /// Tests the delete mapping.
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/ExtendedProperty/Delete/5".ShouldMapTo<ExtendedPropertyController>(a => a.Delete(5));
        }
        #endregion Route Tests
    }
}
