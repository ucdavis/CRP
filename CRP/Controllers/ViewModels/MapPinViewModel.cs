using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class MapPinViewModel
    {
        public MapPin MapPin { get; set; }
        public Item Item { get; set; }

        public static MapPinViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new MapPinViewModel() { Item = item };

            return viewModel;
        }
    }
}
