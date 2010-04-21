using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace CRP.Tests.Controllers
{
    [TestClass]
    public class CouponControllerRepositoryTests : ControllerTestBase<CouponController>
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
            Controller = new TestControllerBuilder().CreateController<CouponController>();
        }

        #endregion Init

        #region Route Tests

        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Coupon/Index".ShouldMapTo<CouponController>(a => a.Index());
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters1()
        {
            "~/Coupon/Create/5".ShouldMapTo<CouponController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters2()
        {
            "~/Coupon/Create/5".ShouldMapTo<CouponController>(a => a.Create(5, new Coupon()), true);
        }

        /// <summary>
        /// Test validates the ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestValidateIgnoreParameters()
        {
            "~/Coupon/Validate/5".ShouldMapTo<CouponController>(a => a.Validate(5, "test"), true);    
        }

        /// <summary>
        /// Tests the deactivate mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestDeactivateMappingIgnoreParameters()
        {
            "~/Coupon/Deactivate/5".ShouldMapTo<CouponController>(a => a.Deactivate(5), true);    
        }

        #endregion Route Tests

        
    }
}
