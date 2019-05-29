using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [Authorize]
    public class QuestionController : ApplicationController
    {

        /// <summary>
        /// GET: /Question/Create/{questionSetId}
        /// </summary>
        /// <param name="questionSetId">Question set Id</param>
        /// <returns></returns>
        public ActionResult Create(int questionSetId)
        {
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(questionSetId);
            if (questionSet == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, questionSet))
            {
                // go back to the question set edit view
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            // Check to make sure the question set hasn't been used yet
            // If it is reusable, and it has already been used, we can add a question to it.
            if ((questionSet.SystemReusable || questionSet.CollegeReusable || questionSet.UserReusable) && questionSet.Items.Count > 0)
            {
                Message = "Question cannot be added to the question set because it is already being used by an item.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // check to make sure it isn't the system's default contact information set
            if (questionSet.Name == StaticValues.QuestionSet_ContactInformation && questionSet.SystemReusable)
            {
                Message = "This is a system default question set and cannot be modified.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // lookups
            ViewBag.QuestionTypes = Repository.OfType<QuestionType>().GetAll().ToList();
            ViewBag.Validators = Repository.OfType<Validator>().GetAll().ToList();

            var viewModel = new QuestionViewModel();
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
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int questionSetId, QuestionViewModel model)
        {
            ModelState.Clear();

            // look for question set
            var questionSet = Repository.OfType<QuestionSet>().GetNullableById(questionSetId);
            if (questionSet == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, questionSet))
            {
                // go back to the question set edit view
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            // Check to make sure the question set hasn't been used yet
            // If it is reusable, and it has already been used, we can add a question to it.
            if ((questionSet.SystemReusable || questionSet.CollegeReusable || questionSet.UserReusable) && questionSet.Items.Count > 0)
            {
                Message = "Question cannot be added to the question set because it is already being used by an item.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // check to make sure it isn't the system's default contact information set
            if (questionSet.Name == StaticValues.QuestionSet_ContactInformation && questionSet.SystemReusable)
            {
                Message = "This is a system default question set and cannot be modified.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // lookups
            ViewBag.QuestionTypes = Repository.OfType<QuestionType>().GetAll().ToList();
            ViewBag.Validators = Repository.OfType<Validator>().GetAll().ToList();

            // build new question
            var question = new Question();
            question.Name = model.Name;

            // find question type
            var questionType = Repository.OfType<QuestionType>().GetNullableById(model.QuestionTypeId);
            if (questionType == null)
            {
                ModelState.AddModelError("QuestionType", "Question Type not found.");
                return View(model);
            }
            question.QuestionType = questionType;

            // process the options
            if (questionType.HasOptions)
            {
                foreach (var s in model.Options)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var option = new QuestionOption(s);
                        question.AddOption(option);
                    }
                }
            }

            // add the validators
            foreach (var validatorId in model.Validators)
            {
                var validator = Repository.OfType<Validator>().GetNullableById(validatorId);
                if (validator != null)
                {
                    question.Validators.Add(validator);
                }
            }

            // add the question to the set
            questionSet.AddQuestion(question);

            var validatorsSelected = question.Validators.Count(validator => validator.Class.ToLower().Trim() != "required");

            // Validator and Question type validation:
            switch (questionType.Name)
            {
                case "Text Box":
                    //All possible, but only a combination of required and others
                    if (validatorsSelected > 1)
                    {
                        ModelState.AddModelError("Validators", "Cannot have Email, Url, Date, or Phone Number validators selected together.");
                    }
                    break;

                case "Boolean":
                    if (question.Validators.Count > 0) //Count of all validators
                    {
                        ModelState.AddModelError("Validators", "Boolean Question Type should not have validators.");
                    }
                    break;

                case "Radio Buttons":
                case "Checkbox List":
                case "Drop Down":
                case "Text Area":
                    if (validatorsSelected > 0) //count of all validators excluding required
                    {
                        ModelState.AddModelError("Validators", string.Format("The only validator allowed for a Question Type of {0} is Required.", question.QuestionType.Name));
                    }
                    break;

                case "Date":
                    foreach (var validator in question.Validators)
                    {
                        if (validator.Class.ToLower().Trim() != "required" && validator.Class.ToLower().Trim() != "date")
                        {
                            ModelState.AddModelError("Validators", string.Format("{0} is not a valid validator for a Question Type of {1}", validator.Name, question.QuestionType.Name));
                        }
                    }
                    break;

                case "No Answer":
                    foreach (var validator in question.Validators)
                    {
                        ModelState.AddModelError("Validators", string.Format("{0} is not a valid validator for a Question Type of {1}", validator.Name, question.QuestionType.Name));
                    }
                    break;

                default:
                    //No checks
                    break;
            }

            // check to make sure there are options if needed
            if (questionType.HasOptions && question.Options.Count <= 0)
            {
                ModelState.AddModelError("Options", "The question type requires at least one option.");
            }

            // check any other complex issues
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, question.ValidationResults());

            // not valid, go back
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // valid redirect to edit
            Repository.OfType<Question>().EnsurePersistent(question);
            Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question");
            return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
        }

        /// <summary>
        /// POST: /Question/Delete/{id}
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var question = Repository.OfType<Question>().GetNullableById(id);

            if (question == null || !Access.HasQuestionSetAccess(Repository, CurrentUser, question.QuestionSet))
            {
                return this.RedirectToAction<QuestionSetController>(a => a.List());
            }

            var questionSetId = question.QuestionSet.Id;

            // Check to make sure the question set hasn't been used yet
            if (question.QuestionSet.Items.Count > 0 && 
                (question.QuestionSet.CollegeReusable || question.QuestionSet.SystemReusable || question.QuestionSet.UserReusable))
            {
                Message = "Question cannot be deleted from the question set because it is already being used by an item.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // check to make sure it isn't the system's default contact information set
            if (question.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation && question.QuestionSet.SystemReusable)
            {
                //ModelState.AddModelError("Question Set", "This is a sytem default question set and cannot be modified.");
                Message = "Question cannot be deleted from the question set because it is a system default.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            //Check to make sure there isn't an answer
            if(Repository.OfType<TransactionAnswer>().Queryable.Where(a => a.Question == question).Any()
                || Repository.OfType<QuantityAnswer>().Queryable.Where(a => a.Question == question).Any())
            {
                Message =
                    "Question cannot be deleted from the question set because it has an answer associated with it.";
                return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
            }

            // delete the object
            Repository.OfType<Question>().Remove(question);
            Message = NotificationMessages.STR_ObjectRemoved.Replace(NotificationMessages.ObjectType, "Question");
            return this.RedirectToAction<QuestionSetController>(a => a.Edit(questionSetId));
        }
    }
}
