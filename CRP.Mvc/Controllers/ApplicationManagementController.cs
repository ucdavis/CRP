using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;
//using CRP.App_GlobalResources;

namespace CRP.Controllers
{
    [AdminOnlyAttribute]
    public class ApplicationManagementController : ApplicationController
    {
        //
        // GET: /ApplicationManagement/
        /// <summary>
        /// Tested 20200415
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region Item Types

        /// <summary>
        /// GET: /ApplicationManagement/ListItemTypes
        /// Tested 20200415 (Fixed VIew)
        /// </summary>
        /// <returns></returns>
        public ActionResult ListItemTypes()
        {
            return View(Repository.OfType<ItemType>().Queryable.ToArray());
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
        [HttpPost]
        public ActionResult CreateItemType(ItemType itemType, ExtendedProperty[] extendedProperties)
        {
            ModelState.Clear();
            //foreach (var ep in extendedProperties)
            //{
            //    ep.ItemType = itemType;

            //    if (ep.IsValid())
            //    {
            //        itemType.AddExtendedProperty(ep);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("ExtendedProperty", "At least one extended property is not valid.");
            //    }
            //}
            if (extendedProperties != null)
            {
                var duplicateCheck = new List<string>();
                foreach (var list in extendedProperties)
                {
                    if (duplicateCheck.Contains(list.Name))
                    {
                        ModelState.AddModelError("ExtendedProperty",
                                                 "Duplicate names not allowed. Extended property \"" + list.Name +
                                                 "\" already exists.");
                        break;
                    }
                    duplicateCheck.Add(list.Name);
                }
            }

            //Validation is done in the domain
            if (extendedProperties != null)
            {
                foreach (var ep in extendedProperties)
                {
                    ep.ItemType = itemType;
                    itemType.AddExtendedProperty(ep);
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
            var itemType = Repository.OfType<ItemType>().GetNullableById(id);

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
        /// <param name="id"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditItemType(int id, [Bind(Exclude="Id")]ItemType itemType)
        {
            ModelState.Clear();
            var it = Repository.OfType<ItemType>().GetNullableById(id);
            if (it == null)
            {
                return this.RedirectToAction(a => a.ListItemTypes());
            }

            it.Name = itemType.Name;
            it.IsActive = itemType.IsActive;

            //Done: Review. I think this needs to pass in the id, get it, 
            //copy over the fields which are edited, then check and persist that.
            //As the name should not be duplicate, that check would need to be added.
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, it.ValidationResults());

            if (Repository.OfType<ItemType>().Queryable.Where(a => a.Name == it.Name && a.Id != id).Any())
            {
                ModelState.AddModelError("Name", "The new name already exists with a different item type.");
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemType>().EnsurePersistent(it);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Item Type");
                return View(it); //Display the updated itemType, not the passed itemType
            }
            else
            {
                return View(it); //Display the updated itemType, not the passed itemType
            }
        }

        [HttpPost]
        public ActionResult ToggleActive(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableById(id);

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
    }
}
