using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

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
    }
}
