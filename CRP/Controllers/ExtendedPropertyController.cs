using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [Authorize(Roles="Admin")]
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
            var itemType = Repository.OfType<ItemType>().GetNullableByID(itemTypeId);

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
        ///     Item Type does not have any existing items associated with this type
        ///     Item Type does not already have extended property with same name
        /// PostCondition:
        ///     Extended property created and associated
        ///     User is redirected back to the Edit Item Page
        /// </remarks>
        /// <param name="itemTypeId"></param>
        /// <param name="extendedProperty"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int itemTypeId, [Bind(Exclude="Id")] ExtendedProperty extendedProperty)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(itemTypeId);

            if (itemType != null)
            {
                extendedProperty.ItemType = itemType;

                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, extendedProperty.ValidationResults());

                if (ModelState.IsValid)
                {
                    Repository.OfType<ExtendedProperty>().EnsurePersistent(extendedProperty);
                    Message = "Extended property was added.";
                    return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
                }
                else
                {
                    return View(ExtendedPropertyViewModel.Create(Repository, itemType));
                }
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        [AcceptPost]
        public ActionResult Delete(int id)
        {
            var extendedProperty = Repository.OfType<ExtendedProperty>().GetNullableByID(id);

            if(extendedProperty == null)
            {
                return this.RedirectToAction<ApplicationManagementController>(a => a.ListItemTypes());
            }
            
            var itemTypeId = extendedProperty.ItemType.Id;

            Repository.OfType<ExtendedProperty>().Remove(extendedProperty);
            Message = "Extended property has been removed.";
            return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(itemTypeId));
        }
    }
}
