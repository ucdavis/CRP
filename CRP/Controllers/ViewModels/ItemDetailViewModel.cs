using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemDetailViewModel
    {
        public Item Item { get; set; }
        public DisplayProfile DisplayProfile { get; set; }

        public static ItemDetailViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemDetailViewModel() {Item = item};

            // get the proper display profile
            var unit = item.Unit;

            viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.Unit == unit).FirstOrDefault();

            if (viewModel.DisplayProfile == null)
            {
                // get the college profile
                viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.School == unit.School && a.SchoolMaster).FirstOrDefault();
            }

            return viewModel;
        }
    }

    public class UserItemDetailViewModel
    {
        public Item Item { get; set; }

        public static UserItemDetailViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new UserItemDetailViewModel() { Item = item };

            return viewModel;
        }
    }
}
