using System.Web.Mvc;
using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region RemoveEditor Tests

        [TestMethod]
        public void TestRemoveEditorWithValidDataSaves()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeEditors(3);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);

            Editors[0].User = Users[0];
            Editors[0].Item = Items[0];
            Items[0].Editors.Add(Editors[0]);

            Editors[2].User = Users[2];
            Editors[2].Item = Items[0];
            Items[0].Editors.Add(Editors[2]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(1)).Return(Editors[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(2)).Return(Editors[1]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(3)).Return(Editors[2]).Repeat.Any();

            var result = Controller.RemoveEditor(1, 1)
                .AssertHttpRedirect();
            Assert.AreEqual("#Editors", result.Url);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual(2, Items[0].Editors.Count);
            Assert.IsFalse(Items[0].Editors.Contains(Editors[0]));
            Assert.AreEqual("Editor has been saved successfully.", Controller.Message);
        }

        [TestMethod]
        public void TestRemoveEditorWillNotRemoveOwner()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeEditors(3);
            Editors[1].Owner = false;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);

            Editors[0].Owner = true;
            Editors[0].User = Users[0];
            Editors[0].Item = Items[0];
            Items[0].Editors.Add(Editors[0]);

            Editors[2].User = Users[2];
            Editors[2].Item = Items[0];
            Items[0].Editors.Add(Editors[2]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(1)).Return(Editors[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(2)).Return(Editors[1]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(3)).Return(Editors[2]).Repeat.Any();

            var result = Controller.RemoveEditor(1, 1)
                .AssertHttpRedirect();
            Assert.AreEqual("#Editors", result.Url);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreEqual(3, Items[0].Editors.Count);
            Assert.IsTrue(Items[0].Editors.Contains(Editors[0]));
            Assert.AreEqual("Can not remove owner from item.", Controller.Message);
        }

        [TestMethod]
        public void TestRemoveEditorDoesNotSaveWhenCurrentUserIsNotAnEditor()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 4);
            Users[3].LoginID = Controller.CurrentUser.Identity.Name; //This user not added to editors
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeEditors(3);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);


            Editors[0].User = Users[0];
            Editors[0].Item = Items[0];
            Items[0].Editors.Add(Editors[0]);

            Editors[2].User = Users[2];
            Editors[2].Item = Items[0];
            Items[0].Editors.Add(Editors[2]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(1)).Return(Editors[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(2)).Return(Editors[1]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(3)).Return(Editors[2]).Repeat.Any();

            Controller.RemoveEditor(1, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreEqual(3, Items[0].Editors.Count);
            Assert.IsTrue(Items[0].Editors.Contains(Editors[0]));
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
        }


        [TestMethod]
        public void TestRemoveEditorWillNotSaveWithInvalidData()
        {
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            ControllerRecordFakes.FakeItems(Items, 1);
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeEditors(3);
            Editors[1].Owner = true;
            Editors[1].User = Users[1];
            Editors[1].Item = Items[0];
            Items[0].Editors.Add(Editors[1]);

            Editors[0].User = Users[0];
            Editors[0].Item = Items[0];
            Items[0].Editors.Add(Editors[0]);

            Editors[2].User = Users[2];
            Editors[2].Item = Items[0];
            Items[0].Editors.Add(Editors[2]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(1)).Return(Editors[0]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(2)).Return(Editors[1]).Repeat.Any();
            EditorRepository.Expect(a => a.GetNullableByID(3)).Return(Editors[2]).Repeat.Any();

            Items[0].Name = " "; //Invalid

            var result = Controller.RemoveEditor(1, 1)
                .AssertHttpRedirect();
            Assert.AreEqual("#Editors", result.Url);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreNotEqual("Editor has been saved successfully.", Controller.Message);

            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        #endregion RemoveEditor Tests
    }
}
