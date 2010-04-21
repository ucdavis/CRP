using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class PaymentController : SuperController
    {
        /// <summary>
        /// GET: /Check/LinkToTransaction/{id}
        /// </summary>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns></returns>
        public ActionResult LinkToTransaction(int transactionId)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableByID(transactionId);
            if (transaction == null) return this.RedirectToAction<ItemManagementController>(a => a.List());

            var viewModel = LinkPaymentViewModel.Create(Repository, transaction);
            viewModel.PaymentLogs = transaction.PaymentLogs.Where(a => a.Check);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult LinkToTransaction(int transactionId, PaymentLog[] Checks)
        {
            // get the transaction
            var transaction = Repository.OfType<Transaction>().GetNullableByID(transactionId);
            if (transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            // go through and process the checks
            foreach (var check in Checks)
            {
                PaymentLog paymentLog;

                if (check.Id <= 0 && check.Accepted && (string.IsNullOrEmpty(check.Name) || check.Amount <= 0.0m))
                {
                    ModelState.AddModelError("Check", "At least one check is invalid or incomplete");
                }

                // new check that is considered accepted
                if (check.Id <= 0 && check.Accepted && !string.IsNullOrEmpty(check.Name) && check.Amount > 0.0m)
                {
                    paymentLog = Copiers.CopyCheckValues(check, new PaymentLog());
                    paymentLog.Check = true;
                    transaction.AddPaymentLog(paymentLog);
                }
                // update an existing one
                else if (check.Id > 0)
                {
                    var tempCheck = Repository.OfType<PaymentLog>().GetNullableByID(check.Id);
                    paymentLog = Copiers.CopyCheckValues(check, tempCheck);
                }
            }

            //figure out the total of the checks
            var checktotal = transaction.PaymentLogs.Where(a => a.Accepted).Sum(a => a.Amount);
            var transactiontotal = transaction.Total;

            // more money is coming in than the transaction total, make a donation for the rest
            if (checktotal > transactiontotal)
            {
                var donation = new Transaction(transaction.Item);
                donation.Donation = true;
                donation.Amount = checktotal - transactiontotal;

                transaction.AddChildTransaction(donation);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Message = "Checks associated with transaction.";
                return Redirect(Url.DetailItemUrl(transaction.Item.Id, StaticValues.Tab_Checks));
            }

            var viewModel = LinkPaymentViewModel.Create(Repository, transaction);
            viewModel.PaymentLogs = transaction.PaymentLogs.Where(a => a.Check);

            return View(viewModel);
        }
    }
}
