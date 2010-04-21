using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class CouponViewModel
    {
        public Coupon Coupon { get; set; }
        public Item Item { get; set; }

        public static CouponViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new CouponViewModel() {Item = item};

            return viewModel;
        }
    }
}
