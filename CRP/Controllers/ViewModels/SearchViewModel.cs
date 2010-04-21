using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace CRP.Controllers.ViewModels
{
    public class SearchViewModel
    {
        public string Suggestion { get; set; }
        public ICollection<Item> Items { get; set; }

        public static SearchViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new SearchViewModel();

            return viewModel;
        }
    }
}
