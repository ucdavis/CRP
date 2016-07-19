using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using System.Linq;


namespace CRP.Controllers.ViewModels
{
    public class LookupViewModel
    {
        public string Email { get; set; }
        public string TransactionNumber{ get; set; }
        public Transaction Transaction { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show credit card re submit].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if credit card, not paid, and at least one paymentLog with a canceled or error status; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCreditCardReSubmit { get; set; }

        public static LookupViewModel Create (IRepository repository)
        {
            var viewModel = new LookupViewModel();
            viewModel.ShowCreditCardReSubmit = false;            
            return viewModel;
        }
    }
}
