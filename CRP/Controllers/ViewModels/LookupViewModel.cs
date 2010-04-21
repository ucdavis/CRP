using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Controllers.ViewModels
{
    public class LookupViewModel
    {
        public string Email { get; set; }
        public string TransactionNumber{ get; set; }
        public Transaction Transaction { get; set; }

        public static LookupViewModel Create (IRepository repository)
        {
            var viewModel = new LookupViewModel();

            return viewModel;
        }
    }
}
