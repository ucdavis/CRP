using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Web.ActionResults;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region SaveTemplate Tests

        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            var result = Controller.SaveTemplate(1, "Test")
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfTextIsNull()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            var result = Controller.SaveTemplate(1, null)
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfTextIsEmpty()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            var result = Controller.SaveTemplate(1, string.Empty)
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }
        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfTextIsSpacesOnly()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[1];
            Items[0].AddEditor(Editors[0]);

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            var result = Controller.SaveTemplate(1, " ")
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            Controller.ModelState.AssertErrorsAre("Text: may not be null or empty");
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfTextIsSpacesOnlyAndItemIsInvalid()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[1];
            Items[0].AddEditor(Editors[0]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            Items[0].Name = " "; //Also invalid
            var result = Controller.SaveTemplate(1, " ")
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            Controller.ModelState.AssertErrorsAre("Text: may not be null or empty"
                                                  , "Name: may not be null or empty");
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfTextIsValidAndItemIsInvalid()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            Items[0].Name = " "; //Also invalid
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[1];
            Items[0].AddEditor(Editors[0]);
            var result = Controller.SaveTemplate(1, "test")
                .AssertResultIs<JsonNetResult>();
            Assert.IsFalse((bool)result.Data);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        [TestMethod]
        public void TestSaveTemplateSavesWithValidData()
        {
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[1];
            Items[0].AddEditor(Editors[0]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            Assert.AreEqual(0, Items[0].Templates.Count);
            var result = Controller.SaveTemplate(1, "test")
                .AssertResultIs<JsonNetResult>();
            Assert.IsTrue((bool)result.Data);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual(1, Items[0].Templates.Count);
            Assert.AreEqual("test", Items[0].Templates.ToList()[0].Text);
        }


        /// <summary>
        /// Tests the save template does not save if user is not an editor.
        /// </summary>
        [TestMethod]
        public void TestSaveTemplateDoesNotSaveIfUserIsNotAnEditor()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[2]; //Current user is not an editor.
            Items[0].AddEditor(Editors[0]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SaveTemplate(1, "test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsFalse((bool)result.Data);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreNotEqual(1, Items[0].Templates.Count);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the save template saves if user is not an editor but is admin.
        /// </summary>
        [TestMethod]
        public void TestSaveTemplateSavesIfUserIsNotAnEditorButIsAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true); // Admin
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[2]; //Current user is not an editor.
            Items[0].AddEditor(Editors[0]);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SaveTemplate(1, "test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsTrue((bool)result.Data);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual(1, Items[0].Templates.Count);
            Assert.AreEqual("test", Items[0].Templates.ToList()[0].Text);
            #endregion Assert
        }

        #endregion SaveTemplate Tests
    }
}
