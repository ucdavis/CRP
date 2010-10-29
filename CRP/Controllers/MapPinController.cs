using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CRP.Controllers.Filter;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    [UserOnly]
    public class MapPinController : SuperController
    {
        //
        // GET: /MapPin/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /MapPin/Create

        public ActionResult Create(int id)
        {
            return View();
        } 

        //
        // POST: /MapPin/Create

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
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MapPin/Edit/5

        [AcceptPost]
        public ActionResult Edit(int id, MapPin mapPin)
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

        [AcceptPost]
        public ActionResult Remove(int id, int mapPinId)
        {
            //TODO :Check that there is editor access
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if(item == null)
            {
                //TODO: Redirect with message
            }
            var mapPin = Repository.OfType<MapPin>().GetNullableByID(mapPinId);
            if(mapPin == null)
            {
                //TODO: Redirect with message
            }
            if(!item.MapPins.Contains(mapPin))
            {
                //TODO: Redirect with messgae
            }
            item.RemoveMapPin(mapPin);
            //TODO: Return back to map tab with message
            return View();
        }
    }
}
