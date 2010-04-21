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
        ///     Item type wiht same name doesn't already exist
        /// PostCondition:
        ///     Item is created
        ///     Extended properties passed in are saved
        /// </remarks>
        /// <param name="itemType"></param>
        /// <param name="ExtendedPropertyName">List of extended properties names (indexes should match question type)</param>
        /// <param name="QuestionType">Question types for each extended property (indexes should match extended property name)</param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult CreateItemType([Bind(Exclude="Id, IsActive")]ItemType itemType, string[] ExtendedPropertyName, string[] QuestionType)
        {
            // try to add in the extended properties
            for (var i = 0; i < ExtendedPropertyName.Count(); i++ )
            {
                // make sure the items are valid
                var propName = ExtendedPropertyName[i];
                var questionTypeId = QuestionType[i];
                

                // make sure none of them are blank
                if (!string.IsNullOrEmpty(propName) && !string.IsNullOrEmpty(questionTypeId))
                {
                    // make sure the question type is a valid number
                    var questionType = Repository.OfType<QuestionType>().GetNullableByID(Convert.ToInt32(questionTypeId));

                    // once we make sure the question type isn't null, go ahead and add it
                    if (questionType != null)
                    {
                        itemType.AddExtendedProperty(new ExtendedProperty()
                            {
                                Name = ExtendedPropertyName[i],
                                QuestionType = questionType
                            });        
                    }
                }
            }

            // run the validation stuff now
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
    }
}
