using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class LinkCheckViewModel
    {
        public Transaction Transaction { get; set; }
        public IEnumerable<PaymentLog> PaymentLogs { get; set; }

        public static LinkCheckViewModel Create(IRepository repository, Transaction transaction)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new LinkCheckViewModel() {Transaction = transaction};

            return viewModel;
        }
    }
}
