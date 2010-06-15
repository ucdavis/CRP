using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.Details(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
        }

        [TestMethod]
        public void TestDetailsReturnsUserItemDetailViewModelWhenIdFoundAndAdmin()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, true);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<UserItemDetailViewModel>();

        }

        [TestMethod]
        public void TestDetailsReturnsUserItemDetailViewModelWhenIdFoundAndUserIsAnEditor()
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

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<UserItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the details redirect to list if not an editor or admin.
        /// </summary>
        [TestMethod]
        public void TestDetailsRedirectToListIfNotAnEditorOrAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, false);
            ControllerRecordFakes.FakeItems(Items, 1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            #endregion Assert		
        }
        #endregion Details Tests
    }
}
