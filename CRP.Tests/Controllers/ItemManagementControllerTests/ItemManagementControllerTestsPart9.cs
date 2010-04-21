using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
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
                .ToAction<ItemManagementController>(a => a.List());
        }

        [TestMethod]
        public void TestDetailsReturnsUserItemDetailViewModelWhenIdFound()
        {
            FakeItems(1);
            FakeItemReports(1);
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();
            ItemReportRepository.Expect(a => a.Queryable).Return(ItemReports.AsQueryable()).Repeat.Any();
            Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<UserItemDetailViewModel>();

        }
        #endregion Details Tests
    }
}
