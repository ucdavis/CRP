using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;

namespace CRP.Controllers
{
    public class ApplicationManagementController : SuperController
    {
        //
        // GET: /ApplicationManagement/

        public ActionResult Index()
        {
            return View();
        }

        #region Item Types

        /// <summary>
        /// GET: /ApplicationManagement/ListItemTypes
        /// </summary>
        /// <returns></returns>
        public ActionResult ListItemTypes()
        {
            return View(Repository.OfType<ItemType>().GetAll());
        }

        /// <summary>
        /// GET: /ApplicationManagement/CreateItemType
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateItemType()
        {
            return View(ItemTypeViewModel.Create(Repository));
        }

        /// <summary>
        /// POST: /ApplicationManagement/CreateItemType
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a new item type with defined extended properties
        /// PreCondition:
        ///     Item type with same name doesn't already exist
        /// PostCondition:
        ///     Item is created
        ///     Extended properties passed in are saved
        /// </remarks>
        /// <param name="itemType"></param>
        /// <param name="extendedProperties"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult CreateItemType(ItemType itemType, ExtendedProperty[] extendedProperties)
        {
            foreach(var ep in extendedProperties)
            {
                ep.ItemType = itemType;

                if (ep.IsValid())
                {
                    itemType.AddExtendedProperty(ep);
                }
                else
                {
                    ModelState.AddModelError("ExtendedProperty", "At least one extended property is not valid.");
                }
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());

            // make sure the item type doesn't already exist with the same name
            if (Repository.OfType<ItemType>().Queryable.Where(a => a.Name == itemType.Name).Any())
            {
                // name already exists, we have a problem
                ModelState.AddModelError("Name", "A item type of the same name already exists.");
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemType>().EnsurePersistent(itemType);
                Message = "Item Type has been saved.";
                return this.RedirectToAction(a => a.ListItemTypes());
            }
            else
            {
                var viewModel = ItemTypeViewModel.Create(Repository);
                viewModel.ItemType = itemType;

                return View(viewModel);
            }
        }

        /// <summary>
        /// GET: /ApplicationManagement/EditItemType/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditItemType(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

            if (itemType != null)
            {
                return View(itemType);
            }
            else
            {
                return this.RedirectToAction(a => a.ListItemTypes());
            }
        }

        #endregion

        #region Question Sets
        /// <summary>
        /// GET: /ApplicationManagement/ListQuestionSets
        /// </summary>
        /// <returns></returns>
        public ActionResult ListQuestionSets()
        {
            return View(Repository.OfType<QuestionSet>().Queryable.Where(a => a.SystemReusable));
        }

        /// <summary>
        /// GET: /ApplicationManagement/CreateQuestionSet
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateQuestionSet()
        {
            var viewModel = QuestionSetViewModel.Create(Repository);
            viewModel.QuestionSet = new QuestionSet();

            return View(viewModel);
        }

        /// <summary>
        /// POST: /ApplicationManagement/CreateQuestionSet
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Create a question set that is system reusable 
        /// PreCondition:
        ///     Question set with the same name that is system reusable does not exist.
        /// PostCondition:
        ///     Question set is created with one or more questions
        ///     Questions if type needs options has one or more options
        /// </remarks>
        /// <param name="questionSet"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult CreateQuestionSet(QuestionSet questionSet)//, string[] questionNames, QuestionType[] questionTypes, string[][] options)
        {

            throw new NotImplementedException();

            //// make the question set system reusable
            //questionSet.SystemReusable = true;
            //questionSet.User = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            //// transfer the validation information
            //MvcValidationAdapter.TransferValidationMessagesTo(ModelState, questionSet.ValidationResults());

            //// validate the questions before adding them
            //foreach(var q in questions)
            //{
            //    if (q.IsValid())
            //    {
            //        questionSet.AddQuestion(q);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("Question", "There was a problem with one or more questions.");
            //        break;
            //    }
            //}

            //if (ModelState.IsValid)
            //{
            //    // is valid save
            //    Repository.OfType<QuestionSet>().EnsurePersistent(questionSet);
            //    Message = "Question set was created.";
            //    return this.RedirectToAction(a => a.ListQuestionSets());
            //}
            //else
            //{
            //    // not valid, go back
            //    var viewModel = QuestionSetViewModel.Create(Repository);
            //    // push all the questions so that it gets regenerated on the page for the users
            //    foreach(var q in questions)
            //    {
            //        questionSet.AddQuestion(q);
            //    }
            //    viewModel.QuestionSet = questionSet;
            //    return View(viewModel);
            //}
        }
        #endregion
    }
}
