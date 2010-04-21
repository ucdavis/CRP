using System;
using System.Linq;
using System.Web.Mvc;
//using CRP.App_GlobalResources;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using MvcContrib;
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

        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View();
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
            
            //TODO: deal with coupon codes
            // coupon is valid code for this item

            // get the email
            var emailQ = allQuestions.Where(a => a.Name == StaticValues.Question_Email && a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();
            if (emailQ != null)
            {
                // get the answer
                var answer = transactionAnswers.Where(a => a.QuestionId == emailQ.Id).FirstOrDefault();

                if (coup != null)
                {
                    discount = coup.UseCoupon(answer.Answer, quantity);
                }
            }
            else
            {
                discount = coup.UseCoupon(null, quantity);
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

                foreach(var qa in quantityAnswers)
                {
                    var question = allQuestions.Where(a => a.Id == qa.QuestionId).FirstOrDefault();
                    // if question is null just drop it
                    if (question != null)
                    {
                        var answer = new QuantityAnswer(transaction, question.QuestionSet, question, qa.Answer,
                                                        quantityId);
                        transaction.AddQuantityAnswer(answer);
                    }
                }
            }

            // deal with donation
            if (donation.HasValue)
            {
                var donationTransaction = new Transaction(item);
                donationTransaction.Donation = true;
                donationTransaction.Amount = donation.Value;

                transaction.AddChildTransaction(donationTransaction);
            }

            // check to see if it's a restricted item
            if (!string.IsNullOrEmpty(item.RestrictedKey) && item.RestrictedKey != restrictedKey)
            {
                ModelState.AddModelError("Restricted Key", "The item is restricted.");
            }
            
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transaction);

                if (transaction.Credit)
                {
                    //TODO: redirect to touchnet to take payment


                }
                else
                {
                    // TODO: what do we want to do if they pay by check, check confirmation screen?
                }

                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemController>(a => a.Details(item.Id));
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name);
            //TODO: add the transaction to the viewmodel so the answers will be populated
            return View(viewModel);
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
