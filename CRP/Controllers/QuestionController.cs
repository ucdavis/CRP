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
    [Authorize]
    public class QuestionController : SuperController
    {
        //
        // GET: /Question/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Question/Create/{questionSetId}
        /// </summary>
        /// <param name="questionSetId">Question set Id</param>
        /// <returns></returns>
        public ActionResult Create(int questionSetId)
        {
            //TODO: Check to make sure the user is allowed to edit the QuestionSet

            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(questionSetId);
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
        /// POST: /Question/Create/{questionSetId}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a question and associates it with a question set.
        /// Assumption:
        ///     User trying to create the question is someone allowed to edit the question set container
        /// PreCondition:
        ///     Question Set already exists
        ///     Question Set does not already have any items associated with it
        /// PostCondition:
        ///     Question was created
        ///     Options were created if question needs them
        /// </remarks>
        /// <param name="questionSetId">Question set Id</param>
        /// <param name="question"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int questionSetId, [Bind(Exclude="Id")]Question question, string[] questionOptions)
        {
            //TODO: Check to make sure the user is allowed to edit the QuestionSet

            var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(questionSetId);
            if (questionSet == null)
            {
                // go back to the question set edit view
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            // Check to make sure the question set hasn't been used yet
            if (questionSet.Items.Count > 0)
            {
                Message = "Question cannot be added to the question set becuase it is already being used by an item.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // process the options
            if (question.QuestionType.HasOptions && questionOptions != null)
            {
                foreach (string s in questionOptions)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var option = new QuestionOption(s);
                        question.AddOption(option);
                    }
                }
            }

            // add the question
            questionSet.AddQuestion(question);

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, question.ValidationResults());

            // check to make sure there are options if needed
            if (question.QuestionType.HasOptions && question.Options.Count <= 0)
            {
                ModelState.AddModelError("Options", "The question type requires at least one option.");
            }

            if (ModelState.IsValid)
            {
                // valid redirect to edit
                Repository.OfType<Question>().EnsurePersistent(question);
                Message = "Question has been created.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
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

        /// <summary>
        /// POST: /QuestionSet/Delete/{id}
        /// </summary>
        /// <remarks>
        /// Assumption:
        ///     User trying to delete the question is someone allowed to edit the question set container
        /// Description:
        ///     Delete a question from a question set.
        /// PreCondition:
        ///     Question exists
        ///     Question set that the question is associated with is not used with any item
        /// PostCondition:
        ///     Question is deleted.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Delete(int id)
        {
            //TODO: Check to make sure the user is allowed to edit the QuestionSet

            var question = Repository.OfType<Question>().GetNullableByID(id);
            if (question == null)
            {
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            var questionSetId = question.QuestionSet.Id;

            // Check to make sure the question set hasn't been used yet
            if (question.QuestionSet.Items.Count > 0)
            {
                Message = "Question cannot be added to the question set becuase it is already being used by an item.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // delete the object
            Repository.OfType<Question>().Remove(question);
            Message = "Question was deleted.";
            return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
        }
    }
}
