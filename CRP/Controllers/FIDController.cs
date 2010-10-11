using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CRP.Controllers.Filter;
using UCDArch.Web.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Web.Controller;
using MvcContrib.Attributes;
using UCDArch.Web.Helpers;
using MvcContrib;

namespace CRP.Controllers
{
    [AdminOnly]
    public class FIDController : SuperController
    {

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>Queryable of TouchnetFID</returns>
        public ActionResult Index()
        {
            return View(Repository.OfType<TouchnetFID>().Queryable);
        }


        /// <summary>
        /// Detail of the specified FID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var touchnetFID = Repository.OfType<TouchnetFID>().GetNullableByID(id);
            if (touchnetFID == null)
            {
                Message = "FID Not Found";
                return this.RedirectToAction(a => a.Index());
            }

            return View(touchnetFID);
        }

        /// <summary>
        /// Create New FID Value.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new TouchnetFID());
        }

        /// <summary>
        /// Creates the specified touchnet FID.
        /// </summary>
        /// <param name="touchnetFID">The touchnet FID.</param>
        /// <returns></returns>
        [AdminOnly]
        [AcceptPost]
        public ActionResult Create(TouchnetFID touchnetFID)
        {
            var fid = new TouchnetFID();
            fid.FID = touchnetFID.FID.Trim();
            fid.Description = touchnetFID.Description;

            fid.TransferValidationMessagesTo(ModelState);

            if(Repository.OfType<TouchnetFID>().Queryable.Where(a => a.FID == fid.FID).Any())
            {
                ModelState.AddModelError("TouchnetFID.FID", "FID value already used");
            }

            if (Repository.OfType<TouchnetFID>().Queryable.Where(a => a.Description == fid.Description).Any())
            {
                ModelState.AddModelError("TouchnetFID.Description", "Description value already used");
            }

            if(ModelState.IsValid)
            {
                Repository.OfType<TouchnetFID>().EnsurePersistent(fid);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                       "Touchnet FID");
                return this.RedirectToAction<FIDController>(a => a.Index());
            }

            return View(fid);
        }


        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var touchnetFID = Repository.OfType<TouchnetFID>().GetNullableByID(id);
            if (touchnetFID == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                      "Touchnet FID");
                return this.RedirectToAction(a => a.Index());
            }
            return View(touchnetFID);
        }

        //
        // POST: /FID/Edit/5

        [AcceptPost]
        public ActionResult Edit(int id, TouchnetFID touchnetFID)
        {
            var fid = Repository.OfType<TouchnetFID>().GetNullableByID(id);
            if (fid == null)
            {
                Message = "FID Not Found";
                return this.RedirectToAction(a => a.Index());
            }
            fid.FID = touchnetFID.FID.Trim();
            fid.Description = touchnetFID.Description;

            fid.TransferValidationMessagesTo(ModelState);

            if (Repository.OfType<TouchnetFID>().Queryable.Where(a => a.FID == fid.FID && a.Id != id).Any())
            {
                ModelState.AddModelError("TouchnetFID.FID", "FID value already used");
            }

            if (Repository.OfType<TouchnetFID>().Queryable.Where(a => a.Description == fid.Description && a.Id != id).Any())
            {
                ModelState.AddModelError("TouchnetFID.Description", "Description value already used");
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<TouchnetFID>().EnsurePersistent(fid);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                       "Touchnet FID");
                return this.RedirectToAction<FIDController>(a => a.Index());
            }

            return View(fid);
        }
    }
}
