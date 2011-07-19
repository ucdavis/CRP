using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region Map Tests

        [TestMethod]
        public void TestMapRedirectToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            Controller.Map(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));

            Assert.IsNull(Controller.Message);
        }

        [TestMethod]
        public void TestMapReturnsItemWhenIdFoundAndAdmin()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            Controller.Map(1)
                .AssertViewRendered()
                .WithViewData<Item>();

            Assert.IsNull(Controller.Message);

        }

        [TestMethod]
        public void TestMapReturnsItemWhenIdFoundAndUserIsAnEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            Users[1].LoginID = "UserName";
            FakeEditors(1);
            Editors[0].User = Users[1]; //User is editor
            Items[0].AddEditor(Editors[0]);

            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Map(1)
                .AssertViewRendered()
                .WithViewData<Item>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestMapRedirectToListIfNotAnEditorOrAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Map(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
    
        #endregion Map Tests

        #region Copy Tests

        [TestMethod]
        public void TestCopyRedirectToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            Controller.Copy(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
        }


        /// <summary>
        /// Tests the details redirect to list if not an editor or admin.
        /// </summary>
        [TestMethod]
        public void TestCopyRedirectToListIfNotAnEditorOrAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Copy(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCopyRedirectsToErrorControllerIfCopyIsNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();

            var inValidItem = CreateValidEntities.Item(99);
            inValidItem.Name = null;
            CopyItemService.Expect(a => a.Copy(Arg<Item>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<string>.Is.Anything))
                .Return(inValidItem).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Copy(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index(ErrorController.ErrorType.UnknownError));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ErrorController.ErrorType.UnknownError, result.RouteValues["errorType"]);
            Assert.AreEqual("The copy was not able to save because of Invalid Data", Controller.Message);

            #endregion Assert		
        }

        [TestMethod]
        public void TestCopyRedirectsToEditIfCopyIsValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();

            var validItem = CreateValidEntities.Item(99);
            validItem.SetIdTo(99);
            CopyItemService.Expect(a => a.Copy(Arg<Item>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<string>.Is.Anything))
                .Return(validItem).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Copy(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Edit(99));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.RouteValues["id"]);

            #endregion Assert
        }
        #endregion Copy Tests
    }
}
