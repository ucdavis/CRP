using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using CRP.Controllers.ViewModels;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class SearchController : SuperController
    {
        //
        // GET: /Search/

        public ActionResult Index(string searchTerm)
        {
            var viewModel = SearchViewModel.Create(Repository);
            return View(viewModel);
        }
    }
}