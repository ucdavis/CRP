using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemViewModel
    {
        public static ItemViewModel Create(IRepository repository, IPrincipal principal)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemViewModel(){
                ItemTypes = repository.OfType<ItemType>().Queryable.Where(a => a.IsActive).ToList(),
                Users = repository.OfType<User>().Queryable,
                CurrentUser = repository.OfType<User>().Queryable.Where(a => a.LoginID == principal.Identity.Name).FirstOrDefault()
            };

            if (principal.IsInRole(RoleNames.Admin))
            {
                viewModel.Units = repository.OfType<Unit>().GetAll();
            }
            else
            {
                viewModel.Units = viewModel.CurrentUser.Units;
            }

            return viewModel;
        }

        public IQueryable<User> Users { get; set; }
        public IEnumerable<ItemType> ItemTypes { get; set; }
        public Item Item { get; set; }
        public User CurrentUser { get; set; }
        public IEnumerable<Unit> Units { get; set; }
    }
}
