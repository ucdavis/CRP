using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class BrowseItemsViewModel
    {
        public IQueryable<Item> Items { get; set; }
        public IEnumerable<Tag> Tags { get; set; }

        public static BrowseItemsViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new BrowseItemsViewModel()
                                {
                                    Items = repository.OfType<Item>().Queryable.Where(a => a.Available && !a.Private && a.Expiration != null && a.Expiration >= DateTime.Now.Date).OrderBy(a => a.Expiration),
                                    Tags = repository.OfType<Tag>().Queryable.OrderBy(a => a.Items.Count()).Take(2)
                                };

            return viewModel;
        }
    }
}
