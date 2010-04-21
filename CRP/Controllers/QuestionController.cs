using System;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class QuestionController : SuperController
    {
        //
        // GET: /Question/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Question/Create/{id}
        /// </summary>
        /// <param name="id">Question set Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(id);
            if (questionSet == null)
            {
                // go back to the question set edit view
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            var viewModel = QuestionViewModel.Create(Repository);
            viewModel.QuestionSet = questionSet;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /Question/Create/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a question and associates it with a question set.
        /// PreCondition:
        /// PostCondition:
        /// </remarks>
        /// <param name="id">Question set Id</param>
        /// <param name="question"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int id, [Bind(Exclude="Id")]Question question, string[] questionOptions)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(id);
            if (questionSet == null)
            {
                // go back to the question set edit view
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            // process the options
            foreach(string s in questionOptions)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    var option = new QuestionOption(s);
                    question.AddOption(option);
                }
            }

            // add the question
            questionSet.AddQuestion(question);

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, question.ValidationResults());

            if (ModelState.IsValid)
            {
                // valid redirect to edit
                Repository.OfType<QuestionSet>().EnsurePersistent(questionSet);
                Message = "Question has been created.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(id));
            }
            else
            {
                // not valid, go back
                var viewModel = QuestionViewModel.Create(Repository);
                viewModel.QuestionSet = questionSet;
                viewModel.Question = question;

                return View(viewModel);
            }
        }

        public ActionResult Edit(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
