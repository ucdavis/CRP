using System.Configuration;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class PaymentConfirmationViewModel
    {
        public Transaction Transaction { get; set; }
        public DisplayProfile DisplayProfile { get; set; }
        public string PaymentGatewayUrl { get; set; }

        public static PaymentConfirmationViewModel Create(IRepository repository, Transaction transaction)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new PaymentConfirmationViewModel() { Transaction = transaction, PaymentGatewayUrl = ConfigurationManager.AppSettings["PaymentGateway"] };

            // get the proper display profile
            var unit = transaction.Item.Unit;
            viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.Unit == unit).FirstOrDefault();

            return viewModel;
        }
    }
}
