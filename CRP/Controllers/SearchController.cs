using System;
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

        /// <summary>
        /// Jason changed this July 1, 2010 (Canada day) so that it does not show items that:
        ///     have no expiration date, or the expiration date is 15 or more days past (too old).
        /// Also, it orders them by expired on the bottom and then the ones that will expired soonest on top.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ActionResult Index(string searchTerm)
        {
            var viewModel = SearchViewModel.Create(Repository);

            if (!string.IsNullOrEmpty(searchTerm)) viewModel.Items = _searchTermProvider.GetByTerm(searchTerm).Where(a => a.Private == false && a.Available && a.Expiration != null && a.Expiration >= DateTime.Now.AddDays(-15)).OrderByDescending(a => a.Expiration >= DateTime.Now).ThenBy(a => a.Expiration);

            return View(viewModel);
        }
    }
}