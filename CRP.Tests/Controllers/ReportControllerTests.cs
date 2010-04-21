using CRP.Controllers;
using CRP.Core.Abstractions;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ReportControllerTests : ControllerTestBase<ReportController>
    {
        protected IChartProvider ChartProvider;
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
            ChartProvider = MockRepository.GenerateStub<IChartProvider>();
            Controller = new TestControllerBuilder().CreateController<ReportController>(ChartProvider);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the view report ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestViewReportIgnoreParameters()
        {
            "~/Report/ViewReport/5".ShouldMapTo<ReportController>(a => a.ViewReport(5, 12), true);
        }
        

        #endregion Route Tests
    }
}
