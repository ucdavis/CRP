using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Question Controller Tests
    /// </summary>
    [TestClass]
    public class QuestionControllerTests : ControllerTestBase<QuestionController>
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
            Controller = new TestControllerBuilder().CreateController<QuestionController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Question/Index".ShouldMapTo<QuestionController>(a => a.Index());
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters2()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5, new Question(),new string[1]), true);
        }

        /// <summary>
        /// Tests the delete mapping.
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/Question/Delete/5".ShouldMapTo<QuestionController>(a => a.Delete(5));
        }
        #endregion Route Tests
    }
}
