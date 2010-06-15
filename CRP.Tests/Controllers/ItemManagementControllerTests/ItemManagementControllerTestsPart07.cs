using System.Web.Mvc;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region AddEditor Tests

        [TestMethod]
        public void TestAddEditorRedirectsToEditorTabIfUserIdParameterIsNull()
        {
            #region Arrange
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(1, null)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreEqual("#Editors", result.Url);
            Assert.AreEqual(NotificationMessages.STR_SelectUserFirst, Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddEditorRedirectsToListIfIdNotFound()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(1)).Return(Users[0]).Repeat.Any();
            Controller.AddEditor(1, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestAddEditorRedirectsToListUserIfIdNotFound()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.AddEditor(1, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestAddEditorWithValidDataSaves()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            FakeEditors(2);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(1)).Return(Users[0]).Repeat.Any();

            Assert.AreEqual(1, Items[0].Editors.Count);

            var result = Controller.AddEditor(1, 1)
                .AssertHttpRedirect();
            Assert.AreEqual("#Editors", result.Url);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual(2, Items[0].Editors.Count);
            Assert.AreEqual("Editor has been saved successfully.", Controller.Message);
        }
        [TestMethod]
        public void TestAddEditorWithInvalidDataDoesNotSave()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            FakeEditors(2);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(1)).Return(Users[0]).Repeat.Any();

            Items[0].Name = " ";

            var result = Controller.AddEditor(1, 1)
                .AssertHttpRedirect();
            Assert.AreEqual("#Editors", result.Url);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual("Unable to add editor.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        [TestMethod]
        public void TestAddEditorWithCurrentUserNotAnEditorDoesNotSave()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            //Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            Users[1].LoginID = "NotFound";
            FakeEditors(2);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(1)).Return(Users[0]).Repeat.Any();

            Assert.AreEqual(1, Items[0].Editors.Count);

            Controller.AddEditor(1, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));


            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
        }

        [TestMethod]
        public void TestAddEditorWithUserAlreadyLinkedThroughEditorDoesNotSave()
        {
            #region Arrange
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            FakeEditors(2);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            UserRepository.Expect(a => a.GetNullableByID(2)).Return(Users[1]).Repeat.Any();

            Assert.AreEqual(1, Items[0].Editors.Count);
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(1, 2)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual("#Editors", result.Url);
            Assert.AreEqual(1, Items[0].Editors.Count);
            Assert.AreEqual(NotificationMessages.STR_EditorAlreadyExists, Controller.Message);
            #endregion Assert
        }


        #endregion AddEditor Tests
    }
}
