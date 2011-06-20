using System;
using System.Linq;
using System.Web.Mvc;
//using CRP.App_GlobalResources;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using UCDArch.Web.Attributes;
using MvcContrib;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers
{
    [Authorize]
    public class QuestionSetController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<School, string> _schoolRepository;

        public QuestionSetController(IRepositoryWithTypedId<School, string> schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        //
        // GET: /QuestionSet/

        public ActionResult Index()
        {
            return this.RedirectToAction(a => a.List());
        }

        /// <summary>
        /// GET: /QuestionSet/List
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            IQueryable query;
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();
            Check.Require(user != null, "User is required.");
            var schools = user.Units.Select(a => a.School).ToList();

            if (CurrentUser.IsInRole(RoleNames.Admin))
            {
                query = from a in Repository.OfType<QuestionSet>().Queryable
                        where a.SystemReusable || (a.UserReusable && a.User == user)
                            || (a.CollegeReusable && schools.Contains(a.School))
                        select a;
            }
            else if (CurrentUser.IsInRole(RoleNames.SchoolAdmin))
            {
                query = from a in Repository.OfType<QuestionSet>().Queryable
                        where (a.UserReusable && a.User == user) || (a.CollegeReusable && schools.Contains(a.School))
                        select a;
            }
            else
            {
                query = from a in Repository.OfType<QuestionSet>().Queryable
                        where (a.UserReusable && a.User == user)
                        select a;
            }

            return View(query);
        }

        public ActionResult Details(int id)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(id);

            if (questionSet == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            return View(questionSet);
        }

        /// <summary>
        /// GET: /QuestionSet/Edit/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(id);
            if (questionSet == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, questionSet))
            {
                Message = "You do not have access to the requested Question Set.";
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = QuestionSetViewModel.Create(Repository, CurrentUser, _schoolRepository);
            viewModel.QuestionSet = questionSet;

            if (!questionSet.SystemReusable && !questionSet.CollegeReusable && !questionSet.UserReusable)
            {
                // there should only be one item type or item associated with this question set since it's not reusable
                var itemType = questionSet.ItemTypes.FirstOrDefault();
                var item = questionSet.Items.FirstOrDefault();

                if (itemType != null)
                {
                    viewModel.ItemType = itemType.ItemType;
                }

                if (item != null)
                {
                    viewModel.Item = item.Item;
                }
            }

            return View(viewModel);
        }

        /// <summary>
        /// POST: /QuestionSet/Edit/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Edits a question set.
        /// PreCondition:
        ///     Question set exists
        ///     Question set is not the system default "Contact Information"
        /// PostCondition:
        ///     Question set is updated
        /// </remarks>
        /// <param name="questionSet"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, QuestionSet questionSet)
        {
            var existingQs = Repository.OfType<QuestionSet>().GetNullableById(id);

            // check for valid question set and access
            if (existingQs == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, existingQs))
            {
                Message = "You do not have access to the requested Question Set.";
                return this.RedirectToAction(a => a.List());
            }

            // check to make sure it isn't the system's default contact information set
            //This check MUST happen before we change the name
            if (existingQs.Name == StaticValues.QuestionSet_ContactInformation && existingQs.SystemReusable)
            {
                Message = "This is a system default question set and cannot be modified";
                ModelState.AddModelError("Question Set", "This is a system default question set and cannot be modified");
            }

            // copy the fields
            existingQs.Name = questionSet.Name;
            existingQs.IsActive = questionSet.IsActive;

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, existingQs.ValidationResults());

            
            if (questionSet.Name.ToLower() == StaticValues.QuestionSet_ContactInformation.ToLower())
            {
                ModelState.AddModelError("Name", StaticValues.QuestionSet_ContactInformation + " is reserved for internal system use only.");
                Message = StaticValues.QuestionSet_ContactInformation + " is reserved for internal system use only.";
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<QuestionSet>().EnsurePersistent(existingQs);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Question Set");
            }

            var viewModel = QuestionSetViewModel.Create(Repository, CurrentUser, _schoolRepository);
            viewModel.QuestionSet = existingQs;

            if (!existingQs.SystemReusable && !existingQs.CollegeReusable && !existingQs.UserReusable)
            {
                // there should only be one item type or item associated with this question set since it's not reusable
                var itemType = existingQs.ItemTypes.FirstOrDefault();
                var item = existingQs.Items.FirstOrDefault();

                if (itemType != null)
                {
                    viewModel.ItemType = itemType.ItemType;
                }

                if (item != null)
                {
                    viewModel.Item = item.Item;
                }
            }

            return View(viewModel);
        }

        /// <summary>
        /// GET: /QuestionSet/Create
        /// </summary>
        /// <param name="itemId">The id for an item, if it is to be automatically associated with an item</param>
        /// <param name="itemTypeId">The id for an itemType, if it is to be automatically associated with an itemType</param>
        /// <param name="transaction">If adding to an item or item type, the questionSet type to add it to</param>
        /// <param name="quantity">If adding to an item or item type, the questionSet type to add it to</param>
        /// <returns></returns>
        public ActionResult Create(int? itemId, int? itemTypeId, bool? transaction, bool? quantity)
        {
            var viewModel = QuestionSetViewModel.Create(Repository, CurrentUser, _schoolRepository);

            // set the item or item type as needed
            if (itemId.HasValue)
            {
                viewModel.Item = Repository.OfType<Item>().GetNullableById(itemId.Value);
                if (viewModel.Item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                    return this.RedirectToAction(a => a.List());
                }
            }
            if (itemTypeId.HasValue)
            {
                viewModel.ItemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId.Value);
                if(viewModel.ItemType == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemType");
                    return this.RedirectToAction(a => a.List());
                }
            }

            viewModel.Transaction = transaction.HasValue ? transaction.Value : false;
            viewModel.Quantity = quantity.HasValue ? quantity.Value : false;

            // display reusable options
            if (CurrentUser.IsInRole(RoleNames.Admin))
            {
                viewModel.IsAdmin = true;
            }
            else if (CurrentUser.IsInRole(RoleNames.SchoolAdmin))
            {
                viewModel.IsSchoolAdmin = true;
            }
            else
            {
                viewModel.IsUser = true;
            }
            
            return View(viewModel);
        }

        /// <summary>
        /// POST: /QuestionSet/Create
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a question set and automatically associates it to an item if an item itemId is provided.
        /// PreCondition:
        ///     Questionset does not exist with the same name if it is reusable at each level of reusability
        /// PostCondition:
        ///     Questionset is created with no question
        ///     if item itemId is given, question set is assigend to item
        /// </remarks>
        /// <param name="itemId">the id for an item to be assigned to</param>
        /// <param name="itemTypeId">the item type id to be associated with</param>
        /// <param name="questionSet"></param>
        /// <param name="school"></param>
        /// <param name="transaction"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        [HandleTransactionsManually]
        public ActionResult Create(int? itemId, int? itemTypeId, [Bind(Exclude="Id")]QuestionSet questionSet, string school, bool? transaction, bool? quantity)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();
            questionSet.User = user;

            if (questionSet.CollegeReusable) questionSet.School = _schoolRepository.GetNullableById(school);

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, questionSet.ValidationResults());

            if (itemId.HasValue || itemTypeId.HasValue)
            {
                if (!transaction.HasValue || !quantity.HasValue)
                {
                    ModelState.AddModelError("Transaction/Quantity", "Transaction or Quantity must be specified.");
                }
                else if (transaction.Value == quantity.Value)
                {
                    ModelState.AddModelError("Transaction/Quantity", "Transaction and Quantity cannot be the same.");
                }
            }
            if (questionSet.Name.ToLower() == "Contact Information".ToLower())
            {
                ModelState.AddModelError("Name", "Contact Information is reserved for internal system use only.");
            }

            //Validation check before we persist, shouldn't really be needed
            if (itemId.HasValue)
            {
                var item = Repository.OfType<Item>().GetNullableById(itemId.Value);
                if (item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                    return this.RedirectToAction(a => a.List());
                }
                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());
            }
            else if (itemTypeId.HasValue)
            {
                var itemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId.Value);
                if (itemType == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemType");
                    return this.RedirectToAction(a => a.List());
                }
                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());
            }
            if (ModelState.IsValid)
            {
                if (itemId.HasValue)
                {
                    // has an item to automatically associate to
                    var item = Repository.OfType<Item>().GetById(itemId.Value);
                    
                    if (transaction.Value)
                    {
                        item.AddTransactionQuestionSet(questionSet);
                    }
                    else
                    {
                        item.AddQuantityQuestionSet(questionSet);
                    }

                    using (var ts = new TransactionScope())
                    {
                        Repository.OfType<Item>().EnsurePersistent(item);
                        ts.CommitTransaction();
                    }

                    Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
                    return this.RedirectToAction(a => a.Edit(questionSet.Id));
                }
                else if (itemTypeId.HasValue)
                {
                    var itemType = Repository.OfType<ItemType>().GetById(itemTypeId.Value);

                    if (transaction.Value)
                    {
                        itemType.AddTransactionQuestionSet(questionSet);
                    }
                    else
                    {
                        itemType.AddQuantityQuestionSet(questionSet);
                    }

                    using (var ts = new TransactionScope())
                    {
                        Repository.OfType<ItemType>().EnsurePersistent(itemType);
                        ts.CommitTransaction();
                    }

                    Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
                    return this.RedirectToAction(a => a.Edit(questionSet.Id));
                }
                else
                {
                    if (!CurrentUser.IsInRole(RoleNames.Admin) && !CurrentUser.IsInRole(RoleNames.SchoolAdmin) && CurrentUser.IsInRole(RoleNames.User))
                    {
                        questionSet.UserReusable = true;
                    }

                    //make sure it's some type of reusable
                    if (questionSet.SystemReusable || questionSet.CollegeReusable || questionSet.UserReusable)
                    {
                        using (var ts = new TransactionScope())
                        {
                            Repository.OfType<QuestionSet>().EnsurePersistent(questionSet);
                            ts.CommitTransaction();
                        }

                        Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
                        return this.RedirectToAction(a => a.Edit(questionSet.Id));
                    }
                    else
                    {
                        ModelState.AddModelError("Reusable", "Question set must be reusable of some sort.");
                    }
                }
            }

            // return to the view
            var viewModel = QuestionSetViewModel.Create(Repository, CurrentUser, _schoolRepository);
            viewModel.QuestionSet = questionSet;

            if (itemId.HasValue) viewModel.Item = Repository.OfType<Item>().GetNullableById(itemId.Value);
            if (itemTypeId.HasValue) viewModel.ItemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId.Value);

            viewModel.Transaction = transaction.HasValue ? transaction.Value : false;
            viewModel.Quantity = quantity.HasValue ? quantity.Value : false;

            if (CurrentUser.IsInRole(RoleNames.Admin))
            {
                viewModel.IsAdmin = true;
            }
            else if (CurrentUser.IsInRole(RoleNames.SchoolAdmin))
            {
                viewModel.IsSchoolAdmin = true;
            }
            else
            {
                viewModel.IsUser = true;
            }

            return View(viewModel);    
        }

        /// <summary>
        /// GET: /QuestionSet/LinkToItemType/{itemTypeId}
        /// </summary>
        /// <param name="itemTypeId">The id of the item type to link to</param>
        /// <param name="transaction"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult LinkToItemType(int itemTypeId, bool transaction, bool quantity)
        {
            var item = Repository.OfType<ItemType>().GetNullableById(itemTypeId);

            if (item == null || transaction == quantity)
            {
                // redirect no item to link to
                if(item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemType");
                }
                else
                {
                    Message = NotificationMessages.STR_TransactionQuantitySame;
                }
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }

            var viewModel = QuestionSetLinkViewModel.Create(Repository, CurrentUser.Identity.Name);
            viewModel.ItemTypeId = itemTypeId;
            viewModel.Transaction = transaction;
            viewModel.Quantity = quantity;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /QuestionSet/LinkToItemType/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Adds a question set to the selected item type
        /// Assumption:
        ///     User has access to modify item type
        /// PreCondition:
        ///     Item Type has been created
        /// PostCondition:
        ///     Item type has question set associated with it
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="itemTypeId">The item type id.</param>
        /// <param name="transaction">if set to <c>true</c> [transaction].</param>
        /// <param name="quantity">if set to <c>true</c> [quantity].</param>
        /// <returns></returns>
        [HttpPost]
        [AdminOnly]
        public ActionResult LinkToItemType(int id, int itemTypeId, bool transaction, bool quantity)
        {
            // get the question set
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(id);
            var itemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId);

            if (questionSet == null || itemType == null || transaction == quantity)
            {
                if(questionSet == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "QuestionSet");
                }
                else if (itemType == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemType");
                }
                else
                {
                    Message = NotificationMessages.STR_TransactionQuantitySame;
                }
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }

            foreach (var itemTypeQuestionSet in itemType.QuestionSets)
            {
                if(itemTypeQuestionSet.QuestionSet == questionSet)
                {
                    ModelState.AddModelError("Question Set", "QuestionSet \"" + questionSet.Name.Trim() + "\" is already added to the ItemType.");
                    Message = "QuestionSet was already added";
                    break;
                }
            }

            // add the questionset
            if (transaction)
            {
                itemType.AddTransactionQuestionSet(questionSet);
            }
            else if (quantity)
            {
                itemType.AddQuantityQuestionSet(questionSet);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());

            // check if it's a contact information question set
            if (questionSet.Name == StaticValues.QuestionSet_ContactInformation) ModelState.AddModelError("Question Set Name", "A duplicate Contact Information question set cannot be added.");

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemType>().EnsurePersistent(itemType);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
            }
            else
            {
                return LinkToItemType(itemTypeId, transaction, quantity);
            }

            return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
        }

        /// <summary>
        /// Get: /QuestionSet/LinkToItem
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="transaction">if set to <c>true</c> [transaction].</param>
        /// <param name="quantity">if set to <c>true</c> [quantity].</param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult LinkToItem(int itemId, bool transaction, bool quantity)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (item == null || transaction == quantity)
            {
                // redirect no item to link to
                if (item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_TransactionQuantitySame;
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var viewModel = QuestionSetLinkViewModel.Create(Repository, CurrentUser.Identity.Name);
            viewModel.ItemId = itemId;
            viewModel.Transaction = transaction;
            viewModel.Quantity = quantity;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /QuestionSet/LinkToItem/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Adds a question set to an item
        /// Assumption:
        ///     Item exists
        ///     User is an editor for that item
        ///     User has the "User" role
        /// PreCondition:
        ///     Item exists with no transactions against it
        /// PostCondition:
        ///     Question set is assigned to the item
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="itemId"></param>
        /// <param name="transaction"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        [UserOnly]
        public ActionResult LinkToItem(int id, int itemId, bool transaction, bool quantity)
        {
            // get teh question set
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(id);
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (questionSet == null || item == null || transaction == quantity)
            {
                if (questionSet == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "QuestionSet");
                }
                else if (item == null)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                }
                else
                {
                    Message = NotificationMessages.STR_TransactionQuantitySame;
                }
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (!Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            
            // add the questionset
            if (transaction)
            {
                item.AddTransactionQuestionSet(questionSet);
            }
            else if (quantity)
            {
                item.AddQuantityQuestionSet(questionSet);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            // check if it's a contact information question set
            if (questionSet.Name == StaticValues.QuestionSet_ContactInformation) ModelState.AddModelError("Question Set Name", "A duplicate Contact Information question set cannot be added.");

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
            }
            else
            {
                if(item.QuestionSets.Where(a => a.QuestionSet == questionSet).Where(b => b.TransactionLevel).Count() > 1)
                {
                    Message = "That question set is already linked to the transaction question sets";
                }
                else if (item.QuestionSets.Where(a => a.QuestionSet == questionSet).Where(b => b.QuantityLevel).Count() > 1)
                {
                    Message = "That question set is already linked to the quantity question sets";
                }
                else
                {
                    Message = "An error with the item prevents this from saving.";
                }                
                return LinkToItem(itemId, transaction, quantity);
            }

            //return Redirect(ReturnUrlGenerator.EditItemUrl(itemId, StaticValues.Tab_Questions));
            return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_Questions));

        }

        /// <summary>
        /// POST: /QuestionSet/UnlinkFromItem
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Removes a question set to an item
        /// Assumption:
        ///     Item exists
        ///     Question set exists
        ///     ItemQuestionSet object exists
        ///     User is an editor for that item
        ///     User has the "User" role
        /// PreCondition:
        ///     Item exists with no transactions against it
        /// PostCondition:
        ///     Question set is removed from the item
        /// </remarks>
        /// <param name="id">Item Question Set Id</param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        [UserOnly]
        public ActionResult UnlinkFromItem(int id)
        {
            var itemQuestionSet = Repository.OfType<ItemQuestionSet>().GetNullableById(id);
            if (itemQuestionSet == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemQuestionSet");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            if (!Access.HasItemAccess(CurrentUser, itemQuestionSet.Item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var itemId = itemQuestionSet.Item.Id;
            
            //We are removing it, we don't care about any invalid data
            //MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemQuestionSet.ValidationResults());

            // check to make sure it isn't the system's default contact information set
            var ciQuestionSet = Repository.OfType<QuestionSet>().GetNullableById(itemQuestionSet.QuestionSet.Id);
            if (ciQuestionSet != null)
            {
                if (ciQuestionSet.Name == StaticValues.QuestionSet_ContactInformation && ciQuestionSet.SystemReusable)
                {
                    ModelState.AddModelError("Question Set",
                                             "This is a system default question set and cannot be modified.");
                }
            }

            // check to make sure there aren't any answers already
            if (Repository.OfType<TransactionAnswer>().Queryable.Where(a => a.QuestionSet == itemQuestionSet.QuestionSet).Any())
            {
                ModelState.AddModelError("Question Set", "Someone has already entered a response to this question set and it cannot be deleted.");
            }

            if(ModelState.IsValid)
            {
                Repository.OfType<ItemQuestionSet>().Remove(itemQuestionSet);
                Message = NotificationMessages.STR_ObjectRemoved.Replace(NotificationMessages.ObjectType, "Question Set");
            }
            else
            {
                Message = "Unable to remove question set.";
            }

            //return Redirect(ReturnUrlGenerator.EditItemUrl(itemId, StaticValues.Tab_Questions));
            return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_Questions));
        }
    }
}
