using System.Security.Principal;
using System.Web;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : ControllerTestBase<AccountController>
    {
        private IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;
        protected IPrincipal Principal = new MockPrincipal();

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
            _openIdUserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OpenIdUser, string>>();
            Controller = new TestControllerBuilder().CreateController<AccountController>(_openIdUserRepository);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the log on mapping.
        /// </summary>
        [TestMethod, Ignore]
        public void TestLogOnMapping()
        {
            "~/Account/LogOn/Test".ShouldMapTo<AccountController>(a => a.LogOn(null), true);
        }


        /// <summary>
        /// Tests the log out mapping.
        /// </summary>
        [TestMethod, Ignore]
        public void TestLogOutMapping()
        {
            "~/Account/LogOut".ShouldMapTo<AccountController>(a => a.LogOut());
        }

        #endregion Route Tests


        [TestMethod, Ignore]
        public void TestLogOutRedirects()
        {
            //This requires more mocking because of the Forms... (Can look at FSNEP for an example)
            Controller.ControllerContext.HttpContext.User = Principal;

            Controller.LogOut()
                .AssertHttpRedirect()
                .ToUrl("https://cas.ucdavis.edu/cas/logout");
        }


        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "httpUserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }
        }
        #endregion
    }
}
