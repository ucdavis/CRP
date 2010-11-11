using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using CRP.Controllers.Helpers;
using CRP.Controllers.Services;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CRP.Tests.InterfaceTests
{
    [TestClass]
    public class AccessControllServiceTests
    {
        public IAccessControllService AccessControllService = new AccessControllService();


        [TestMethod]
        public void TestAccessControllServiceReturnsTrueWhenUserIsAdmin()
        {
            #region Arrange
            var principal = MockRepository.GenerateStub<IPrincipal>();
            principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(true);
            #endregion Arrange

            #region Act
            var result = AccessControllService.HasItemAccess(principal, new Item());
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            principal.AssertWasCalled(a => a.IsInRole(RoleNames.Admin));
            #endregion Assert		
        }


        [TestMethod]
        public void TestAccessControllService()
        {
            #region Arrange

            Assert.Inconclusive("Write other tests");

            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }


    }
}
