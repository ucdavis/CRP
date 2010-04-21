using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using Resources;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class CheckController : SuperController
    {
        /// <summary>
        /// GET: /Check/LinkToTransaction/{id}
        /// </summary>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns></returns>
        public ActionResult LinkToTransaction(int transactionId)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableByID(transactionId);
            if (transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            var viewModel = LinkCheckViewModel.Create(Repository, transaction);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult LinkToTransaction(int transactionId, Check[] Checks)
        {
            // get the transaction
            var transaction = Repository.OfType<Transaction>().GetNullableByID(transactionId);
            if (transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            // go through and process the checks
            foreach(var check in Checks)
            {
                // new check
                if (check.Id <= 0)
                {
                    transaction.AddCheck(check);
                }
            }

            // figure out the total of the checks
            var checkTotal = Checks.Sum(a => a.Amount);
            var transactionTotal = transaction.Amount + transaction.ChildTransactions.Sum(a => a.Amount);

            // more money is coming in than the transaction total, make a donation for the rest
            if (checkTotal > transactionTotal)
            {
                var donation = new Transaction(transaction.Item);
                donation.Donation = true;
                donation.Amount = checkTotal - transactionTotal;

                transaction.AddChildTransaction(donation);
            }

            if (checkTotal >= transactionTotal)
            {
                transaction.Paid = true;
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Message = "Checks associated with transaction.";
                //return Redirect(ReturnUrlGenerator.DetailItemUrl(transaction.Item.Id, StaticValues.Tab_Checks));
                return Redirect(Url.DetailItemUrl(transaction.Item.Id, StaticValues.Tab_Checks));
            }

            var viewModel = LinkCheckViewModel.Create(Repository, transaction);

            return View(viewModel);
        }
    }
}
