using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;

namespace CRP.Controllers
{
    [AdminOnly]
    public class FIDController : SuperController
    {

        /// <summary>
        /// Index
        /// 1
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
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                       "Touchnet FID");
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
        [AcceptPost]
        public ActionResult Create(TouchnetFID touchnetFID)
        {
            var fid = new TouchnetFID();
            fid.FID = touchnetFID.FID.Trim();
            fid.Description = touchnetFID.Description.Trim();

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
                return this.RedirectToAction(a => a.Index());
            }

            return View(fid);
        }


        /// <summary>
        /// Edits the specified id.
        /// GET
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


        /// <summary>
        /// Edits the specified id.
        /// POST
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="touchnetFID">The touchnet FID.</param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Edit(int id, TouchnetFID touchnetFID)
        {
            var fid = Repository.OfType<TouchnetFID>().GetNullableByID(id);
            if (fid == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                       "Touchnet FID");
                return this.RedirectToAction(a => a.Index());
            }
            fid.FID = touchnetFID.FID.Trim();
            fid.Description = touchnetFID.Description.Trim();

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
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                                                                       "Touchnet FID");
                return this.RedirectToAction(a => a.Index());
            }

            return View(fid);
        }
    }
}
