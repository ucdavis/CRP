using CRP.Controllers;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    [TestClass]
    public class TransactionControllerTests : ControllerTestBase<TransactionController>
    {
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }
        //public IPaymentProvider PaymentProvider { get; set; }
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
            OpenIdUserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OpenIdUser, string>>();
            //PaymentProvider = MockRepository.GenerateStub<IPaymentProvider>();
            Controller = new TestControllerBuilder().CreateController<TransactionController>(OpenIdUserRepository);
        }

        #endregion Init

        #region Route Tests
        /// <summary>
        /// Tests the index mapping.
        /// Alan: Modified to remove index for now. 2/24/2010
        /// </summary>
        //[TestMethod]
        //public void TestIndexMapping()
        //{
        //    "~/Transaction/Index/Test".ShouldMapTo<TransactionController>(a => a.Index());
        //}

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
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5,1, null, "test", string.Empty, string.Empty , new QuestionAnswerParameter[1],new QuestionAnswerParameter[1], true ), true);
        }

        #endregion Route Tests
    }
}
