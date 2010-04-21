using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
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
        private readonly INotificationProvider _notificationProvider;

        public TransactionController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository, INotificationProvider notificationProvider)
        {
            _openIdUserRepository = openIdUserRepository;
            _notificationProvider = notificationProvider;
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
            viewModel.Quantity = 1;
            viewModel.Answers = PopulateItemTransactionAnswer(viewModel.OpenIdUser, item.QuestionSets); // populate the open id stuff for transaction answer contact information
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
        [CaptchaValidatorAttribute]
        [AcceptPost]
        public ActionResult Checkout(int id, int quantity, decimal? donation, string paymentType, string restrictedKey, string coupon, QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers, bool captchaValid)
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

            // invalid item, or not available for registration
            if (item == null || !item.IsAvailableForReg)
            {
                return this.RedirectToAction<ItemController>(a => a.List());
            }

            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Captcha values are not valid.");
            }

            var transaction = new Transaction(item);

            // fill the openid user if they are openid validated
            if (HttpContext.Request.IsOpenId())
            {
                // doesn't matter if it's null, just assign what we have
                transaction.OpenIDUser = _openIdUserRepository.GetNullableByID(CurrentUser.Identity.Name);
            }

            // deal with selected payment type
            if (paymentType == StaticValues.CreditCard)
            {
                transaction.Credit = true;
                transaction.Check = false;
            }
            else if (paymentType == StaticValues.Check)
            {
                transaction.Check = true;
                transaction.Credit = false;
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
            foreach (var qa in transactionAnswers)
            {
                var question = allQuestions.Where(a => a.Id == qa.QuestionId).FirstOrDefault();

                // if question is null just drop it
                if (question != null)
                {

                    var answer = question.QuestionType.Name != QuestionTypeText.STR_CheckboxList
                                     ? qa.Answer
                                     : (qa.CblAnswer != null ? string.Join(", ", qa.CblAnswer) : string.Empty);

                    // validate each of the validators
                    foreach (var validator in question.Validators)
                    {
                        string message;
                        if (!Validate(validator, answer, question.Name, out message))
                        {
                            ModelState.AddModelError("Transaction Question", message);
                        }
                    }

                    var qanswer = new TransactionAnswer(transaction, question.QuestionSet, question, answer);
                    transaction.AddTransactionAnswer(qanswer);
                }
                //TODO: consider writing this to a log or something
            }
            
            // deal with quantity level answers
            for (var i = 0; i < quantity; i++)
            {
                // generate the unique id for each quantity
                var quantityId = Guid.NewGuid();

                foreach (var qa in quantityAnswers.Where(a => a.QuantityIndex == i))
                {
                    var question = allQuestions.Where(a => a.Id == qa.QuestionId).FirstOrDefault();
                    // if question is null just drop it
                    if (question != null)
                    {
                        var answer = question.QuestionType.Name != QuestionTypeText.STR_CheckboxList
                                         ? qa.Answer
                                         : (qa.CblAnswer != null ? string.Join(", ", qa.CblAnswer) : string.Empty);

                        var fieldName = question.Name + " for attendee " + (i + 1);

                        // validate each of the validators
                        foreach (var validator in question.Validators)
                        {
                            string message;
                            if (!Validate(validator, answer, fieldName, out message))
                            {
                                ModelState.AddModelError("Quantity Question", message);
                            }
                        }

                        var qanswer = new QuantityAnswer(transaction, question.QuestionSet, question, answer,
                                                         quantityId);
                        transaction.AddQuantityAnswer(qanswer);
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
            
            // do a final check to make sure the inventory is there
            if (item.Sold + quantity > item.Quantity)
            {
                ModelState.AddModelError("Quantity", "There is not enough inventory to complete your order.");
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
            viewModel.Quantity = quantity;
            viewModel.Answers = PopulateItemTransactionAnswer(transactionAnswers, quantityAnswers);
            viewModel.CreditPayment = (paymentType == StaticValues.CreditCard);
            viewModel.CheckPayment = (paymentType == StaticValues.Check);
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
            string postingString = ConfigurationManager.AppSettings["TouchNetPostingKey"]; 
            var validationKey = CalculateValidationString(postingString, transaction.Id.ToString(), transaction.Total.ToString());
            var viewModel = PaymentConfirmationViewModel.Create(Repository, transaction, validationKey, Request, Url);
            return View(viewModel);
        }
        /// <summary>
        /// Calculates the validation string.
        /// </summary>
        /// <param name="PostingKey">The posting key.</param>
        /// <param name="EXT_TRANS_ID">The TransactionId</param>
        /// <param name="AMT">The AMT.</param>
        /// <returns></returns>
        public static string CalculateValidationString(string PostingKey, string EXT_TRANS_ID, string AMT)
        {
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(PostingKey + EXT_TRANS_ID + AMT));
            return Convert.ToBase64String(data);
        }
        public ActionResult PaymentSuccess(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            throw new NotImplementedException();
        }
        public ActionResult PaymentCancel(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            throw new NotImplementedException();
        }

        public ActionResult PaymentError(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            throw new NotImplementedException();
        }

        [AnyoneWithRole]
        public ActionResult Edit(int id)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableByID(id);
            if(transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }
            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                if (transaction.Item == null)
                {
                    return this.RedirectToAction<ItemManagementController>(a => a.List());
                }
                return this.RedirectToAction<ItemManagementController>(a => a.Details(transaction.Item.Id));
            }
            var viewModel = EditTransactionViewModel.Create(Repository);
            viewModel.TransactionValue = transaction;
            viewModel.ContactName =
                transaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;
            viewModel.ContactName = viewModel.ContactName + " " + transaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
            viewModel.ContactEmail = transaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email).FirstOrDefault().Answer;
            return View(viewModel);
        }

        /// <summary>
        /// POST: /Transaction/Edit/{id}
        /// </summary>
        /// <param name="id">id of the original parent transaction</param>
        /// <param name="transaction">the new correction transaction</param>
        /// <returns></returns>
        [AcceptPost]
        [AnyoneWithRole]
        public ActionResult Edit(int id, [Bind(Exclude="Id")]Transaction transaction)
        {
            // load the original transaction
            var origTransaction = Repository.OfType<Transaction>().GetNullableByID(id);
            if (origTransaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }
            if (origTransaction.Item == null || !Access.HasItemAccess(CurrentUser, origTransaction.Item))
            {
                if (origTransaction.Item == null)
                {
                    return this.RedirectToAction<ItemManagementController>(a => a.List());
                }
                return this.RedirectToAction<ItemManagementController>(a => a.Details(origTransaction.Item.Id));
            }

            // create the new transaction
            var correctionTransaction = new Transaction(origTransaction.Item);
            correctionTransaction.ParentTransaction = origTransaction;
            correctionTransaction.Amount = transaction.Amount;
            correctionTransaction.CorrectionReason = transaction.CorrectionReason;
            correctionTransaction.Donation = true;
            //if(correctionTransaction.Amount > 0)
            //{
            //    correctionTransaction.Donation = true;
            //}
            //else
            //{
            //    correctionTransaction.Donation = false;
            //}
            correctionTransaction.CreatedBy = CurrentUser.Identity.Name;

            //origTransaction.AddChildTransaction(correctionTransaction);

            //origTransaction.TransferValidationMessagesTo(ModelState);

            // validate the correction transaction
            correctionTransaction.TransferValidationMessagesTo(ModelState);

            if(ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(correctionTransaction);
                return this.RedirectToAction<ItemManagementController>(a => a.Details(origTransaction.Item.Id));
            }

            var viewModel = EditTransactionViewModel.Create(Repository);
            viewModel.TransactionValue = origTransaction;
            viewModel.ContactName =
                origTransaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;
            viewModel.ContactName = viewModel.ContactName + " " + origTransaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
            viewModel.ContactEmail = origTransaction.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email).FirstOrDefault().Answer;
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
            // validate to make sure a transaction value was received
            if (EXT_TRANS_ID.HasValue)
            {
                var transaction = Repository.OfType<Transaction>().GetNullableByID(EXT_TRANS_ID.Value);

                // create a payment log
                var paymentLog = new PaymentLog(transaction.Total);
                paymentLog.Credit = true;

                // on success, save the valid information
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

                    // attempt to get the contact information question set and retrieve email address
                    var question = transaction.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_Email).FirstOrDefault();
                    if (question != null)
                    {
                        // send an email to the user
                        _notificationProvider.SendConfirmation(transaction, question.Answer);
                    }
                }
            }

            return View();
        }


        /// <summary>
        /// Compares an answer against a regular expression
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        private bool Validate(Validator validator, string answer, string fieldName, out string message)
        {
            // set as default so we can return without having to set it individually
            message = string.Empty;

            // check to make sure we have a reg ex
            if (string.IsNullOrEmpty(validator.RegEx)) return true;

            var regExVal = new Regex(validator.RegEx);
            // valid
            // check for when answer is null, becuase when doing a radio button it is null when nothing is selected
            if (regExVal.IsMatch(answer ?? string.Empty)) return true;

            // not valid input provide error message
            message = string.Format(validator.ErrorMessage, fieldName);
            return false;
        }

        private IEnumerable<ItemTransactionAnswer> PopulateItemTransactionAnswer(OpenIdUser openIdUser, ICollection<ItemQuestionSet> questionSets)
        {
            var answers = new List<ItemTransactionAnswer>();

            // if anything is null, just return no answers
            if (openIdUser == null || questionSets == null) return answers;

            // find the contact information question set
            var questionSet = questionSets.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation).Select(a => a.QuestionSet).FirstOrDefault();

            // if it exists, fill in the questions
            if (questionSet != null)
            {
                foreach(var question in questionSet.Questions)
                {
                    var ans = string.Empty;

                    if (question.Name == StaticValues.Question_FirstName)
                    {
                        ans = openIdUser.FirstName;
                    }
                    else if (question.Name == StaticValues.Question_LastName)
                    {
                        ans = openIdUser.LastName;
                    }
                    else if (question.Name == StaticValues.Question_StreetAddress)
                    {
                        ans = openIdUser.StreetAddress;
                    }
                    else if (question.Name == StaticValues.Question_AddressLine2)
                    {
                        ans = openIdUser.Address2;
                    }
                    else if (question.Name == StaticValues.Question_City)
                    {
                        ans = openIdUser.City;
                    }
                    else if (question.Name == StaticValues.Question_State)
                    {
                        ans = openIdUser.State;
                    }
                    else if (question.Name == StaticValues.Question_Zip)
                    {
                        ans = openIdUser.Zip;
                    }
                    else if (question.Name == StaticValues.Question_PhoneNumber)
                    {
                        ans = openIdUser.PhoneNumber;
                    }
                    else if (question.Name == StaticValues.Question_Email)
                    {
                        ans = openIdUser.Email;
                    }

                    // create the answer object
                    var answer = new ItemTransactionAnswer()
                    {
                        Answer = ans,
                        QuestionId = question.Id,
                        QuestionSetId = question.QuestionSet.Id,
                        Transaction = true
                    };

                    answers.Add(answer);
                }
            }

            return answers;
        }
        private IEnumerable<ItemTransactionAnswer> PopulateItemTransactionAnswer(QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers)
        {
            var answers = new List<ItemTransactionAnswer>();

            foreach (var qap in transactionAnswers)
            {
                var question = Repository.OfType<Question>().GetNullableByID(qap.QuestionId);
                var answer = qap.Answer;

                if (question != null)
                {
                    if (question.QuestionType.Name == QuestionTypeText.STR_CheckboxList && qap.CblAnswer != null)
                        answer = string.Join(",", qap.CblAnswer);
                }

                var a = new ItemTransactionAnswer()
                            {
                                Answer = answer,
                                QuestionId = qap.QuestionId,
                                QuestionSetId = qap.QuestionSetId,
                                Transaction = true
                            };

                answers.Add(a);
            }

            foreach (var qap in quantityAnswers)
            {
                var question = Repository.OfType<Question>().GetNullableByID(qap.QuestionId);
                var answer = qap.Answer;

                if (question != null)
                {
                    if (question.QuestionType.Name == QuestionTypeText.STR_CheckboxList && qap.CblAnswer != null)
                        answer = string.Join(",", qap.CblAnswer);
                }

                var a = new ItemTransactionAnswer()
                            {
                                Answer = answer,
                                QuestionId = qap.QuestionId,
                                QuestionSetId = qap.QuestionSetId,
                                QuantityIndex = qap.QuantityIndex,
                                Transaction = false
                            };

                answers.Add(a);
            }

            return answers;
        }


        public ActionResult Lookup()
        {
            return View(LookupViewModel.Create(Repository));
        }

        [AcceptPost]
        public ActionResult Lookup(string orderNumber, string email)
        {
            var transaction = Repository.OfType<Transaction>().Queryable.Where(a => a.TransactionNumber == orderNumber).FirstOrDefault();

            var viewModel = LookupViewModel.Create(Repository);
            if (transaction != null)
            {
                var answer = transaction.TransactionAnswers.Where(
                                a =>
                                a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                                a.Question.Name == StaticValues.Question_Email).FirstOrDefault();

                if (answer != null && answer.Answer == email)
                {
                    viewModel.TransactionNumber = orderNumber;
                    viewModel.Email = email;
                    viewModel.Transaction = transaction;
                }
            }

            else
            {
                viewModel.TransactionNumber = orderNumber;
                viewModel.Email = email;

                Message = "Unable to locate order, please check your information and try again.";
            }

            return View(viewModel);
        }


    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public int QuestionSetId { get; set; }
        public int QuantityIndex { get; set; }
        public string Answer { get; set; }

        public string[] CblAnswer { get; set; }
    }
}
