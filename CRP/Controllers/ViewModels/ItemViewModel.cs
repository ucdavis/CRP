using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemViewModel
    {
        public static ItemViewModel Create(IRepository repository, string loginId)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemViewModel(){
                ItemTypes = repository.OfType<ItemType>().Queryable.Where(a => a.IsActive).ToList(),
                Users = repository.OfType<User>().Queryable,
                CurrentUser = repository.OfType<User>().Queryable.Where(a => a.LoginID == loginId).FirstOrDefault()
            };

            return viewModel;
        }

        public IQueryable<User> Users { get; set; }
        public IEnumerable<ItemType> ItemTypes { get; set; }
        public Item Item { get; set; }
        public User CurrentUser { get; set; }
    }
}
