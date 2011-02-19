using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
//using CRP.App_GlobalResources;
using CRP.Core.Resources;

namespace CRP.Controllers
{
    [AdminOnlyAttribute]
    public class ExtendedPropertyController : SuperController
    {
        //
        // GET: /ExtendedProperty/
        public ActionResult Index()
        {
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// GET: /ExtendedProperty/Create/{itemTypeId}
        /// </summary>
        /// <param name="itemTypeId">The id of the item type</param>
        /// <returns></returns>
        public ActionResult Create(int itemTypeId)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId);

            if (itemType != null)
            {
                return View(ExtendedPropertyViewModel.Create(Repository, itemType));
            }
            else
            {
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
        }

        /// <summary>
        /// POST: /ExtendedProperty/Create/{itemTypeId}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a new extended property and associates it to the item type
        /// PreCondition:
        ///     Item Type does not already have extended property with same name
        /// PostCondition:
        ///     Extended property created and associated
        ///     User is redirected back to the Edit Item Page
        /// </remarks>
        /// <param name="itemTypeId"></param>
        /// <param name="extendedProperty"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int itemTypeId, [Bind(Exclude="Id")] ExtendedProperty extendedProperty)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableById(itemTypeId);

            if (itemType != null)
            {
                

                // check to make sure it doesn't already have an extended property with the same name already
                foreach(var ep in itemType.ExtendedProperties)
                {
                    if (ep.Name == extendedProperty.Name)
                    {
                        ModelState.AddModelError("Name", "Item type already has extended property with the same name.");
                    }
                }

                itemType.AddExtendedProperty(extendedProperty); //Moved to be after the name check above

                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, extendedProperty.ValidationResults());

                if (ModelState.IsValid)
                {
                    Repository.OfType<ItemType>().EnsurePersistent(itemType);
                    Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Extended property");
                    return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
                }
                else
                {
                    return View(ExtendedPropertyViewModel.Create(Repository, itemType));
                }
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// POST: /ExtendedProperty/Delete/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Delete the extended property
        /// Assumption:
        ///     No item exists with this item type
        /// PreCondition:
        ///     Item Type exists
        ///     Extended type exists and is associated with above mentioned item type
        /// PostCondition:
        ///     Extended property is deleted
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // get the extended property itself
            var extendedProperty = Repository.OfType<ExtendedProperty>().GetNullableById(id);

            // check to make sure it's valid
            if(extendedProperty == null)
            {
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }

            // set aside the item type id
            var itemTypeId = extendedProperty.ItemType.Id;

            // check to see if there are any items using the extended property
            if (!Repository.OfType<ExtendedPropertyAnswer>().Queryable.Where(a => a.ExtendedProperty == extendedProperty).Any())
            {
                // no item is using the extended property, allow the deletion
                Repository.OfType<ExtendedProperty>().Remove(extendedProperty);
                Message = NotificationMessages.STR_ObjectRemoved.Replace(NotificationMessages.ObjectType,
                                                                         "Extended property");
                return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));   
            }

            Message = "Extended property cannot be deleted, because there is already an item associated with the item type.";
            return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
        }
    }
}
