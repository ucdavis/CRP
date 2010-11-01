using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Helpers;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [UserOnly]
    public class MapPinController : SuperController
    {
        //
        // GET: /MapPin/Details/5

        public ActionResult Details(int id, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableByID(mapPinId);
            if (mapPin == null || !item.MapPins.Contains(mapPin))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                return Redirect(Url.EditItemUrl(id, StaticValues.Tab_MapPins));
            }

            Message = "View not done";
            return Redirect(Url.EditItemUrl(id, StaticValues.Tab_MapPins));
            //return View();
        }

        //
        // GET: /MapPin/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <returns></returns>
        public ActionResult Create(int itemId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = new MapPin();
            
            return View(viewModel);
        } 

        //
        // POST: /MapPin/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPin"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int itemId, [Bind(Exclude = "Id")]MapPin mapPin)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            mapPin.IsPrimary = !Repository.OfType<MapPin>().Queryable.Where(a => a.Item == item && a.IsPrimary).Any();
            mapPin.Item = item;
            mapPin.TransferValidationMessagesTo(ModelState);
            if(ModelState.IsValid)
            {
                Repository.OfType<MapPin>().EnsurePersistent(mapPin);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                       "Map Pin");
                return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
            }
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = mapPin;

            return View(mapPin);
        }

        //
        // GET: /MapPin/Edit/5

        public ActionResult Edit(int itemId, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableByID(mapPinId);
            if (mapPin == null || !item.MapPins.Contains(mapPin))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
            }

            Message = "View not done";
            return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
            return View();
        }

        //
        // POST: /MapPin/Edit/5

        [AcceptPost]
        public ActionResult Edit(int itemId, int mapPinId, MapPin mapPin)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPinId"></param>
        /// <returns></returns>
        public ActionResult RemoveMapPin(int itemId, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            if(item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableByID(mapPinId);
            if(mapPin == null || !item.MapPins.Contains(mapPin))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
            }
            item.RemoveMapPin(mapPin);
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());
            if(mapPin.IsPrimary && item.MapPins.Count > 0)
            {
                ModelState.AddModelError("MapPin", "Can't remove the primary pin when there are still other pins.");
                Message = "Can't remove the primary pin when there are still other pins.";
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectRemoved.Replace(NotificationMessages.ObjectType, "MapPin");
            }
            if(string.IsNullOrEmpty(Message))
            {
                Message = "Unable to save item/remove map pin.";
            }

            return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
        }
    }
}