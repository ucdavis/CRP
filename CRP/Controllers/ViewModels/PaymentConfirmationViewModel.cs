using System;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
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
        public string ValidationKey { get; set; }
        public string SiteId { get; set; }
        public string SuccessLink { get; set; } //The link we want touchnet to post back to on success
        public string CancelLink { get; set; } //If the user cancels in touchnet
        public string ErrorLink { get; set; } //Oops from touchnet
        public string Fid { get; set; }

        public static PaymentConfirmationViewModel Create(IRepository repository, Transaction transaction, string validationKey, HttpRequestBase request, UrlHelper url, string fid)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(url != null);
            Check.Require(request != null);

            var viewModel = new PaymentConfirmationViewModel() { Transaction = transaction
                , PaymentGatewayUrl = ConfigurationManager.AppSettings["PaymentGateway"]
                , ValidationKey = validationKey
                , SiteId = ConfigurationManager.AppSettings["TouchNetSiteId"]
                , Fid = fid
            };
            
            // get the proper display profile
            var unit = transaction.Item.Unit;
            viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.Unit == unit).FirstOrDefault();
            viewModel.SuccessLink = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Action("PaymentSuccess", "Transaction"));
            viewModel.CancelLink = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Action("PaymentCancel", "Transaction"));
            viewModel.ErrorLink = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Action("PaymentError", "Transaction"));
            return viewModel;
        }
    }
}
