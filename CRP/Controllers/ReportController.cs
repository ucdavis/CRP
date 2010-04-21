using System;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class ReportController : SuperController
    {
        [Authorize(Roles="User")]
        public ActionResult ViewReport(int id, int itemId)
        {
            var itemReport = Repository.OfType<ItemReport>().GetNullableByID(id);
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            var viewModel = ReportViewModel.Create(Repository, itemReport, item);

            return View(viewModel);
        }
    }
}
