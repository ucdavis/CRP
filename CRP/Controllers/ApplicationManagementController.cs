using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;
using CRP.App_GlobalResources;

namespace CRP.Controllers
{
    [Authorize(Roles="Admin")]
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
            return View(Repository.OfType<ItemType>().Queryable);
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
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Item Type");
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

        /// <summary>
        /// POST: /ApplicationManagement/EditItemType
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Saves the name and the is active flag
        /// PreCondition:
        ///     The item type exists
        /// PostCondition:
        ///     The item is updated
        /// </remarks>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult EditItemType([Bind(Exclude="Id")]ItemType itemType)
        {
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemType>().EnsurePersistent(itemType);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Item Type");
                return View(itemType);
            }
            else
            {
                return View(itemType);
            }
        }

        [AcceptPost]
        public ActionResult ToggleActive(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

            if (itemType == null)
            {
                return this.RedirectToAction(a => a.ListItemTypes());
            }

            itemType.IsActive = !itemType.IsActive;

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, itemType.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemType>().EnsurePersistent(itemType);

                Message = itemType.IsActive
                              ? NotificationMessages.STR_Activated.Replace(NotificationMessages.ObjectType, "Item Type")
                              :
                                  NotificationMessages.STR_Deactivated.Replace(NotificationMessages.ObjectType,
                                                                               "Item Type");
            }

            return this.RedirectToAction(a => a.ListItemTypes());
        }

        #endregion

        #region Display Profiles

        #endregion
    }
}
