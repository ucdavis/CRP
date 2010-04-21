using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Helpers;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class TransactionController : SuperController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;

        public TransactionController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository)
        {
            _openIdUserRepository = openIdUserRepository;
        }

        /// <summary>
        /// GET: /Transaction/Checkout/{id}
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public ActionResult Checkout(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null)
            {
                return this.RedirectToAction<ItemController>(a => a.List());
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name);
            return View(viewModel);
        }

        /// <summary>
        /// POST: /Transaction/Checkout/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Checks the shopper out for the item
        /// Assumption:
        ///     Item is valid (not expired, full and is available)
        /// PreCondition:
        ///     Item has not expired
        ///     Item is not full and has enough quantity to accept the checkout
        /// PostCondition:
        ///     Transaction item is created and "paid" field is marked false
        ///         At least Check or Credit field is true
        ///         Quantity answers are populated
        ///         Transaction answers are populated
        ///     If donation is present, seperate transaction record is created and linked to parent object
        ///         Donation field is marked true
        /// </remarks>
        /// <param name="id">Item Id</param>
        /// <param name="quantity"></param>
        /// <param name="transactionAnswers"></param>
        /// <param name="quantityAnswers"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Checkout(int id, int quantity, decimal? donation, string paymentType, string restrictedKey, string coupon, QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers)
        {
            #region DB Queries
            // get the item
            var item = Repository.OfType<Item>().GetNullableByID(id);

            // get all the questions in 1 queries
            var allQuestions = (from a in Repository.OfType<Question>().Queryable
                                where transactionAnswers.Select(b => b.QuestionId).ToArray().Contains(a.Id)
                                    || quantityAnswers.Select(b => b.QuestionId).ToArray().Contains(a.Id)
                                select a).ToList();

            // get the coupon
            var coup = Repository.OfType<Coupon>().Queryable.Where(a => a.Code == coupon && a.Item == item && a.IsActive).FirstOrDefault();
            #endregion

            // invalid item
            if (item == null)
            {
                return this.RedirectToAction<ItemController>(a => a.List());
            }

            var transaction = new Transaction(item);

            // deal with selected payment type
            if (paymentType == StaticValues.CreditCard)
            {
                transaction.Credit = true;
            }
            else if (paymentType == StaticValues.Check)
            {
                transaction.Check = true;
            }
            else
            {
                ModelState.AddModelError("Payment Type", "Payment type was not selected.");
            }

            
            // deal with the amount
            var amount = item.CostPerItem*quantity; // get the initial amount
            decimal discount = 0.0m;
            
            // get the email
            if (coup != null)
            {
                var emailQ = allQuestions.Where(a => a.Name == StaticValues.Question_Email && a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();
                if (emailQ != null)
                {
                    // get the answer
                    var answer = transactionAnswers.Where(a => a.QuestionId == emailQ.Id).FirstOrDefault();

                    discount = coup.UseCoupon(answer.Answer, quantity);
                }
                else
                {
                    discount = coup.UseCoupon(null, quantity);
                }
            }
            transaction.Amount = amount - discount;
            transaction.Quantity = quantity;


            // deal with the transaction answers
            foreach(var qa in transactionAnswers)
            {
                var question = allQuestions.Where(a => a.Id == qa.QuestionId).FirstOrDefault();
                // if question is null just drop it
                if (question != null)
                {
                    // check to make sure there is an answer if it's required
                    if (question.Required)
                    {
                        if (string.IsNullOrEmpty(qa.Answer))
                        {
                            // add a model error
                            ModelState.AddModelError("Transaction Question", question.Name + " requires an answer.");
                        }
                    }

                    var answer = new TransactionAnswer(transaction, question.QuestionSet, question, qa.Answer);
                    transaction.AddTransactionAnswer(answer);
                }
                //TODO: consider writing this to a log or something
            }

            // deal with quantity level answers
            for (var i = 0; i < quantity; i++ )
            {
                // generate the unique id for each quantity
                var quantityId = Guid.NewGuid();

                foreach(var qa in quantityAnswers.Where(a => a.QuantityIndex == i))
                {
                    var question = allQuestions.Where(a => a.Id == qa.QuestionId).FirstOrDefault();
                    // if question is null just drop it
                    if (question != null)
                    {
                        if (question.Required)
                        {
                            if (string.IsNullOrEmpty(qa.Answer))
                            {
                                ModelState.AddModelError("Quantity Question", question.Name + " for attendee " + (i + 1).ToString() + " requires an answer.");
                            }
                        }

                        var answer = new QuantityAnswer(transaction, question.QuestionSet, question, qa.Answer,
                                                        quantityId);
                        transaction.AddQuantityAnswer(answer);
                    }
                }
            }

            // deal with donation
            if (donation.HasValue && donation.Value > 0.0m)
            {
                var donationTransaction = new Transaction(item);
                donationTransaction.Donation = true;
                donationTransaction.Amount = donation.Value;

                transaction.AddChildTransaction(donationTransaction);
            }

            // check to see if it's a restricted item
            if (!string.IsNullOrEmpty(item.RestrictedKey) && item.RestrictedKey != restrictedKey)
            {
                ModelState.AddModelError("Restricted Key", "The item is restricted please enter the passphrase.");
            }
            
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                // create the new transaction
                Repository.OfType<Transaction>().EnsurePersistent(transaction);

                // redirect to confirmation and let the user decide payment or not
                return this.RedirectToAction(a => a.Confirmation(transaction.Id));
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name);
            //TODO: add the transaction to the viewmodel so the answers will be populated
            return View(viewModel);
        }

        
        /// <summary>
        /// GET: /Transaction/Confirmation/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     This is a confirmation page that displays a transaction confirmation
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Confirmation(int id)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableByID(id);

            if (transaction == null) return this.RedirectToAction<HomeController>(a => a.Index());

            var viewModel = PaymentConfirmationViewModel.Create(Repository, transaction);
            return View(viewModel);
        }

        /// <summary>
        /// POST: /Transaction/PaymentResult/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Deals with the return result from the payment gateway.
        /// </remarks>
        /// <param name="EXT_TRANS_ID"></param>
        /// <param name="PMT_STATUS"></param>
        /// <param name="PMT_AMT"></param>
        /// <param name="TPG_TRANS_ID"></param>
        /// <returns></returns>
        [AcceptPost]
        [BypassAntiForgeryToken]
        public ActionResult PaymentResult(int? EXT_TRANS_ID, string PMT_STATUS, string NAME_ON_ACT, decimal? PMT_AMT, string TPG_TRANS_ID, string CARD_TYPE)
        {
            if (EXT_TRANS_ID.HasValue)
            {
                var transaction = Repository.OfType<Transaction>().GetNullableByID(EXT_TRANS_ID.Value);

                var paymentLog = new PaymentLog(transaction.Total);
                paymentLog.Credit = true;

                if (PMT_STATUS == "success")
                {
                    paymentLog.Name = NAME_ON_ACT;
                    paymentLog.Amount = PMT_AMT.Value;
                    paymentLog.Accepted = true;
                    paymentLog.GatewayTransactionId = TPG_TRANS_ID;
                    paymentLog.CardType = CARD_TYPE;
                }

                transaction.AddPaymentLog(paymentLog);

                paymentLog.TransferValidationMessagesTo(ModelState);

                if (ModelState.IsValid)
                {
                    Repository.OfType<Transaction>().EnsurePersistent(transaction);

                    //TODO: send the shopper an email
                }
            }

            return View();
        }
    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public int QuestionSetId { get; set; }
        public int QuantityIndex { get; set; }
        public string Answer { get; set; }
    }
}
