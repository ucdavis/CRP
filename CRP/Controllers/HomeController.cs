using System.Web.Mvc;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace CRP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
