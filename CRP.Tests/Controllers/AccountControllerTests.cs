using CRP.Controllers;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : ControllerTestBase<AccountController>
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
            Controller = new TestControllerBuilder().CreateController<AccountController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the logon mapping.
        /// </summary>
        [TestMethod]
        public void TestLogonMapping()
        {
            "~/Account/LogOn/Test".ShouldMapTo<AccountController>(a => a.LogOn(null), true);
        }


        /// <summary>
        /// Tests the logout mapping.
        /// </summary>
        [TestMethod]
        public void TestLogoutMapping()
        {
            "~/Account/LogOut".ShouldMapTo<AccountController>(a => a.LogOut());
        }

        #endregion Route Tests


        [TestMethod]
        public void TestLogOutRedirects()
        {
            Controller.LogOut()
                .AssertHttpRedirect()
                .ToUrl("https://cas.ucdavis.edu/cas/logout");
        }
    }
}
