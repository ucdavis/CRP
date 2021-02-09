using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class BrowseItemsViewModel
    {
        public IList<Item> Items { get; set; }
        public IEnumerable<Tag> Tags { get; set; }

        public static BrowseItemsViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new BrowseItemsViewModel()
                                {                                    
                                    Tags = repository.OfType<Tag>().Queryable.OrderBy(a => a.Items.Count()).Take(2)
                                };

            var unexpiredItems =
                repository.OfType<Item>()
                    .Queryable
                    .Where(a => a.Available && !a.Private && (a.Expiration == null || a.Expiration >= DateTime.UtcNow.ToPacificTime().Date))
                    .OrderBy(a => a.Expiration)
                    .ToList();

            var expiredItems = repository.OfType<Item>()
                    .Queryable
                    .Where(a => a.Available && !a.Private && a.Expiration != null && a.Expiration >= DateTime.UtcNow.ToPacificTime().AddDays(-15).Date && a.Expiration < DateTime.UtcNow.ToPacificTime().Date)
                    .OrderByDescending(a => a.Expiration)
                    .ToList();
            
            //viewModel.Items = new List<Item>();
            //foreach (var unexpiredItem in unexpiredItems)
            //{
            //    viewModel.Items.Add(unexpiredItem);
            //}
            //foreach (var expiredItem in expiredItems)
            //{
            //    viewModel.Items.Add(expiredItem);
            //}
            viewModel.Items = new List<Item>(unexpiredItems);
            foreach (var expiredItem in expiredItems)
            {
                viewModel.Items.Add(expiredItem);
            }
            return viewModel;
        }
    }
}
