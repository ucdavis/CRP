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
    }
}
