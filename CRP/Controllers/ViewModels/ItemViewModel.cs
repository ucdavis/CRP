using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemViewModel
    {
        public static ItemViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemViewModel(){ItemTypes = repository.OfType<ItemType>().Queryable.Where(a => a.IsActive).ToList()};

            return viewModel;
        }

        public IEnumerable<ItemType> ItemTypes { get; set; }
        public Item Item { get; set; }
    }
}
