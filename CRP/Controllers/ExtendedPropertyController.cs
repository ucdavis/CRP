using System;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Data.NHibernate;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class ExtendedPropertyController : SuperController
    {
        //
        // GET: /ExtendedProperty/
        public ActionResult Index()
        {
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// GET: /ExtendedProperty/Create/{id}
        /// </summary>
        /// <param name="id">The id of the item type</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

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
        /// POST: /ExtendedProperty/Create/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="extendedProperty"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int id, [Bind(Exclude="Id")] ExtendedProperty extendedProperty)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

            if (itemType != null)
            {
                extendedProperty.ItemType = itemType;

                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, extendedProperty.ValidationResults());

                if (ModelState.IsValid)
                {
                    Repository.OfType<ExtendedProperty>().EnsurePersistent(extendedProperty);
                    Message = "Extended property was added.";
                    return this.RedirectToAction<ApplicationManagementController>(a => a.EditItemType(id));
                }
                else
                {
                    return View(ExtendedPropertyViewModel.Create(Repository, itemType));
                }
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }
    }
}
