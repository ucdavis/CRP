using System.Web.Mvc;
using CRP.Controllers.Filter;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace CRP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
