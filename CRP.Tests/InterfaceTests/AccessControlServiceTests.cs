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
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CRP.Tests.InterfaceTests
{
    [TestClass]
    public class AccessControlServiceTests
    {
        public IAccessControlService AccessControlService = new AccessControlService();


        [TestMethod]
        public void TestAccessControlServiceReturnsTrueWhenUserIsAdmin()
        {
            #region Arrange
            var principal = MockRepository.GenerateStub<IPrincipal>();
            principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(true);
            #endregion Arrange

            #region Act
            var result = AccessControlService.HasItemAccess(principal, new Item());
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            principal.AssertWasCalled(a => a.IsInRole(RoleNames.Admin));
            #endregion Assert		
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestAccessControlServiceThrowsExceptionIfItemIsNull()
        {
            try
            {
                #region Arrange
                var principal = MockRepository.GenerateStub<IPrincipal>();
                principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(true);
                #endregion Arrange

                #region Act
                var result = AccessControlService.HasItemAccess(principal, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("An item is required.", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestAccessControlServiceThrowsExceptionIfItemHasNullEditors()
        {
            try
            {
                #region Arrange
                var item = CreateValidEntities.Item(1);
                item.Editors = null;
                var principal = MockRepository.GenerateStub<IPrincipal>();
                principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(true);
                #endregion Arrange

                #region Act
                var result = AccessControlService.HasItemAccess(principal, item);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("An item must have at least one editor.", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestAccessControlServiceReturnsTrueIfCurrentUserIsAnEditor()
        {
            #region Arrange
            var principal = MockRepository.GenerateStub<IPrincipal>();
            principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(false);
            principal.Expect(a => a.Identity.Name).Return("TestUser").Repeat.Any();
            var item = CreateValidEntities.Item(1);
            var matchUser = CreateValidEntities.User(1);
            matchUser.LoginID = "TestUser";
            var editors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                editors.Add(CreateValidEntities.Editor(i+1));
                editors[i].User = CreateValidEntities.User(i + 2);
            }
            editors.Add(CreateValidEntities.Editor(9));
            editors[3].User = matchUser;
            item.Editors = editors;
            #endregion Arrange

            #region Act
            var result = AccessControlService.HasItemAccess(principal, item);
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAccessControlServiceReturnsFalseIfCurrentUserIsNotAnEditor()
        {
            #region Arrange
            var principal = MockRepository.GenerateStub<IPrincipal>();
            principal.Expect(a => a.IsInRole(RoleNames.Admin)).Return(false);
            principal.Expect(a => a.Identity.Name).Return("TestUser").Repeat.Any();
            var item = CreateValidEntities.Item(1);
            var matchUser = CreateValidEntities.User(1);
            matchUser.LoginID = "NoMatch";
            var editors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = CreateValidEntities.User(i + 2);
            }
            editors.Add(CreateValidEntities.Editor(9));
            editors[3].User = matchUser;
            item.Editors = editors;
            #endregion Arrange

            #region Act
            var result = AccessControlService.HasItemAccess(principal, item);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }


    }
}
