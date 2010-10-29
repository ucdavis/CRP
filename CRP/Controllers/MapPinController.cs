using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using MvcContrib;
using MvcContrib.Attributes;
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
            return View();
        }

        //
        // GET: /MapPin/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            Message = "Create View not done";
            return Redirect(Url.EditItemUrl(id, StaticValues.Tab_MapPins));
        } 

        //
        // POST: /MapPin/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <param name="mapPin"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(int id, MapPin mapPin)
        {
            //try
            //{
            //    // TODO: Add insert logic here

            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
            return View();
        }

        //
        // GET: /MapPin/Edit/5
 
        public ActionResult Edit(int id, int mapPinId)
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
            return View();
        }

        //
        // POST: /MapPin/Edit/5

        [AcceptPost]
        public ActionResult Edit(int id, int mapPinId, MapPin mapPin)
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
        /// <param name="id">item id</param>
        /// <param name="mapPinId">map pin id</param>
        /// <returns></returns>
        public ActionResult RemoveMapPin(int id, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
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
                return Redirect(Url.EditItemUrl(id, StaticValues.Tab_MapPins));
            }
            item.RemoveMapPin(mapPin);
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectRemoved.Replace(NotificationMessages.ObjectType, "MapPin");
            }

            return Redirect(Url.EditItemUrl(id, StaticValues.Tab_MapPins));
        }
    }
}
