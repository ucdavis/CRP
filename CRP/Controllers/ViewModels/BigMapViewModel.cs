using CRP.Core.Domain;

namespace CRP.Controllers.ViewModels
{
    public class BigMapViewModel
    {
        public bool UsePins { get; set; }
        public bool HasMapPins { get; set; }
        public Item Item { get; set; }

        public static BigMapViewModel Create(Item item)
        {
            var viewModel = new BigMapViewModel(){Item = item};
            if(viewModel.Item.MapPins != null && viewModel.Item.MapPins.Count > 0)
            {
                viewModel.HasMapPins = true;
            }
            else
            {
                viewModel.HasMapPins = false;
            }
            return viewModel;
        }

    }
}
