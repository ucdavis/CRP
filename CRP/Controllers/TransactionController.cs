using System;
using System.Web.Mvc;
using CRP.Core.Domain;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class TransactionController : SuperController
    {
        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Transaction/Register/{id}
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public ActionResult Register(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null)
            {
                return this.RedirectToAction<ItemController>(a => a.List());
            }

            return View(item);
        }
    }
}
