using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using UCDArch.Web.Controller;
using CRP.Core.Abstractions;
using System.Linq;

namespace CRP.Controllers
{
    public class SearchController : SuperController
    {
        private readonly ISearchTermProvider _searchTermProvider;

        //
        // GET: /Search/
        public SearchController(ISearchTermProvider searchTermProvider)
        {
            _searchTermProvider = searchTermProvider;
        }

        public ActionResult Index(string searchTerm)
        {
            var viewModel = SearchViewModel.Create(Repository);

            if (!string.IsNullOrEmpty(searchTerm)) viewModel.Items = _searchTermProvider.GetByTerm(searchTerm).Where(a => a.Private == false && a.Available);

            return View(viewModel);
        }
    }
}