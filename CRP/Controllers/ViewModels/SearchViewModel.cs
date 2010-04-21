using System.Collections.Generic;
using System.Configuration;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace CRP.Controllers.ViewModels
{
    public class SearchViewModel
    {
        public string ApiKey { get; set; }
        public IEnumerable<Item> Items { get; set; }

        public static SearchViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new SearchViewModel()
                                {
                                    Items = new List<Item>(),
                                    ApiKey = ConfigurationManager.AppSettings["BingApiKey"]
                                };

            return viewModel;
        }
    }
}
