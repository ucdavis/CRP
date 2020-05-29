using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.ViewModels.Transaction;
using CRP.Mvc.Models.Sloth;
using CRP.Mvc.Resources;
using CRP.Mvc.Services;
using MvcContrib;
using Newtonsoft.Json;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class TransactionController : ApplicationController
    {
        private readonly INotificationProvider _notificationProvider;
        private readonly ISlothService _slothService;

        public TransactionController(INotificationProvider notificationProvider, ISlothService slothService)
        {
            _notificationProvider = notificationProvider;
            _slothService = slothService;
        }

        /// <summary>
        /// Tested 20200527
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);
            if (transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            
            return View(transaction);
        }

        /// <summary>
        /// GET: /Payment/LinkToTransaction/{id}
        /// </summary>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns></returns>
        public ActionResult Link(int transactionId, string sort, string page)
        {
            var transaction = Repository.OfType<Transaction>().GetNullableById(transactionId);
            if (transaction == null) return this.RedirectToAction<ItemManagementController>(a => a.List(null));

            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var viewModel = LinkPaymentViewModel.Create(Repository, transaction);
            viewModel.PaymentLogs = transaction.PaymentLogs.Where(a => a.Check);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Link(int transactionId, PaymentLog[] Checks, string checkSort, string checkPage)
        {
            ModelState.Clear();
            // get the transaction
            var transaction = Repository.OfType<Transaction>().GetNullableById(transactionId);
            if (transaction == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (transaction.Item == null || !Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            bool checkErrorFound = false;

            // go through and process the checks
            foreach (var check in Checks)
            {
                PaymentLog paymentLog;
                //check.DisplayCheckInvalidMessage = false;
                //if (check.Id <= 0 && check.Accepted && (string.IsNullOrEmpty(check.Name) || string.IsNullOrEmpty(check.Name.Trim()) || check.Amount <= 0.0m))
                //{                    
                //    check.DisplayCheckInvalidMessage = true;
                //    checkErrorFound = true;
                //}

                //Invalid ones will be rolled back from the record and not saved.
                // new check that is considered accepted
                if (check.Id <= 0 && check.Accepted) // && !string.IsNullOrEmpty(check.Name) && check.Amount > 0.0m)
                {
                    //If all these are empty, they probably just don't want it.
                    if (!string.IsNullOrEmpty(check.Name) || check.Amount != 0 || !string.IsNullOrEmpty(check.Notes))
                    {
                        paymentLog = Copiers.CopyCheckValues(check, new PaymentLog());
                        paymentLog.Check = true;
                        transaction.AddPaymentLog(paymentLog);
                        if (!paymentLog.IsValid())
                        {
                            //var test = paymentLog.ValidationResults();
                            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, paymentLog.ValidationResults());
                            paymentLog.DisplayCheckInvalidMessage = true;
                            checkErrorFound = true;
                        }
                    }
                }
                // update an existing one
                else if (check.Id > 0)
                {
                    var tempCheck = Repository.OfType<PaymentLog>().GetNullableById(check.Id);
                    paymentLog = Copiers.CopyCheckValues(check, tempCheck);
                    if (paymentLog.Id > 0 && paymentLog.Accepted)// && (string.IsNullOrEmpty(paymentLog.Name) || string.IsNullOrEmpty(paymentLog.Name.Trim()) || paymentLog.Amount <= 0.0m))
                    {
                        if (!paymentLog.IsValid())
                        {
                            var test = paymentLog.ValidationResults();
                            paymentLog.DisplayCheckInvalidMessage = true;
                            checkErrorFound = true;
                        }
                    }
                }
            }
            if (checkErrorFound)
            {
                ModelState.AddModelError("Check", "At least one check is invalid or incomplete");
            }

            //figure out the total of the checks
            var checktotal = transaction.PaymentLogs.Where(a => a.Accepted).Sum(a => a.Amount);
            var transactiontotal = transaction.Total;

            // more money is coming in than the transaction total, make a donation for the rest
            if (checktotal > transactiontotal)
            {
                //var donation = new Transaction(transaction.Item);
                //donation.Donation = true;
                //donation.Amount = checktotal - transactiontotal;

                //transaction.AddChildTransaction(donation);
                ModelState.AddModelError("Checks", "The check amount has exceeded the total amount. Enter a donation first.");
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Transaction>().EnsurePersistent(transaction);
                Message = "Checks associated with transaction.";
                if (transaction.Paid && !transaction.Notified)
                {
                    // attempt to get the contact information question set and retrieve email address
                    var question = transaction.TransactionAnswers.Where(a => a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && a.Question.Name == StaticValues.Question_Email).FirstOrDefault();
                    if (question != null)
                    {
                        // send an email to the user
                        _notificationProvider.SendConfirmation(Repository, transaction, question.Answer);
                    }
                }
                return Redirect(Url.Action("Details", "ItemManagement", new { id = transaction.Item.Id }) + "#Checks");
            }
            var viewModel = LinkPaymentViewModel.Create(Repository, transaction);
            viewModel.PaymentLogs = transaction.PaymentLogs.Where(a => a.Check);
            viewModel.AddBlankCheck = false; //We had errors, we will display what they entered without adding a new one.

            //JCS Ok we have an invalid object, where we have added paymentLogs, 
            //if they just go back to the list, the automatic commit will save the changes,
            //so tring a rollback...
            Repository.OfType<Transaction>().DbContext.RollbackTransaction(); //Moved after so invalid checks are not removed from the view

            return View(viewModel);
        }

        /// <summary>
        /// Edit Get
        /// Tested 20200519
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
        /// Tested 20200519
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
                return Redirect(Url.Action("Details", "ItemManagement", new { id = transactionToUpdate.Item.Id }) + "#Checks");
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
        [RefunderOnly]
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
                return RedirectToAction("Details", "ItemManagement", new { id = transaction.Item.Id });
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
        [RefunderOnly]
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
                return RedirectToAction("Details", "ItemManagement", new { id = transaction.Item.Id });
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

        [RefunderOnly]
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
                return RedirectToAction("Details", "ItemManagement", new { id = transactionToUpdate.Item.Id });
            }

            childTransaction.IsActive = false;
            Repository.OfType<Transaction>().EnsurePersistent(transactionToUpdate);
            if(transactionToUpdate.Credit)
            {
                var user = Repository.OfType<User>().Queryable.First(a => a.LoginID == CurrentUser.Identity.Name);
                _notificationProvider.SendRefundNotification(user, childTransaction, true);
            }
            return RedirectToAction("Details", "ItemManagement", new { id = transactionToUpdate.Item.Id });
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

            return Redirect(Url.Action("Details", "ItemManagement", new { id = transactionToUpdate.Item.Id }) + "#Notifications");

            //return RedirectToAction("Details", "ItemManagement", new { id = transactionToUpdate.Item.Id }); //Above uses the Fragment.

        }

        /// <summary>
        /// Details of the refund.
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
                return RedirectToAction("Details", "ItemManagement", new { id = transactionToView.Item.Id });

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

        [HttpPost]
        [AllowAnonymous]
        [BypassAntiForgeryToken]
        public async Task<ActionResult> DepositNotify(TransactionDepositNotification model)
        {
            // parse id
            if (!int.TryParse(model.MerchantTrackingNumber, out int transactionId))
            {
                return new JsonNetResult(new
                {
                    message = "merchant tracking number bad format",
                    success = false,
                });
            }

            // find transaction with payment
            var paymentLog = Repository.OfType<PaymentLog>().Queryable
                .FirstOrDefault(p => p.GatewayTransactionId == model.ProcessorTrackingNumber
                                  && p.Transaction.Id == transactionId);

            if (paymentLog == null)
            {
                return new JsonNetResult(new
                {
                    message = "transaction not found for merchant tracking number",
                    success = false,
                });
            }

            if (paymentLog.Cleared)
            {
                return new JsonNetResult(new
                {
                    message = "transaction already cleared",
                    success = false,
                });
            }

            // build transfer request
            var total = paymentLog.Amount;
            var fee = total * FeeSchedule.StandardRate;
            var income = total - fee;

            // create transfers
            var debitHolding = new CreateTransfer()
            {
                Amount      = total,
                Direction   = CreateTransfer.CreditDebit.Debit,
                Chart       = KfsAccounts.HoldingChart,
                Account     = KfsAccounts.HoldingAccount,
                ObjectCode  = KfsObjectCodes.Income,
                Description = "Funds Distribution"
            };

            var feeCredit = new CreateTransfer()
            {
                Amount      = fee,
                Direction   = CreateTransfer.CreditDebit.Credit,
                Chart       = KfsAccounts.FeeChart,
                Account     = KfsAccounts.FeeAccount,
                ObjectCode  = KfsObjectCodes.Income,
                Description = "Processing Fee"
            };

            var incomeCredit = new CreateTransfer()
            {
                Amount      = income,
                Direction   = CreateTransfer.CreditDebit.Credit,
                Chart       = paymentLog.Transaction.Item.FinancialAccount.Chart,
                Account     = paymentLog.Transaction.Item.FinancialAccount.Account,
                SubAccount  = paymentLog.Transaction.Item.FinancialAccount.SubAccount,
                ObjectCode  = KfsObjectCodes.Income,
                Description = "Funds Distribution"
            };

            // setup transaction
            var merchantUrl = Url.Action("Details", "Transaction",  new {id = paymentLog.Transaction.Id});

            var request = new CreateTransaction()
            {
                AutoApprove            = true,
                MerchantTrackingNumber = paymentLog.Transaction.Id.ToString(),
                MerchantTrackingUrl    = merchantUrl,
                KfsTrackingNumber      = model.KfsTrackingNumber,
                TransactionDate        = DateTime.UtcNow,
                Transfers              = new List<CreateTransfer>()
                {
                    debitHolding,
                    feeCredit, 
                    incomeCredit,
                },
                Source                 = "Registration CyberSource",
                SourceType             = "CyberSource",
                ProcessorTrackingNumber = paymentLog.GatewayTransactionId,
            };

            //var getIt = JsonConvert.SerializeObject(request); //Debug it so can test in swagger

            var response = await _slothService.CreateTransaction(request);

            // mark transaction as cleared
            paymentLog.Cleared = true;
            Repository.OfType<PaymentLog>().EnsurePersistent(paymentLog);

            return new JsonNetResult(new
            {
                success = true,
            });
        }
    }
}
