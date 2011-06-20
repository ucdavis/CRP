using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Services;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Helpers;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [AnyoneWithRole]
    public class MapPinController : ApplicationController
    {
        private readonly IAccessControlService _accessControlService;
        public MapPinController(IAccessControlService AccessControlService)
        {
            _accessControlService = AccessControlService;
        }

        /// <summary>
        /// GET: /MapPin/Create
        /// #1
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <returns></returns>
        public ActionResult Create(int itemId)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null || !_accessControlService.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = new MapPin();
            
            return View(viewModel);
        } 

        /// <summary>
        /// POST: /MapPin/Create
        /// #2
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int itemId, [Bind(Exclude = "Id")]MapPin mapPin)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null || !_accessControlService.HasItemAccess(CurrentUser, item))
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
                //could replace with EnsurePersistent(item), but this is working fine.
                Repository.OfType<MapPin>().EnsurePersistent(mapPin);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                       "Map Pin");
                //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
                return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
            }
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = mapPin;

            return View(viewModel);
        }

        /// <summary>
        /// GET: /MapPin/Edit/5
        /// #3
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPinId"></param>
        /// <returns></returns>
        public ActionResult Edit(int itemId, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null || !_accessControlService.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableById(mapPinId);
            if (mapPin == null || !item.MapPins.Contains(mapPin))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
                return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
            }
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = mapPin;
            return View(viewModel);
        }

        /// <summary>
        /// POST: /MapPin/Edit/5
        /// #4
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPinId"></param>
        /// <param name="mapPin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int itemId, int mapPinId, MapPin mapPin)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null || !_accessControlService.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPinToUpdate = Repository.OfType<MapPin>().GetNullableById(mapPinId);
            if (mapPinToUpdate == null || !item.MapPins.Contains(mapPinToUpdate))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
                return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
            }

            mapPinToUpdate.Latitude = mapPin.Latitude;
            mapPinToUpdate.Longitude = mapPin.Longitude;
            mapPinToUpdate.Title = mapPin.Title;
            mapPinToUpdate.Description = mapPin.Description;
            //mapPinToUpdate.Item = item;

            mapPinToUpdate.TransferValidationMessagesTo(ModelState);
            item.TransferValidationMessagesTo(ModelState);

            if(ModelState.IsValid)
            {
                //could replace with EnsurePersistent(item), but this is working fine.
                Repository.OfType<MapPin>().EnsurePersistent(mapPinToUpdate);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                                                                       "Map Pin");
                //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
                return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
            }

            Message = "Unable to save Map Pin changes.";
            var viewModel = MapPinViewModel.Create(Repository, item);
            viewModel.MapPin = mapPinToUpdate;
            return View(viewModel);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mapPinId"></param>
        /// <returns></returns>
        public ActionResult RemoveMapPin(int itemId, int mapPinId)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null || !_accessControlService.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableById(mapPinId);
            if(mapPin == null || !item.MapPins.Contains(mapPin))
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "MapPin");
                //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
                return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
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

            //return Redirect(Url.EditItemUrl(itemId, StaticValues.Tab_MapPins));
            return this.RedirectToAction<ItemManagementController>(a => a.Map(item.Id));
        }
    }
}
