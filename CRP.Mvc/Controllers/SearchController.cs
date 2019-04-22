using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Abstractions;
using System.Linq;
using CRP.Core.Helpers;
using DotNetOpenAuth.Messaging;

namespace CRP.Controllers
{
    public class SearchController : ApplicationController
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

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var itemList = _searchTermProvider.GetByTerm(searchTerm).Where(a => a.Private == false && a.Available && a.Expiration != null && a.Expiration >= DateTime.UtcNow.AddDays(-15).ToPacificTime()).AsQueryable(); //.OrderByDescending(a => a.Expiration >= DateTime.UtcNow.ToPacificTime()).ThenBy(a => a.Expiration);
                var unexpiredItems = itemList
                    .Where(a => a.Available && !a.Private && a.Expiration != null && a.Expiration >= DateTime.UtcNow.ToPacificTime().Date)
                    .OrderBy(a => a.Expiration)
                    .ToList();

                var expiredItems = itemList
                    .Where(a => a.Available && !a.Private && a.Expiration != null && a.Expiration >= DateTime.UtcNow.AddDays(-15).ToPacificTime().Date && a.Expiration < DateTime.UtcNow.ToPacificTime().Date)
                    .OrderByDescending(a => a.Expiration)
                    .ToList();

                viewModel.Items = new List<Item>(unexpiredItems);
                viewModel.Items.AddRange(expiredItems);
            }


            return View(viewModel);
        }
    }
}