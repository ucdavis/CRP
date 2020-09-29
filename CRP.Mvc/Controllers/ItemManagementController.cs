using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.Helpers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.Helpers;
using CRP.Mvc.Controllers.ViewModels;
using CRP.Mvc.Controllers.ViewModels.ItemManagement;
using CRP.Services;
using MvcContrib;
using Serilog;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [UserOnly]
    public class ItemManagementController : ApplicationController
    {
        private readonly ICopyItemService _copyItemService;


        public ItemManagementController(ICopyItemService copyItemService)
        {
            _copyItemService = copyItemService;
        }

        //Tested 20200422
        // GET: /ItemManagement/

        public ActionResult Index()
        {
            return this.RedirectToAction(a => a.List(null));
        }

        /// <summary>
        /// Tested 20200422
        /// </summary>
        /// <param name="transactionNumber"></param>
        /// <returns></returns>
        public ActionResult List(string transactionNumber)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            var query = Repository.OfType<Item>().Queryable.Where(a => a.Editors.Any(b => b.User == user));

            // admins can see all
            if (CurrentUser.IsInRole(RoleNames.Admin))
            {
                query = Repository.OfType<Item>().Queryable;
            }

            // fetch matching transactions
            if (!string.IsNullOrEmpty(transactionNumber))
            {
                query = query.Where(a => a.Transactions.Any(b => b.ParentTransaction == null && b.TransactionNumber.Contains(transactionNumber)));
            }

            query = query.OrderByDescending(a => a.DateCreated);

            var slimmedDown = query.Select(a => new ItemListView
            {
                Id          = a.Id,
                Name        = a.Name,
                CostPerItem = a.CostPerItem,
                Quantity    = a.Quantity,
                Sold        = a.SoldCount,
                Expiration  = a.Expiration,
                DateCreated = a.DateCreated,
                Available   = a.Available
            }).ToList();

            return View(slimmedDown);
        }

        /// <summary>
        /// GET: /ItemManagement/Create
        /// Tested 20200505
        /// </summary>
        /// <returns></returns>
        [PageTracker]
        public ActionResult Create()
        {
            var viewModel = ItemViewModel.Create(Repository, CurrentUser, null);
            viewModel.IsNew = true;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/Create/
        /// Tested 20200505
        /// </summary>
        /// <remarks>
        /// Description:   
        ///     Create a new item
        /// PreCondition:
        ///     Item type was created
        /// PostCondition:
        ///     Item is created
        ///     Extended properties are created
        ///     Default question sets are added ("Contact Information" and Item Type defaults)
        /// </remarks>       
        /// <param name="item">The item.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="mapLink">The map link.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [PageTracker]
        public ActionResult Create(EditItemViewModel item, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink)
        {
            ModelState.Clear();

            // get the file and add it into the item
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                //var reader = new BinaryReader(file.InputStream);
                //item.Image = reader.ReadBytes(file.ContentLength);


                Image temp = Image.FromStream(file.InputStream);
                var temp2 = ImageHelper.ResizeImage(temp, 1200, 675);

                using (var ms = new MemoryStream())
                {
                    temp2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    item.Image = ms.ToArray();
                }
            }
            if (item.Image == null || item.Image.Length <= 0)
            {
                ModelState.AddModelError("Image", @"An image is required.");
            }

            //This was changed to a nullable decimal because text was setting the value to zero.
            //Check it here because the copyItem sets the value and a null would trow an exception.
            if (!item.CostPerItem.HasValue)
            {
                ModelState.AddModelError("Item.CostPerItem", "Please enter a valid amount (Just a number).");
                item.CostPerItem = 0m;
            }
            if (!item.Quantity.HasValue)
            {
                ModelState.AddModelError("Item.Quantity", "Please enter a number for the Quantity.");
                item.Quantity = 0;
            }

            // setup new item CanChangeFinanceAccount will always be true on a create
            var itemToCreate = Copiers.CopyItem(Repository, item, new Item(), extendedProperties, tags, mapLink, true);

            if (itemToCreate.ExtendedPropertyAnswers != null && itemToCreate.ExtendedPropertyAnswers.Count > 0)
            {
                foreach (var extendedPropertyAnswer in itemToCreate.ExtendedPropertyAnswers)
                {
                    if(string.IsNullOrWhiteSpace(extendedPropertyAnswer.Answer))
                    {
                        ModelState.AddModelError("Extended Properties", "All Extended Properties are required");
                        break;
                    }
                }
            }

            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            // add permissions
            var editor = new Editor(itemToCreate, user);
            editor.Owner = true;
            itemToCreate.AddEditor(editor);

            // set the unit
            //item.Unit = user.Units.FirstOrDefault();

            // add the required question set
            var questionSet =
                Repository.OfType<QuestionSet>().Queryable.Where(
                    a => a.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();

            // this should really not be null at any point.
            if (questionSet != null)
            {
                itemToCreate.AddTransactionQuestionSet(questionSet);
            }

            if (itemToCreate.ItemType != null)
            {
                // add the default question sets
                foreach (var qs in itemToCreate.ItemType.QuestionSets)
                {
                    if (qs.TransactionLevel)
                    {
                        itemToCreate.AddTransactionQuestionSet(qs.QuestionSet);
                    }
                    else
                    {
                        itemToCreate.AddQuantityQuestionSet(qs.QuestionSet);
                    }
                }
            }

            if (itemToCreate.Template == null)
            {
                var tempTemplate = Repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault();
                if (tempTemplate != null && !string.IsNullOrEmpty(tempTemplate.Text))
                {
                    var template = new Template(tempTemplate.Text);
                    itemToCreate.Template = template;
                }
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemToCreate.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(itemToCreate);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction(a => a.List(null));
            }
            else
            {
                var viewModel = ItemViewModel.Create(Repository, CurrentUser, itemToCreate);
                viewModel.IsNew = true;
                return View(viewModel);
            }
        }

        /// <summary>
        /// GET: /ItemManagement/GetExtendedProperties/{id}
        /// Tested 20200427
        /// </summary>
        /// <param name="id">Id of the item type</param>
        /// <returns></returns>
        public JsonNetResult GetExtendedProperties(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableById(id);

            if (itemType == null)
            {
                return new JsonNetResult(false);
            }

            return new JsonNetResult(itemType.ExtendedProperties);
        }

        /// <summary>
        /// Tested 20200422
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Map(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                return this.RedirectToAction(a => a.List(null));
            }

            //var viewModel = ItemViewModel.Create(Repository, CurrentUser, item); // For now use this view model, but all we really need it the item.

            //return View(viewModel);
            return View(item);

        }


        /// <summary>
        /// GET: /ItemManagement/Edit/{id}
        /// Tested main page, but not all tabs 20200505. Tested all tabs 20200511
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PageTracker]
        public ActionResult Edit(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                return this.RedirectToAction(a => a.List(null));
            }

            var viewModel = ItemViewModel.Create(Repository, CurrentUser, item);


            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/Edit/{id}
        /// Tested 20200505
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Updates an existing item
        /// PreCondition:
        ///     Item already exists
        ///     User has editor rights
        /// PostCondition:
        ///     Item is updated
        /// </remarks>
        /// <param name="id">The id.</param>
        /// <param name="item">The item.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="mapLink">The map link.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [PageTracker]
        public ActionResult Edit(int id, EditItemViewModel item, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink, bool CanChangeFinanceAccount )
        {
            ModelState.Clear();
            var destinationItem = Repository.OfType<Item>().GetNullableById(id);
            
            // check rights to edit
            if(destinationItem == null || !Access.HasItemAccess(CurrentUser, destinationItem))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            //This was changed to a nullable decimal because text was setting the value to zero.
            //Check it here because the copyItem sets the value and a null would trow an exception.
            if (!item.CostPerItem.HasValue)
            {
                ModelState.AddModelError("Item.CostPerItem", "Please enter a valid amount (Just a number).");
                item.CostPerItem = 0m;
            }

            if (!item.Quantity.HasValue)
            {
                ModelState.AddModelError("Item.Quantity", "Please enter a number for the Quantity.");
                item.Quantity = 0;
            }

            destinationItem = Copiers.CopyItem(Repository, item, destinationItem, extendedProperties, tags, mapLink, CanChangeFinanceAccount);

            if(destinationItem.ExtendedPropertyAnswers != null && destinationItem.ExtendedPropertyAnswers.Count > 0)
            {
                foreach(var extendedPropertyAnswer in destinationItem.ExtendedPropertyAnswers)
                {
                    if(string.IsNullOrWhiteSpace(extendedPropertyAnswer.Answer))
                    {
                        ModelState.AddModelError("Extended Properties", "All Extended Properties are required");
                        break;
                    }
                }
            }

            // get the file and add it into the item
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                //var reader = new BinaryReader(file.InputStream);
                //destinationItem.Image = reader.ReadBytes(file.ContentLength);
                
                Image temp = Image.FromStream(file.InputStream);

                var temp2 = ImageHelper.ResizeImage(temp, 1200, 675);

                using (var ms = new MemoryStream())
                {
                    temp2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    destinationItem.Image = ms.ToArray();
                }
            }
            if(destinationItem.Image == null ||destinationItem.Image.Length <=0)
            {
                ModelState.AddModelError("Image", @"An image is required.");
            }


            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, destinationItem.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(destinationItem);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction(a => a.Edit(id));
            }
            var viewModel = ItemViewModel.Create(Repository, CurrentUser, destinationItem);
            
            //viewModel.Item = destinationItem;
            return View(viewModel);
        }


        /// <summary>
        /// POST: /ItemManagement/RemoveEditor
        /// Tested 20200424
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Removes an editor's permission from the item
        /// PreCondition:
        ///     Editor is associated with the item
        ///     Item is valid
        /// PostCondition:
        ///     Item does not contain specified editor
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="editorId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveEditor(int id, int editorId)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);
            var editor = Repository.OfType<Editor>().GetNullableById(editorId);

            if (item == null || editor == null)
            {
                return this.RedirectToAction(a => a.List(null));
            }
            //Only allow an editor to be removed if the current user is in the editors -JCS
            //if (item.Editors == null || !item.Editors.Where(a => a.User.LoginID == CurrentUser.Identity.Name).Any())
            if (!Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            if (editor.Owner)
            {
                Message = NotificationMessages.STR_EditorCanNotBeRemoved; //"Can not remove owner from item.";
            }
            else
            {
                item.RemoveEditor(editor);

                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

                if (ModelState.IsValid)
                {
                    Repository.OfType<Item>().EnsurePersistent(item);
                    Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Editor");
                }
            }

            var redirectUrl = Url.Action("Edit", "ItemManagement", new { id });
            return Redirect(redirectUrl + "#Editors");
        }

        /// <summary>
        /// POST: /ItemManagement/AddEditor
        /// Tested 20200424
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Add an editor's permission to the item
        /// PreCondition:
        ///     Editor is not associated with the item
        ///     Item is valid
        ///     User is valid
        /// PostCondition:
        ///     Item has editor added with specified user
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEditor(int id, int? userId)
        {
            var returnUrl = Url.Action("Edit", "ItemManagement", new { id }) + "#Editors";

            if (!userId.HasValue)
            {
                Message = NotificationMessages.STR_SelectUserFirst;
                return Redirect(returnUrl);
            }

            var item = Repository.OfType<Item>().GetNullableById(id);
            var user = Repository.OfType<User>().GetNullableById(userId.Value);
            
            if (item == null || user == null)
            {
                return this.RedirectToAction(a => a.List(null));
            }


            //if (item.Editors == null || !item.Editors.Where(a => a.User.LoginID == CurrentUser.Identity.Name).Any())
            if (!Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            //TODO: Review if this is how it should behave if an editor is already attached to the item
            if(item.Editors.Where(a => a.User.LoginID == user.LoginID).Any())
            {
                Message = NotificationMessages.STR_EditorAlreadyExists;
                return Redirect(returnUrl);

            }

            item.AddEditor(new Editor(item, user));

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Editor");
            }
            else
            {
                Message = "Unable to add editor.";
            }

            return Redirect(returnUrl);
        }

        /// <summary>
        /// POST: /ItemManagement/SaveTemplate
        /// Tested 20200507
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Saves the template
        /// PreCondition:
        ///     Item is valid
        ///     String is not empty
        /// PostCondition:
        ///     Item's template is updated
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonNetResult SaveTemplate(int id, string textPaid, string textUnpaid)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);

            if (item == null || string.IsNullOrEmpty(textPaid) || !Access.HasItemAccess(CurrentUser, item))
            {
                return new JsonNetResult("Unable to save. No Access or Paid text was empty.");
            }

            var template = new Template(textPaid + StaticValues.ConfirmationTemplateDelimiter + textUnpaid);
            template.Default = false;
            template.Item = item;
            item.Template = template;

            if (template.Text.Trim() == StaticValues.ConfirmationTemplateDelimiter)
            {
                ModelState.AddModelError("Text", "text may not be null or empty");
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, template.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                return new JsonNetResult("Template Saved");
            }

            return new JsonNetResult("Unable to Save.");
        }

        /// <summary>
        /// GET: /ItemManagement/Details/{id}
        /// Tested 20200506
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            var viewModel = UserItemDetailViewModel.Create(Repository, item);

            return View(viewModel);
        }


        /// <summary>
        /// Toggles the transaction is active.
        /// TESTED 20200506 And fixed
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleTransactionIsActive(int id, string sort, string page)
        {
            // get transaction
            var transaction = Repository.OfType<Transaction>().GetNullableById(id);
            if (transaction == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Transaction");
                return this.RedirectToAction(a => a.List(null));
            }

            // check item
            if (transaction.Item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction(a => a.List(null));
            }

            // check user access
            if (!Access.HasItemAccess(CurrentUser, transaction.Item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            if (transaction.IsActive && transaction.Paid)
            {
                // you can't deactivate paid registrations
                ModelState.AddModelError("Deactivate", NotificationMessages.STR_Paid_transactions_can_not_be_deactivated);
            }
            else if (!transaction.IsActive && (transaction.Item.Sold + transaction.Quantity) > transaction.Item.Quantity)
            {
                // you can't activate too many transactions
                ModelState.AddModelError("Activate", NotificationMessages.STR_Transaction_can_not_be_activated);
            }

            // swap active
            transaction.IsActive = !transaction.IsActive;
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, transaction.ValidationResults());
            if (!ModelState.IsValid)
            {
                Message = NotificationMessages.STR_UnableToUpdate.Replace(NotificationMessages.ObjectType, "Transaction");
                var errors = string.Empty;
                try
                {
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors = $"{errors}{error.ErrorMessage}<br/>";
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.InnerException?.Message);
                }
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    ErrorMessage = errors;
                }
                return RedirectToAction("Details", "ItemManagement", new {id = transaction.Item.Id});
            }

            // persist transaction
            Repository.OfType<Transaction>().EnsurePersistent(transaction);

            // set message based on old state => new state
            Message = transaction.IsActive
                ? NotificationMessages.STR_Activated.Replace(NotificationMessages.ObjectType, "Transaction")
                : NotificationMessages.STR_Deactivated.Replace(NotificationMessages.ObjectType, "Transaction");

            try
            {
                // update counts
                if (Repository.OfType<Transaction>().Queryable.Any(a => a.Item.Id == transaction.Item.Id && a.IsActive))
                {
                    transaction.Item.SoldCount = Repository.OfType<Transaction>().Queryable
                        .Where(a => a.Item.Id == transaction.Item.Id && a.IsActive)
                        .Sum(a => a.Quantity);
                }
                else
                {
                    transaction.Item.SoldCount = 0;
                }
                
                // persist item
                Repository.OfType<Item>().EnsurePersistent(transaction.Item);
            }
            catch (Exception ex)
            {
                Log.Error("Error updating Item SoldCount", ex);
            }

            return RedirectToAction("Details", "ItemManagement", new {id = transaction.Item.Id});
        }

        /// <summary>
        /// Tested 20200511
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PageTracker]
        public ActionResult Copy(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List(null));
            }

            var newItem = _copyItemService.Copy(item, Repository, CurrentUser.Identity.Name);
            if (!newItem.IsValid())
            {
                var errorMessages = new StringBuilder();
                var errors = newItem.ValidationResults();
                foreach (var validationResult in errors)
                {
                    errorMessages.AppendLine(validationResult.Message);
                }

                Message = "The copy was not able to save because of Invalid Data: " + errorMessages;
                return this.RedirectToAction<ErrorController>(a => a.Index(ErrorController.ErrorType.UnknownError));
            }

            return this.RedirectToAction(a => a.Edit(newItem.Id)); 
        }
    }

    public class ExtendedPropertyParameter
    {
        // ReSharper disable InconsistentNaming
        public string value { get; set; }
        public int propertyId { get; set; }
        // ReSharper restore InconsistentNaming
    }
}
