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
            return View(Repository.OfType<QuestionSet>().Queryable);
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
            if (questionSet == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = QuestionSetViewModel.Create(Repository);
            viewModel.QuestionSet = questionSet;

            return View(viewModel);
        }

        /// <summary>
        /// GET: /QuestionSet/Create
        /// </summary>
        /// <param name="id">The id for an item, if it is to be automatically associated with an item</param>
        /// <returns></returns>
        public ActionResult Create(int? id)
        {
            var viewModel = QuestionSetViewModel.Create(Repository);

            if (id.HasValue) {
                viewModel.Item = Repository.OfType<Item>().GetById(id.Value); 
            }
            
            return View(viewModel);
        }

        /// <summary>
        /// POST: /QuestionSet/Create
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a question set and automatically associates it to an item if an item id is provided.
        /// PreCondition:
        ///     Questionset does not exist with the same name if it is reusable at each level of reusability
        /// PostCondition:
        ///     Questionset is created with no question
        ///     if item id is given, question set is assigend to item
        /// </remarks>
        /// <param name="id">the id for an item to be assigned to</param>
        /// <param name="questionSet"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int? id, [Bind(Exclude="Id")]QuestionSet questionSet)
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
                if (id.HasValue)
                {
                    // has an item to automatically associate to
                    var item = Repository.OfType<Item>().GetById(id.Value);
                    item.AddQuestionSet(questionSet);

                    Repository.OfType<Item>().EnsurePersistent(item);
                    return this.RedirectToAction(a => a.Edit(questionSet.Id));
                }
                else
                {
                    //make sure it's some type of reusable
                    if (questionSet.SystemReusable || questionSet.CollegeReusable || questionSet.UserReusable)
                    {
                        Repository.OfType<QuestionSet>().EnsurePersistent(questionSet);
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

            if (id.HasValue)
            {
                viewModel.Item = Repository.OfType<Item>().GetById(id.Value);
            }

            return View(viewModel);    
        }
    }
}
