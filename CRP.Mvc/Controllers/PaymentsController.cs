using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.ViewModels;
using CRP.Mvc.Controllers.ViewModels.Payment;
using CRP.Mvc.Resources;
using CRP.Mvc.Services;
using Microsoft.Azure;
using MvcContrib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class PaymentsController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;
        private readonly IDataSigningService _dataSigningService;
        private readonly INotificationProvider _notificationProvider;

        public PaymentsController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository, IDataSigningService dataSigningService,  INotificationProvider notificationProvider)
        {
            _openIdUserRepository = openIdUserRepository;
            _dataSigningService = dataSigningService;
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
            if (!viewModel.Answers.Any())
            {
                viewModel.Answers = PopulateItemTransactionAnswer(agribusinessExtraParams, item.QuestionSets);
            }
            viewModel.TotalAmountToRedisplay = viewModel.Quantity * item.CostPerItem;
            viewModel.CouponAmountToDisplay = 0.0m; // They have not entered a coupon yet
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
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Checkout(int id, string referenceIdHidden, int quantity, decimal? donation, decimal? displayAmount, string paymentType, string restrictedKey, string coupon, QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers)
        {
            bool captchaValid = false;
            var response = Request.Form["g-Recaptcha-Response"];
            using (var client = new HttpClient())
            {

                var googleResponse = await client.PostAsync(String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", CloudConfigurationManager.GetSetting("NewRecaptchaPrivateKey"), response), null);
                googleResponse.EnsureSuccessStatusCode();
                var responseContent = JsonConvert.DeserializeObject(await googleResponse.Content.ReadAsStringAsync());
                dynamic data = JObject.FromObject(responseContent);
                captchaValid = data.success;
            }


            // if the arrays are null create new blank ones
            if (transactionAnswers == null) transactionAnswers = new QuestionAnswerParameter[0];
            if (quantityAnswers == null) quantityAnswers = new QuestionAnswerParameter[0];


            #region DB Queries
            // get the item
            var item = Repository.OfType<Item>().GetNullableById(id);



            // get all the questions in 1 queries
            var questionIds = transactionAnswers.Select(b => b.QuestionId).ToList().Union(quantityAnswers.Select(c => c.QuestionId).ToList()).ToArray();
            var allQuestions = Repository.OfType<Question>().Queryable.Where(a => questionIds.Contains(a.Id)).ToList();

            if (!string.IsNullOrWhiteSpace(referenceIdHidden))
            {
                var refId = allQuestions.FirstOrDefault(a => a.Name == "Reference Id");
                if (refId != null)
                {
                    if (transactionAnswers.Any(a => a.QuestionId == refId.Id && string.IsNullOrWhiteSpace(a.Answer)))
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
                ModelState.AddModelError("Captcha", "Captcha missing or invalid. Are you a robot?");
            }

            if (quantity < 1)
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
            var amount = item.CostPerItem * quantity; // get the initial amount
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
                if (discount == 0)
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
            if (transaction.Total == 0)
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

                if (transaction.Paid && transaction.Check)
                {
                    if (transaction.Item.CostPerItem == 0.0m || couponAmount > 0.0m)
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
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);

                        }
                    }
                }

                var updatedItem = Repository.OfType<Item>().GetNullableById(transaction.Item.Id);
                if (updatedItem != null)
                {
                    try
                    {
                        updatedItem.SoldCount = Repository.OfType<Transaction>().Queryable.Where(a => a.Item.Id == updatedItem.Id && a.IsActive).Sum(a => a.Quantity);
                        Repository.OfType<Item>().EnsurePersistent(updatedItem);

                        if ((updatedItem.Quantity - updatedItem.SoldCount) <= 10)
                        {
                            try
                            {
                                var soldAndPaid = updatedItem.SoldAndPaidQuantity;
                                _notificationProvider.SendLowQuantityWarning(Repository, updatedItem, soldAndPaid);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error trying to send email notification {0}", ex.Message);
                            }
                        }

                        if (updatedItem.NotifyEditors)
                        {
                            try
                            {
                                _notificationProvider.SendPurchaseToOwners(Repository, updatedItem, transaction.Quantity);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error trying to send email notification2 {0}", ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Error Updating SoldCount from user checkout {0}", ex.Message));
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

        public ActionResult Confirmation(int id)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);

            if (transaction == null) return this.RedirectToAction<HomeController>(a => a.Index());

            Check.Require(transaction.Item != null);
            if (transaction.Credit)
            {
                Check.Require(transaction.Item.FinancialAccount != null);
            }

            var model = new PaymentConfirmationViewModel();
            model.Transaction = transaction;
            model.PostUrl = CloudConfigurationManager.GetSetting("CyberSource.BaseUrl");

            // prepare dictionary and sign it
            var dictionary = transaction.GetPaymentDictionary();
            dictionary.Add("access_key", CloudConfigurationManager.GetSetting("CyberSource.AccessKey"));
            dictionary.Add("profile_id", CloudConfigurationManager.GetSetting("CyberSource.ProfileId"));

            var fieldNames = string.Join(",", dictionary.Keys);
            dictionary.Add("signed_field_names", "signed_field_names," + fieldNames);

            model.Signature = _dataSigningService.Sign(dictionary);
            model.PaymentDictionary = dictionary;

            return View(model);
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public async Task<ActionResult> Receipt(ReceiptResponseModel response)
        {
#if DEBUG
            // For testing local only, we should process the actual payment
            // Live systems will use the side channel message from cybersource direct to record the payment event
            // This will duplicate some log messages. That is OKAY
            await ProviderNotify(response);
#endif

            Log.ForContext("response", response, true).Information("Receipt response received");

            // check signature
            var dictionary = Request.Form.AllKeys.ToDictionary(x => x, x => Request.Form[x]);
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
                Message = "An error has occurred. Payment not processed. If you experience further problems, contact us.";
                return new HttpStatusCodeResult(500);
            }

            // find matching transaction
            if (!int.TryParse(response.Req_Reference_Number, out int transactionId))
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                Message = "Transaction for payment not found. Please contact technical support.";
                return new HttpNotFoundResult();
            }

            var transaction = Repository.OfType<Transaction>()
                .Queryable
                .SingleOrDefault(a => a.Id == transactionId);

            if (transaction == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                Message = "Transaction for payment not found. Please contact technical support.";
                return new HttpNotFoundResult();
            }

            var responseValid = CheckResponse(response);
            if (!responseValid.IsValid)
            {
                // send them back to the pay page with errors
                Message = $"Errors detected: {string.Join(",", responseValid.Errors)}";
                return RedirectToAction(nameof(Confirmation), new {id = transactionId});
            }

            // Should be good 
            Message = "Payment Processed. Thank You.";

            // Fake payment status
            var model = new PaymentReceiptViewModel
            {
                Item         = transaction.Item,
                Transaction  = transaction,
                AuthCode     = response.Auth_Code,
                AuthDateTime = response.AuthorizationDateTime,
                CardNumber   = response.Req_Card_Number,
                CardExp      = response.CardExpiration,
                Amount       = response.Auth_Amount
            };

            return View(model);
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public async Task<ActionResult> Cancel(ReceiptResponseModel response)
        {
#if DEBUG
            // For testing local only, we should process the actual payment
            // Live systems will use the side channel message from cybersource direct to record the payment event
            // This will duplicate some log messages. That is OKAY
            await ProviderNotify(response);
#endif

            Log.ForContext("response", response, true).Information("Receipt response received");

            // check signature
            var dictionary = Request.Form.AllKeys.ToDictionary(x => x, x => Request.Form[x]);
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
                Message = "An error has occurred. Payment not processed. If you experience further problems, contact us.";
                return new HttpStatusCodeResult(500);
            }

            // find matching transaction
            var transactionId = new Guid(response.Req_Reference_Number);
            var transaction = Repository.OfType<Transaction>()
                .Queryable
                .SingleOrDefault(a => a.TransactionGuid == transactionId);

            if (transaction == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                Message = "Transaction for payment not found. Please contact technical support.";
                return new HttpNotFoundResult();
            }

            Message = "Payment Process Cancelled";
            return RedirectToAction(nameof(Confirmation), new {id = transactionId});
        }

        [HttpPost]
        [AllowAnonymous]
        [BypassAntiForgeryToken]
        public async Task<ActionResult> ProviderNotify(ReceiptResponseModel response)
        {
            Log.ForContext("response", response, true).Information("Provider Notification Received");

            // check signature
            var dictionary = Request.Form.AllKeys.ToDictionary(x => x, x => Request.Form[x]);
            if (!_dataSigningService.Check(dictionary, response.Signature))
            {
                Log.Error("Check Signature Failure");
                return new JsonNetResult(new { });
            }

            // find matching transaction
            if (!int.TryParse(response.Req_Reference_Number, out int transactionId))
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                Message = "Transaction for payment not found. Please contact technical support.";
                return new HttpNotFoundResult();
            }

            var transaction = Repository.OfType<Transaction>()
                .Queryable
                .SingleOrDefault(a => a.Id == transactionId);

            if (transaction == null)
            {
                Log.Error("Order not found {0}", response.Req_Reference_Number);
                Message = "Transaction for payment not found. Please contact technical support.";
                return new JsonNetResult(new { });
            }

            // create a payment log and record it in the db
            var paymentLog = new PaymentLog(transaction.Total)
            {
                Credit               = true,
                Name                 = $"{response.Req_Bill_To_Forename} {response.Req_Bill_To_Surname}",
                GatewayTransactionId = response.Transaction_Id,
                CardType             = response.Req_Card_Type,
                TnBillingAddress1    = response.Req_Bill_To_Address_Line1,
                TnBillingAddress2    = response.Req_Bill_To_Address_Line2,
                TnBillingCity        = response.Req_Bill_To_Address_City,
                TnBillingState       = response.Req_Bill_To_Address_State,
                TnBillingZip         = response.Req_Bill_To_Address_Postal_Code,
                TnPaymentDate        = response.Auth_Time
            };

            if (decimal.TryParse(response.Auth_Amount, out decimal amount))
            {
                paymentLog.Amount = amount;
            }

            // associate transaction
            transaction.AddPaymentLog(paymentLog);

            if (response.Decision == CyberSourceReplyCodes.Accept)
            {
                paymentLog.Accepted = true;

                if (!transaction.IsActive)
                {
                    //Possibly we could email someone here to say it has been re-activated
                    transaction.IsActive = true;
                }

                // attempt to get the contact information question set and retrieve email address
                var question = transaction.TransactionAnswers
                    .FirstOrDefault(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation
                                      && a.Question.Name == StaticValues.Question_Email);
                if (question != null)
                {
                    // send an email to the user
                    _notificationProvider.SendConfirmation(Repository, transaction, question.Answer);
                }
            }
            else
            {
                
            }

            Repository.OfType<Transaction>().EnsurePersistent(transaction);
            return new JsonNetResult(new { });
        }

        private CheckResponseResults CheckResponse(ReceiptResponseModel response)
        {
            var contextLog = Log.ForContext("decision", response.Decision).ForContext("reason", response.Reason_Code);

            var rtValue = new CheckResponseResults();
            //Ok, check response
            // general error, bad request
            if (string.Equals(response.Decision, CyberSourceReplyCodes.Error) ||
                response.Reason_Code == CyberSourceReasonCodes.BadRequestError ||
                response.Reason_Code == CyberSourceReasonCodes.MerchantAccountError)
            {
                contextLog.Warning("Unsuccessful Reply");
                rtValue.Errors.Add("An error has occurred. If you experience further problems, please contact us");
            }

            // this is only possible on a hosted payment page
            if (string.Equals(response.Decision, CyberSourceReplyCodes.Cancel))
            {
                contextLog.Warning("Cancelled Reply");
                rtValue.Errors.Add("The payment process was canceled before it could complete. If you experience further problems, please contact us");
            }

            // manual review required
            if (string.Equals(response.Decision, CyberSourceReplyCodes.Review))
            {
                contextLog.Warning("Manual Review Reply");
                rtValue.Errors.Add("Error with Credit Card. Please contact issuing bank. If you experience further problems, please contact us");
            }

            // bad cc information, return to payment page
            if (string.Equals(response.Decision, CyberSourceReplyCodes.Decline))
            {
                if (response.Reason_Code == CyberSourceReasonCodes.AvsFailure)
                {
                    contextLog.Warning("Avs Failure");
                    rtValue.Errors.Add("We’re sorry, but it appears that the billing address that you entered does not match the billing address registered with your card. Please verify that the billing address and zip code you entered are the ones registered with your card issuer and try again. If you experience further problems, please contact us");
                }

                if (response.Reason_Code == CyberSourceReasonCodes.BankTimeoutError ||
                    response.Reason_Code == CyberSourceReasonCodes.ProcessorTimeoutError)
                {
                    contextLog.Error("Bank Timeout Error");
                    rtValue.Errors.Add("Error contacting Credit Card issuing bank. Please wait a few minutes and try again. If you experience further problems, please contact us");
                }
                else
                {
                    contextLog.Warning("Declined Card Error");
                    rtValue.Errors.Add("We’re sorry but your credit card was declined. Please use an alternative credit card and try submitting again. If you experience further problems, please contact us");
                }
            }

            // good cc info, partial payment
            if (string.Equals(response.Decision, CyberSourceReplyCodes.Accept) &&
                response.Reason_Code == CyberSourceReasonCodes.PartialApproveError)
            {
                //I Don't think this can happen.
                //TODO: credit card was partially billed. flag transaction for review
                //TODO: send to general error page
                contextLog.Error("Partial Payment Error");
                rtValue.Errors.Add("We’re sorry but a Partial Payment Error was detected. Please contact us");
            }

            if (rtValue.Errors.Count <= 0)
            {
                if (response.Decision != CyberSourceReplyCodes.Accept)
                {
                    contextLog.Error("Got past all the other checks. But it still wasn't Accepted");
                    rtValue.Errors.Add("Unknown Error. Please contact us.");
                }
                else
                {
                    rtValue.IsValid = true;
                }
            }

            return rtValue;
        }

        private class CheckResponseResults
        {
            public bool IsValid { get; set; } = false;
            public IList<string> Errors { get; set; } = new List<string>();
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
                questionAnswer.Add(StaticValues.Question_Region, string.Empty);
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
                questionAnswer.Add(StaticValues.Question_Region, string.Empty);
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
