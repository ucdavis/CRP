using CRP.Controllers;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    [TestClass]
    public class TransactionControllerTests : ControllerTestBase<TransactionController>
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
            Controller = new TestControllerBuilder().CreateController<TransactionController>();
        }

        #endregion Init

        #region Route Tests
        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Transaction/Index/Test".ShouldMapTo<TransactionController>(a => a.Index());
        }

        /// <summary>
        /// Tests the checkout mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5));
        }

        /// <summary>
        /// Tests the checkout with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithParametersMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5,1, null, "test", new QuestionAnswerParameter[1],new QuestionAnswerParameter[1] ), true);
        }

        #endregion Route Tests
    }
}
