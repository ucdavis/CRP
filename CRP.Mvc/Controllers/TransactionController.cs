using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class TransactionController : ApplicationController
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
        /// <param name="referenceId">Reference Number for external applications</param>
        /// <param name="coupon"></param>
        /// <param name="password"> </param>
        /// <param name="agribusinessExtraParams"></param>
        /// <returns></returns>
        public ActionResult Checkout(int id, string referenceId, string coupon, string password, AgribusinessExtraParams agribusinessExtraParams = null)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);

            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name, referenceId, coupon, password);
            viewModel.Quantity = 1;
            viewModel.Answers = PopulateItemTransactionAnswer(viewModel.OpenIdUser, item.QuestionSets); // populate the open id stuff for transaction answer contact information
            if(!viewModel.Answers.Any())
            {
                viewModel.Answers = PopulateItemTransactionAnswer(agribusinessExtraParams, item.QuestionSets);
            }
            viewModel.TotalAmountToRedisplay = viewModel.Quantity*item.CostPerItem;
            viewModel.CouponAmountToDisplay = 0.0m; //They have not entered a coupon yet
            viewModel.CouponTotalDiscountToDisplay = 0.0m;
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
        ///     If donation is present, separate transaction record is created and linked to parent object
        ///         Donation field is marked true
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="referenceIdHidden"></param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="donation">The donation.</param>
        /// <param name="displayAmount">total amount calculated on the form</param>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="restrictedKey">The restricted key.</param>
        /// <param name="coupon">The coupon.</param>
        /// <param name="transactionAnswers">The transaction answers.</param>
        /// <param name="quantityAnswers">The quantity answers.</param>
        /// <param name="captchaValid">if set to <c>true</c> [captcha valid].</param>
        /// <returns></returns>
        [CaptchaValidator]
        [HttpPost]
        public ActionResult Checkout(int id, string referenceIdHidden, int quantity, decimal? donation, decimal? displayAmount, string paymentType, string restrictedKey, string coupon, QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers, bool captchaValid)
        {
            // if the arrays are null create new blank ones
            if (transactionAnswers==null) transactionAnswers = new QuestionAnswerParameter[0];
            if (quantityAnswers==null) quantityAnswers = new QuestionAnswerParameter[0];


            #region DB Queries
            // get the item
            var item = Repository.OfType<Item>().GetNullableById(id);



            // get all the questions in 1 queries
            var questionIds = transactionAnswers.Select(b => b.QuestionId).ToList().Union(quantityAnswers.Select(c => c.QuestionId).ToList()).ToArray();
            var allQuestions = Repository.OfType<Question>().Queryable.Where(a => questionIds.Contains(a.Id)).ToList();

            if(!string.IsNullOrWhiteSpace(referenceIdHidden))
            {
                var refId = allQuestions.FirstOrDefault(a => a.Name == "Reference Id");
                if(refId != null)
                {
                    if(transactionAnswers.Any(a => a.QuestionId == refId.Id && string.IsNullOrWhiteSpace(a.Answer)))
                    {
                        transactionAnswers.First(a => a.QuestionId == refId.Id && string.IsNullOrWhiteSpace(a.Answer)).Answer = referenceIdHidden;
                    }
                }
            }

            // get the coupon
            var coup = Repository.OfType<Coupon>().Queryable.Where(a => a.Code == coupon && a.Item == item && a.IsActive).FirstOrDefault();
            #endregion

            // invalid item, or not available for registration
            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            if (!Access.HasItemAccess(CurrentUser, item)) //Allow editors to over ride and register for things
            {
                if (!item.IsAvailableForReg)
                {
                    Message = NotificationMessages.STR_NotAvailable.Replace(NotificationMessages.ObjectType, "Item");
                    return this.RedirectToAction<HomeController>(a => a.Index());
                }
            }

            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Captcha values are not valid.");
            }

            if(quantity < 1 )
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1");
            }

            var transaction = new Transaction(item);

            var questionCount = 0;
            foreach (var itemQuestionSet in item.QuestionSets.Where(a => a.QuantityLevel))
            {
                questionCount += itemQuestionSet.QuestionSet.Questions.Count;
            }
            if (questionCount * quantity != quantityAnswers.Count())
            {
                ModelState.AddModelError("Quantity Level", "The number of answers does not match the number of Quantity Level questions.");
            }

            // fill the openid user if they are openid validated
            if (HttpContext.Request.IsOpenId())
            {
                // doesn't matter if it's null, just assign what we have
                transaction.OpenIDUser = _openIdUserRepository.GetNullableById(CurrentUser.Identity.Name);
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
            decimal discount = 0.0m;                // used to calculate total discount
            decimal couponAmount = 0.0m;            // used to display individual discount of one coupon
            // get the email
            if (coup != null)
            {
                // calculate the coupon discount
                var emailQ = allQuestions.Where(a => a.Name == StaticValues.Question_Email && a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();
                if (emailQ != null)
                {
                    // get the answer
                    var answer = transactionAnswers.Where(a => a.QuestionId == emailQ.Id).FirstOrDefault();
                    discount = coup.UseCoupon(answer != null ? answer.Answer : null, quantity);
                }
                else
                {
                    discount = coup.UseCoupon(null, quantity);
                }
                
                // if coupon is used set display value
                if(discount == 0)
                {
                    ModelState.AddModelError("Coupon", NotificationMessages.STR_Coupon_could_not_be_used);
                    transaction.Coupon = null;
                }
                else
                {
                    couponAmount = coup.DiscountAmount;
                    // record the coupon usage to this transaction
                    transaction.Coupon = coup;
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
                    //var answer = question.QuestionType.Name != QuestionTypeText.STR_CheckboxList
                    //                 ? qa.Answer
                    //                 : (qa.CblAnswer != null ? string.Join(", ", qa.CblAnswer) : string.Empty);
                    var answer = CleanUpAnswer(question.QuestionType.Name, qa, question.ValidationClasses);

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
                        //var answer = question.QuestionType.Name != QuestionTypeText.STR_CheckboxList
                        //                 ? qa.Answer
                        //                 : (qa.CblAnswer != null ? string.Join(", ", qa.CblAnswer) : string.Empty);

                        var answer = CleanUpAnswer(question.QuestionType.Name, qa, question.ValidationClasses);
                        
                        var fieldName = string.Format("The answer for question \"{0}\" for {1} {2}", question.Name, item.QuantityName, (i + 1));

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

            if (!Access.HasItemAccess(CurrentUser, item)) //Allow editors to over ride and register for things
            {
                // do a final check to make sure the inventory is there
                if (item.Sold + quantity > item.Quantity)
                {
                    ModelState.AddModelError("Quantity", "There is not enough inventory to complete your order.");
                }
            }
            //if (transaction.Total == 0 && transaction.Credit)
            //{
            //    ModelState.AddModelError("Payment Method", "Please select check payment type when amount is zero.");
            //}
            if(transaction.Total == 0)
            {
                transaction.Credit = false;
                transaction.Check = true;
            }
            if (transaction.Total != displayAmount)
            {
                ModelState.AddModelError("Total", "We are sorry, the total amount displayed on the form did not match the total we calculated.");
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                // create the new transaction
                Repository.OfType<Transaction>().EnsurePersistent(transaction);

                if(transaction.Paid && transaction.Check)
                {
                    if(transaction.Item.CostPerItem == 0.0m || couponAmount > 0.0m)
                    {
                        //Ok, it is paid because the amount is zero, and it is because a coupon was used or the cost was zero
                        try
                        {
                            //If the tranascation is not evicted, it doesn't refresh from the database and the transaction number is null.
                            var saveId = transaction.Id;
                            NHibernateSessionManager.Instance.GetSession().Evict(transaction);
                            transaction = Repository.OfType<Transaction>().GetNullableById(saveId);
                            // attempt to get the contact information question set and retrieve email address
                            var question = transaction.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_Email).FirstOrDefault();
                            if (question != null)
                            {
                                // send an email to the user
                                _notificationProvider.SendConfirmation(Repository, transaction, question.Answer);
                            }
                        }
                        catch (Exception)
                        {
                            

                        }
                    }
                }

                var updatedItem = Repository.OfType<Item>().GetNullableById(transaction.Item.Id);
                if (updatedItem != null)
                {
                    //For whatever reason, if you are logged in with your CAES user, the item is updated, 
                    //if you are logged in with open id (google), item is not updated.
                    var transactionQuantity = transaction.Quantity;
                    if (updatedItem.Transactions.Contains(transaction))
                    {
                        transactionQuantity = 0;
                    }
                    if (updatedItem.Quantity - (updatedItem.Sold + transactionQuantity) <= 10)
                    {
                        _notificationProvider.SendLowQuantityWarning(Repository, updatedItem, transactionQuantity);
                    }


                }            
                // redirect to confirmation and let the user decide payment or not
                return this.RedirectToAction(a => a.Confirmation(transaction.Id));
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name, referenceIdHidden, null, null);
            viewModel.Quantity = quantity;
            viewModel.Answers = PopulateItemTransactionAnswer(transactionAnswers, quantityAnswers);
            viewModel.CreditPayment = (paymentType == StaticValues.CreditCard);
            viewModel.CheckPayment = (paymentType == StaticValues.Check);
            viewModel.TotalAmountToRedisplay = transaction.Total;
            viewModel.CouponAmountToDisplay = couponAmount;
            viewModel.CouponTotalDiscountToDisplay = discount;
            return View(viewModel);
        }


        /// <summary>
        /// Cleans up answer.
        /// Null bools get changed to false
        /// Null radio get set to an empty string (because it may not have the required attribute)
        /// Null CheckboxList get set to an empty string
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="qa">The qa.</param>
        /// <param name="validationClasses"></param>
        /// <returns>The answer</returns>
        private static string CleanUpAnswer(string name, QuestionAnswerParameter qa, string validationClasses)
        {
            string answer;
            if (name != QuestionTypeText.STR_CheckboxList)
            {
                if (name == QuestionTypeText.STR_Boolean)
                {
                    //Convert unchecked bool of null to false
                    if (string.IsNullOrEmpty(qa.Answer) || qa.Answer.ToLower() == "false")
                    {
                        answer = "false";
                    }
                    else
                    {
                        answer = "true";
                    }
                }
                else if(name == QuestionTypeText.STR_TextArea)
                {
                    answer = qa.Answer;
                }                
                else
                {      
                    answer = qa.Answer ?? string.Empty;
                    if (validationClasses != null && validationClasses.Contains("email"))
                    {
                        answer = answer.ToLower();
                    }
                }
            }
            else
            {
                if (qa.CblAnswer != null)
                {
                    answer = string.Join(", ", qa.CblAnswer);
                }
                else
                {
                    answer = string.Empty;
                }
            }
            return answer ?? (string.Empty);  //Something seems to have changed in the view where an empty text area now has null instead of an empty string
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
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);            

            if (transaction == null) return this.RedirectToAction<HomeController>(a => a.Index());

            Check.Require(transaction.Item != null);
            if(transaction.Credit)
            {
                Check.Require(!string.IsNullOrEmpty(transaction.Item.TouchnetFID));
            }

            string postingString = ConfigurationManager.AppSettings["TouchNetPostingKey"];
            //string Fid = " FID=" + ConfigurationManager.AppSettings["TouchNetFid"]; 
            string Fid = " FID=" + transaction.Item.TouchnetFID; 
            var validationKey = CalculateValidationString(postingString, transaction.TransactionGuid.ToString() + Fid, transaction.Total.ToString());
            var viewModel = PaymentConfirmationViewModel.Create(Repository, transaction, validationKey, Request, Url, Fid);
            return View(viewModel);
        }
        /// <summary>
        /// Calculates the validation string.
        /// </summary>
        /// <param name="postingKey">The posting key.</param>
        /// <param name="extTransID">The TransactionId</param>
        /// <param name="amt">The AMT.</param>
        /// <returns></returns>
        public static string CalculateValidationString(string postingKey, string extTransID, string amt)
        {
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(postingKey + extTransID + amt));
            return Convert.ToBase64String(data);
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The payment was successfully set to touch net.
        /// </summary>
        /// <param name="UPAY_SITE_ID">The touch net U Pay Site id.</param>
        /// <param name="EXT_TRANS_ID">The transaction Id.</param>
        /// <returns></returns>
        public ActionResult PaymentSuccess(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            Message = NotificationMessages.STR_TouchNetSuccess;
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// The payment was canceled.
        /// </summary>
        /// <param name="UPAY_SITE_ID">The touch net U Pay Site id.</param>
        /// <param name="EXT_TRANS_ID">The transaction Id.</param>
        /// <returns></returns>
        public ActionResult PaymentCancel(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            Message = NotificationMessages.STR_TouchNetCanceled;
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// There was an error from touch net.
        /// </summary>
        /// <param name="UPAY_SITE_ID">The touch net U Pay Site id.</param>
        /// <param name="EXT_TRANS_ID">The transaction Id.</param>
        /// <returns></returns>
        public ActionResult PaymentError(string UPAY_SITE_ID, string EXT_TRANS_ID)
        {
            Message = NotificationMessages.STR_TouchNetError;
            return this.RedirectToAction<HomeController>(a => a.Index());
        }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Edit Get
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [AnyoneWithRole]
        public ActionResult Edit(int id, string sort, string page)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);
            if(transaction == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                if (transaction.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");                    
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item"); 
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
                //return this.RedirectToAction<ItemManagementController>(a => a.Details(transaction.Item.Id));
            }
            var viewModel = EditTransactionViewModel.Create(Repository, transaction);
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

            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", sort, page);
            viewModel.Page = pageAndSort["page"];
            viewModel.Sort = pageAndSort["sort"];

            return View(viewModel);
        }

        /// <summary>
        /// Edit Post
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="checkSort"></param>
        /// <param name="checkPage"></param>
        /// <returns></returns>
        [HttpPost]
        [AnyoneWithRole]
        public ActionResult Edit(Transaction transaction, string checkSort, string checkPage)
        {
            ModelState.Clear();
            var transactionToUpdate = Repository.OfType<Transaction>().GetNullableById(transaction.Id);
            if (transactionToUpdate == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transactionToUpdate.Item == null || !Access.HasItemAccess(CurrentUser, transactionToUpdate.Item))
            {
                if (transactionToUpdate.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");                    
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item"); 
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", checkSort, checkPage);

            var correctionTransaction = new Transaction(transactionToUpdate.Item);
            correctionTransaction.Amount = transaction.Amount;
            correctionTransaction.CorrectionReason = transaction.CorrectionReason;
            if (correctionTransaction.Amount > 0)
            {
                correctionTransaction.Donation = true;
            }
            else
            {
                correctionTransaction.Donation = false; 
            }
            correctionTransaction.CreatedBy = CurrentUser.Identity.Name;

            transactionToUpdate.AddChildTransaction(correctionTransaction);
            //There is a similar check in the payment controller, but with a different message
            if (transactionToUpdate.TotalPaid > transactionToUpdate.Total)
            {
                ModelState.AddModelError("Corrections", "The total of all correction amounts must not exceed the amount already paid.");
            }
            correctionTransaction.TransferValidationMessagesTo(ModelState);//Validate Child as well as parent(next Line)
            transactionToUpdate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transactionToUpdate);
                //return this.RedirectToAction<ItemManagementController>(a => a.Details(transactionToUpdate.Item.Id));
                return
                    Redirect(Url.DetailItemUrl
                    (
                        transactionToUpdate.Item.Id,
                        StaticValues.Tab_Checks,
                        pageAndSort["sort"],
                        pageAndSort["page"])
                    );
            }

            //TODO: We could replace the line below with a rollback to be more consistent.
            transactionToUpdate.ChildTransactions.Remove(correctionTransaction);

            var viewModel = EditTransactionViewModel.Create(Repository, transactionToUpdate);
            viewModel.TransactionValue = transactionToUpdate;
            viewModel.ContactName =
                transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;
            viewModel.ContactName = viewModel.ContactName + " " + transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
            viewModel.ContactEmail = transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email).FirstOrDefault().Answer;

            viewModel.Sort = pageAndSort["sort"];
            viewModel.Page = pageAndSort["page"];
            
            return View(viewModel);
        }

        /// <summary>
        /// Refunds the specified id.
        /// Get ..\Transaction\Refund
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [AnyoneWithRole]
        public ActionResult Refund(int id, string sort, string page)
        {
            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", sort, page);
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);
            if (transaction == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                if (transaction.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item");
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
                //return this.RedirectToAction<ItemManagementController>(a => a.Details(transaction.Item.Id));
            }
            if(transaction.ChildTransactions.Where(a => a.Refunded && a.IsActive).Any())
            {
                Message = @"Active Refund already exists.";
                return Redirect(Url.DetailItemUrl
                    (
                        transaction.Item.Id,
                        StaticValues.Tab_Refunds,
                        pageAndSort["sort"],
                        pageAndSort["page"])
                    );
            }
            var viewModel = EditTransactionViewModel.Create(Repository, transaction);
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

            viewModel.Page = pageAndSort["page"];
            viewModel.Sort = pageAndSort["sort"];

            return View(viewModel);
        }

        /// <summary>
        /// Refunds the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="refundSort">The refund sort.</param>
        /// <param name="refundPage">The refund page.</param>
        /// <returns></returns>
        [AnyoneWithRole]
        [HttpPost]
        public ActionResult Refund(Transaction transaction, string refundSort, string refundPage)
        {
            ModelState.Clear();
            var transactionToUpdate = Repository.OfType<Transaction>().GetNullableById(transaction.Id);
            if (transactionToUpdate == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transactionToUpdate.Item == null || !Access.HasItemAccess(CurrentUser, transactionToUpdate.Item))
            {
                if (transactionToUpdate.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item");
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", refundSort, refundPage);

            var refundTransaction = new Transaction(transactionToUpdate.Item);
            refundTransaction.Amount = transaction.Amount;
            refundTransaction.CorrectionReason = transaction.CorrectionReason;
            refundTransaction.Donation = false;
            refundTransaction.Refunded = true;

            refundTransaction.CreatedBy = CurrentUser.Identity.Name;

            transactionToUpdate.AddChildTransaction(refundTransaction);
            if(refundTransaction.Amount <= 0)
            {
                ModelState.AddModelError("Amount", @"Refund Amount must be greater than zero.");
            }
            //There is a similar check in the payment controller, but with a different message
            if (transactionToUpdate.TotalPaid < 0)
            {
                ModelState.AddModelError("TotalPaid", @"The refund amount must not exceed the amount already paid.");
            }
            refundTransaction.TransferValidationMessagesTo(ModelState);//Validate Child as well as parent(next Line)
            transactionToUpdate.TransferValidationMessagesTo(ModelState);
            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transactionToUpdate);
                if(transactionToUpdate.Credit)
                {
                    var user = Repository.OfType<User>().Queryable.First(a => a.LoginID == CurrentUser.Identity.Name);
                    _notificationProvider.SendRefundNotification(user, refundTransaction, false);
                }
                return Redirect(Url.DetailItemUrl
                    (
                        transactionToUpdate.Item.Id,
                        StaticValues.Tab_Refunds,
                        pageAndSort["sort"],
                        pageAndSort["page"])
                    );
            }

            //TODO: We could replace the line below with a rollback to be more consistent.
            transactionToUpdate.ChildTransactions.Remove(refundTransaction);

            var viewModel = EditTransactionViewModel.Create(Repository, transactionToUpdate);
            viewModel.TransactionValue = transactionToUpdate;
            viewModel.ContactName =
                transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;
            viewModel.ContactName = viewModel.ContactName + " " + transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
            viewModel.ContactEmail = transactionToUpdate.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email).FirstOrDefault().Answer;

            viewModel.Sort = pageAndSort["sort"];
            viewModel.Page = pageAndSort["page"];
            viewModel.CorrectionReason = transaction.CorrectionReason;
            viewModel.RefundAmount = transaction.Amount;

            return View(viewModel);

        }

        [AnyoneWithRole]
        [HttpPost]
        public ActionResult RemoveRefund(int id, string sort, string page)
        {
            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", sort, page);
            var transactionToUpdate = Repository.OfType<Transaction>().GetNullableById(id);
            if (transactionToUpdate == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transactionToUpdate.Item == null || !Access.HasItemAccess(CurrentUser, transactionToUpdate.Item))
            {
                if (transactionToUpdate.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item");
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var childTransaction = transactionToUpdate.ChildTransactions.Where(a => a.Refunded && a.IsActive).FirstOrDefault();
            if(childTransaction == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Refund");
                return Redirect(Url.DetailItemUrl
                    (
                        transactionToUpdate.Item.Id,
                        StaticValues.Tab_Refunds,
                        pageAndSort["sort"],
                        pageAndSort["page"])
                    );
            }

            childTransaction.IsActive = false;
            Repository.OfType<Transaction>().EnsurePersistent(transactionToUpdate);
            if(transactionToUpdate.Credit)
            {
                var user = Repository.OfType<User>().Queryable.First(a => a.LoginID == CurrentUser.Identity.Name);
                _notificationProvider.SendRefundNotification(user, childTransaction, true);
            }
            return Redirect(Url.DetailItemUrl
                (
                    transactionToUpdate.Item.Id,
                    StaticValues.Tab_Refunds,
                    pageAndSort["sort"],
                    pageAndSort["page"])
                );
        }

        public ActionResult SendNotification(int id, string sort, string page)
        {
            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", sort, page);
            var transactionToUpdate = Repository.OfType<Transaction>().GetNullableById(id);
            if (transactionToUpdate == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transactionToUpdate.Item == null || !Access.HasItemAccess(CurrentUser, transactionToUpdate.Item))
            {
                if (transactionToUpdate.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item");
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            // attempt to get the contact information question set and retrieve email address
            var question = transactionToUpdate.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_Email).FirstOrDefault();
            if (question != null)
            {
                // send an email to the user
                _notificationProvider.SendConfirmation(Repository, transactionToUpdate, question.Answer);
            }

            return Redirect(Url.DetailItemUrl
            (
                transactionToUpdate.Item.Id,
                StaticValues.Tab_Notifications,
                pageAndSort["sort"],
                pageAndSort["page"])
            );
        }

        /// <summary>
        /// Detailses the refund.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult DetailsRefund(int id, string sort, string page)
        {
            var pageAndSort = ValidateParameters.PageAndSort("ItemDetails", sort, page);
            var transactionToView = Repository.OfType<Transaction>().GetNullableById(id);
            if (transactionToView == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transactionToView.Item == null || !Access.HasItemAccess(CurrentUser, transactionToView.Item))
            {
                if (transactionToView.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_NoEditorRights.Replace(NotificationMessages.ObjectType, "Item");
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var childTransaction = transactionToView.ChildTransactions.Where(a => a.Refunded && a.IsActive).FirstOrDefault();
            if(childTransaction == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Refund");
                return Redirect(Url.DetailItemUrl
                    (
                        transactionToView.Item.Id,
                        StaticValues.Tab_Refunds,
                        pageAndSort["sort"],
                        pageAndSort["page"])
                    );
            }

            var viewModel = EditTransactionViewModel.Create(Repository, transactionToView);
            viewModel.TransactionValue = transactionToView;
            viewModel.ContactName =
                transactionToView.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;
            viewModel.ContactName = viewModel.ContactName + " " + transactionToView.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
            viewModel.ContactEmail = transactionToView.TransactionAnswers.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email).FirstOrDefault().Answer;

            viewModel.Sort = pageAndSort["sort"];
            viewModel.Page = pageAndSort["page"];
            viewModel.CorrectionReason = childTransaction.CorrectionReason;
            viewModel.CreateDate = childTransaction.TransactionDate;
            viewModel.CreatedBy = childTransaction.CreatedBy;
            viewModel.RefundAmount = childTransaction.Amount;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /Transaction/PaymentResult/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Deals with the return result from the payment gateway.
        /// </remarks>
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult PaymentResult(PaymentResultParameters touchNetValues)
        {
            #region Actual Work
            // validate to make sure a transaction value was received
            if (!string.IsNullOrEmpty(touchNetValues.EXT_TRANS_ID))
            {
                string parsedTransaction = touchNetValues.EXT_TRANS_ID.Substring(0,
                                                                                 touchNetValues.EXT_TRANS_ID.LastIndexOf
                                                                                     (" FID="));
                var transaction = Repository.OfType<Transaction>()
                    .Queryable.Where(a => a.TransactionGuid == new Guid(parsedTransaction))
                    .SingleOrDefault();
                //var transaction = Repository.OfType<Transaction>().GetNullableById(touchNetValues.EXT_TRANS_ID.Value);

                if(transaction == null)
                {
                    #region Email Error Results
                    _notificationProvider.SendPaymentResultErrors(ConfigurationManager.AppSettings["EmailForErrors"], touchNetValues, Request.Params, null, PaymentResultType.TransactionNotFound);
                    #endregion Email Error Results

                    return View();
                }

                // create a payment log
                var paymentLog = new PaymentLog(transaction.Total);
                paymentLog.Credit = true;

                // on success, save the valid information
                if (touchNetValues.PMT_STATUS.ToLower() == "success")
                {
                    paymentLog.Name = touchNetValues.NAME_ON_ACCT;
                    paymentLog.Amount = touchNetValues.PMT_AMT.Value;
                    paymentLog.Accepted = true;
                    paymentLog.GatewayTransactionId = touchNetValues.TPG_TRANS_ID;
                    paymentLog.CardType = touchNetValues.CARD_TYPE;
                    if (!transaction.IsActive)
                    {
                        //Possibly we could email someone here to say it has been re-activated
                        transaction.IsActive = true;
                    }
                }

                paymentLog.TnBillingAddress1 = touchNetValues.acct_addr;
                paymentLog.TnBillingAddress2 = touchNetValues.acct_addr2;
                paymentLog.TnBillingCity = touchNetValues.acct_city;
                paymentLog.TnBillingState = touchNetValues.acct_state;
                paymentLog.TnBillingZip = touchNetValues.acct_zip;
                paymentLog.TnCancelLink = touchNetValues.CANCEL_LINK;
                paymentLog.TnErrorLink = touchNetValues.ERROR_LINK;
                paymentLog.TnPaymentDate = touchNetValues.pmt_date;
                paymentLog.TnSubmit = touchNetValues.Submit;
                paymentLog.TnSuccessLink = touchNetValues.SUCCESS_LINK;
                paymentLog.TnSysTrackingId = touchNetValues.sys_tracking_id;
                paymentLog.TnUpaySiteId = touchNetValues.UPAY_SITE_ID;
                switch (touchNetValues.PMT_STATUS.ToLower())
                {
                    case "success":
                        paymentLog.TnStatus = "S";
                        break;
                    case "cancelled":
                    case "canceled":
                        paymentLog.TnStatus = "C";
                        break;
                    default:
                        paymentLog.TnStatus = "E";
                        break;
                }

                if(touchNetValues.posting_key != ConfigurationManager.AppSettings["TouchNetPostingKey"])
                {
                    ModelState.AddModelError("PostingKey", "Posting Key Error");
                    paymentLog.Accepted = false;
                }
                if (touchNetValues.UPAY_SITE_ID != ConfigurationManager.AppSettings["TouchNetSiteId"])
                {
                    ModelState.AddModelError("SiteId", "TouchNet Site Id Error");
                    paymentLog.Accepted = false;
                }
                if (touchNetValues.TPG_TRANS_ID == "DUMMY_TRANS_ID")
                {
                    ModelState.AddModelError("TPG_TRANS_ID", "TouchNet TPG_TRANS_ID Error");
                    paymentLog.Accepted = false;
                }
                if (touchNetValues.PMT_AMT != transaction.Total)
                {
                    paymentLog.Accepted = false;
                    if (touchNetValues.PMT_AMT != 0 && paymentLog.TnStatus == "S")
                    {
                        ModelState.AddModelError("Amount", "TouchNet Amount does not match local amount");
                    }
                }

                transaction.AddPaymentLog(paymentLog);
                //paymentLog.Transaction = transaction;

                paymentLog.TransferValidationMessagesTo(ModelState);

                if (ModelState.IsValid)
                {
                    Repository.OfType<Transaction>().EnsurePersistent(transaction);
                    if (paymentLog.Accepted)
                    {
                        // attempt to get the contact information question set and retrieve email address
                        var question =
                            transaction.TransactionAnswers.Where(
                                a =>
                                a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                                a.Question.Name == StaticValues.Question_Email).FirstOrDefault();
                        if (question != null)
                        {
                            // send an email to the user
                            _notificationProvider.SendConfirmation(Repository, transaction, question.Answer);
                        }
                        if (transaction.TotalPaid > transaction.Total)
                        {
                            _notificationProvider.SendPaymentResultErrors(
                                ConfigurationManager.AppSettings["EmailForErrors"], touchNetValues, Request.Params, null,
                                PaymentResultType.OverPaid);
                        }
                    }
                }
                else
                {
                    #region InValid PaymentLog -- Email Results                   
                    var body = new StringBuilder();
                    try
                    {
                        body.Append("<br/><br/>Payment log values:<br/>");
                        body.Append("Name:" + paymentLog.Name + "<br/>");
                        body.Append("Amount:" + paymentLog.Amount + "<br/>");
                        body.Append("Accepted:" + paymentLog.Accepted + "<br/>");
                        body.Append("Gateway transaction id:" + paymentLog.GatewayTransactionId + "<br/>");
                        body.Append("Card Type: " + paymentLog.CardType + "<br/>");
                        body.Append("ModelState: " + ModelState.IsValid);

                        body.Append("<br/><br/>===== modelstate errors text===<br/>");
                        foreach (var result in ModelState.Values)
                        {
                            foreach (var errs in result.Errors)
                            {
                                body.Append("Error:" + errs.ErrorMessage + "<br/>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        body.Append(ex.Message);
                    }
                    _notificationProvider.SendPaymentResultErrors(ConfigurationManager.AppSettings["EmailForErrors"], touchNetValues, Request.Params, body.ToString(), PaymentResultType.InValidPaymentLog);
                    #endregion InValid PaymentLog -- Email Results
                }
            }
            #endregion

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
            // check for when answer is null, because when doing a radio button it is null when nothing is selected
            if (regExVal.IsMatch(answer ?? string.Empty)) return true;

            // not valid input provide error message
            message = string.Format(validator.ErrorMessage, fieldName);
            return false;
        }

        /// <summary>
        /// This one is used for Agribusiness to pass these values.
        /// </summary>
        /// <param name="agribusinessExtraParams"></param>
        /// <param name="questionSets"></param>
        /// <returns></returns>
        private IEnumerable<ItemTransactionAnswer> PopulateItemTransactionAnswer(AgribusinessExtraParams agribusinessExtraParams, ICollection<ItemQuestionSet> questionSets)
        {
            var answers = new List<ItemTransactionAnswer>();

            // if anything is null, just return no answers
            if(agribusinessExtraParams == null || questionSets == null)
                return answers;

            // find the contact information question set
            var questionSet = questionSets.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation).Select(a => a.QuestionSet).FirstOrDefault();

            // if it exists, fill in the questions
            if(questionSet != null)
            {
                var questionAnswer = new Dictionary<string, string>();
                questionAnswer.Add(StaticValues.Question_FirstName, agribusinessExtraParams.FN);
                questionAnswer.Add(StaticValues.Question_LastName, agribusinessExtraParams.LN);
                questionAnswer.Add(StaticValues.Question_Title, agribusinessExtraParams.Title);
                questionAnswer.Add(StaticValues.Question_StreetAddress, agribusinessExtraParams.Address);
                questionAnswer.Add(StaticValues.Question_AddressLine2, agribusinessExtraParams.Address2);
                questionAnswer.Add(StaticValues.Question_City, agribusinessExtraParams.City);
                questionAnswer.Add(StaticValues.Question_State, agribusinessExtraParams.State != null ? agribusinessExtraParams.State.Trim().ToUpper() : string.Empty);
                questionAnswer.Add(StaticValues.Question_Zip, agribusinessExtraParams.Zip);
                questionAnswer.Add(StaticValues.Question_PhoneNumber, agribusinessExtraParams.Phone != null ? agribusinessExtraParams.Phone.Replace('.', '-') : string.Empty);
                questionAnswer.Add(StaticValues.Question_Email, agribusinessExtraParams.Email);
                foreach(var question in questionSet.Questions)
                {
                    //If it doesn't find the question, it will throw an exception. (a good thing.)
                    var ans = questionAnswer[question.Name];
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
                var questionAnswer = new Dictionary<string, string>();
                questionAnswer.Add(StaticValues.Question_FirstName, openIdUser.FirstName);
                questionAnswer.Add(StaticValues.Question_LastName, openIdUser.LastName);
                questionAnswer.Add(StaticValues.Question_StreetAddress, openIdUser.StreetAddress);
                questionAnswer.Add(StaticValues.Question_AddressLine2, openIdUser.Address2);
                questionAnswer.Add(StaticValues.Question_City, openIdUser.City);
                questionAnswer.Add(StaticValues.Question_State, openIdUser.State);
                questionAnswer.Add(StaticValues.Question_Zip, openIdUser.Zip);
                questionAnswer.Add(StaticValues.Question_PhoneNumber, openIdUser.PhoneNumber);
                questionAnswer.Add(StaticValues.Question_Email, openIdUser.Email);   
                foreach (var question in questionSet.Questions)
                {
                    //If it doesn't find the question, it will throw an exception. (a good thing.)
                    var ans = questionAnswer[question.Name];
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

                #region old way answers were assigned
                //foreach(var question in questionSet.Questions)
                //{
                //    var ans = string.Empty;
          
                //    if (question.Name == StaticValues.Question_FirstName)
                //    {
                //        ans = openIdUser.FirstName;
                //    }
                //    else if (question.Name == StaticValues.Question_LastName)
                //    {
                //        ans = openIdUser.LastName;
                //    }
                //    else if (question.Name == StaticValues.Question_StreetAddress)
                //    {
                //        ans = openIdUser.StreetAddress;
                //    }
                //    else if (question.Name == StaticValues.Question_AddressLine2)
                //    {
                //        ans = openIdUser.Address2;
                //    }
                //    else if (question.Name == StaticValues.Question_City)
                //    {
                //        ans = openIdUser.City;
                //    }
                //    else if (question.Name == StaticValues.Question_State)
                //    {
                //        ans = openIdUser.State;
                //    }
                //    else if (question.Name == StaticValues.Question_Zip)
                //    {
                //        ans = openIdUser.Zip;
                //    }
                //    else if (question.Name == StaticValues.Question_PhoneNumber)
                //    {
                //        ans = openIdUser.PhoneNumber;
                //    }
                //    else if (question.Name == StaticValues.Question_Email)
                //    {
                //        ans = openIdUser.Email;
                //    }

                //    // create the answer object
                //    var answer = new ItemTransactionAnswer()
                //    {
                //        Answer = ans,
                //        QuestionId = question.Id,
                //        QuestionSetId = question.QuestionSet.Id,
                //        Transaction = true
                //    };

                //    answers.Add(answer);
                //}
                #endregion old way answers were assigned
            }

            return answers;
        }
        private IEnumerable<ItemTransactionAnswer> PopulateItemTransactionAnswer(QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers)
        {
            var answers = new List<ItemTransactionAnswer>();

            foreach (var qap in transactionAnswers)
            {
                var question = Repository.OfType<Question>().GetNullableById(qap.QuestionId);
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
                var question = Repository.OfType<Question>().GetNullableById(qap.QuestionId);
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

        [HttpPost]
        public ActionResult Lookup(string orderNumber, string email)
        {
            var transaction = Repository.OfType<Transaction>().Queryable.Where(a => a.TransactionNumber == orderNumber).FirstOrDefault();
            if(email != null)
            {
                email = email.ToLower();
            }
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
                    if (transaction.Credit && transaction.Check == false)
                    {
                        if (!transaction.Paid && transaction.IsActive)
                        {
                            if (transaction.PaymentLogs.Where(a => a.TnStatus == "C" || a.TnStatus == "E").Any())
                            {
                                if(!transaction.RefundIssued ) //Just in case
                                {
                                    viewModel.ShowCreditCardReSubmit = true;
                                }
                            }
                        }
                    }
                    Message = string.Empty;
                }
                else
                {
                    viewModel.TransactionNumber = orderNumber;
                    viewModel.Email = email;

                    Message = "Unable to locate order, please check your information and try again.";  
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

        [AdminOnly]
        public ActionResult AdminLookup(string email)
        {
            if(email == null)
            {
                email = string.Empty;
            }
            else
            {
                email = email.Trim().ToLower();
            }
            var transactionAnswers =
                Repository.OfType<TransactionAnswer>().Queryable.Where(
                    a =>
                    a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                    a.Question.Name == StaticValues.Question_Email && a.Answer == email).ToList();

            var transactions = transactionAnswers.Select(transactionAnswer => transactionAnswer.Transaction);

            return View(transactions);
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

    public class AgribusinessExtraParams
    {
        /// <summary>
        /// FirstName
        /// </summary>
        public string FN { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public string LN { get; set; }

        public string Title { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        /// <summary>
        /// State (2 Character)
        /// </summary>
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
