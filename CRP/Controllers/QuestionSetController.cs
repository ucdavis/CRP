using System;
using System.Linq;
using System.Web.Mvc;
using CRP.App_GlobalResources;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using UCDArch.Web.Attributes;
using MvcContrib;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers
{
    [Authorize]
    public class QuestionSetController : SuperController
    {
        //
        // GET: /QuestionSet/

        public ActionResult Index()
        {
            return View();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// GET: /QuestionSet/Edit/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(id);
            if (questionSet == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, questionSet))
            {
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = QuestionSetViewModel.Create(Repository);
            viewModel.QuestionSet = questionSet;

            if (!questionSet.SystemReusable || !questionSet.CollegeReusable || !questionSet.UserReusable)
            {
                // there should only be one item type or item associated with this question set since it's not reusable
                var itemType = questionSet.ItemTypes.FirstOrDefault();
                var item = questionSet.Items.FirstOrDefault();

                viewModel.ItemType = itemType;

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
        /// PostCondition:
        /// </remarks>
        /// <param name="questionSet"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Edit(QuestionSet questionSet)
        {
            // check access
            if (!Access.HasQuestionSetAccess(Repository, CurrentUser, questionSet))
            {
                return this.RedirectToAction(a => a.List());
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, questionSet.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<QuestionSet>().EnsurePersistent(questionSet);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Question Set");
            }

            var viewModel = QuestionSetViewModel.Create(Repository);
            viewModel.QuestionSet = questionSet;

            return View(viewModel);
        }

        /// <summary>
        /// GET: /QuestionSet/Create
        /// </summary>
        /// <param name="itemId">The id for an item, if it is to be automatically associated with an item</param>
        /// <returns></returns>
        public ActionResult Create(int? itemId, int? itemTypeId)
        {
            var viewModel = QuestionSetViewModel.Create(Repository);

            if (itemId.HasValue) {
                viewModel.Item = Repository.OfType<Item>().GetById(itemId.Value); 
            }
            if (itemTypeId.HasValue) {
                viewModel.ItemType = Repository.OfType<ItemType>().GetById(itemTypeId.Value);
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
        /// <returns></returns>
        [AcceptPost]
        [HandleTransactionsManually]
        public ActionResult Create(int? itemId, int? itemTypeId, [Bind(Exclude="Id")]QuestionSet questionSet)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();
            questionSet.User = user;
            if (questionSet.CollegeReusable) {
                //TODO: Review that this is ok
                questionSet.School = user.Units.First().School; 
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, questionSet.ValidationResults());

            if (ModelState.IsValid)
            {
                if (itemId.HasValue)
                {
                    // has an item to automatically associate to
                    var item = Repository.OfType<Item>().GetById(itemId.Value);
                    item.AddQuestionSet(questionSet);

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
                    itemType.AddQuestionSet(questionSet);

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
            var viewModel = QuestionSetViewModel.Create(Repository);
            viewModel.QuestionSet = questionSet;

            if (itemId.HasValue)
            {
                viewModel.Item = Repository.OfType<Item>().GetById(itemId.Value);
            }

            return View(viewModel);    
        }

        /// <summary>
        /// GET: /QuestionSet/LinkToItemType/{itemTypeId}
        /// </summary>
        /// <param name="itemTypeId">The id of the item type to link to</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult LinkToItemType(int itemTypeId)
        {
            var item = Repository.OfType<ItemType>().GetNullableByID(itemTypeId);

            if (item == null)
            {
                // redirect no item to link to
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }

            var viewModel = QuestionSetLinkViewModel.Create(Repository, CurrentUser.Identity.Name);
            viewModel.ItemTypeId = itemTypeId;

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
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="itemTypeId"></param>
        /// <returns></returns>
        [AcceptPost]
        [Authorize(Roles="Admin")]
        public ActionResult LinkToItemType(int id, int itemTypeId)
        {
            // get teh question set
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(id);
            var itemType = Repository.OfType<ItemType>().GetNullableByID(itemTypeId);

            if (questionSet == null || itemType == null)
            {
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());

            if (ModelState.IsValid)
            {
                itemType.AddQuestionSet(questionSet);
                Repository.OfType<ItemType>().EnsurePersistent(itemType);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set");
            }
            else
            {
                return LinkToItemType(itemTypeId);
            }

            return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
        }
    }
}
