using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class LinkPaymentViewModel
    {
        public Transaction Transaction { get; set; }
        public IEnumerable<PaymentLog> PaymentLogs { get; set; }

        public static LinkPaymentViewModel Create(IRepository repository, Transaction transaction)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new LinkPaymentViewModel() {Transaction = transaction};

            return viewModel;
        }
    }
}
