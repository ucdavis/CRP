using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace CRP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            //return View();

            var viewModel = BrowseItemsViewModel.Create(Repository);
            return View(viewModel);
        }

        [Authorize]
        public ActionResult AdminHome()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
