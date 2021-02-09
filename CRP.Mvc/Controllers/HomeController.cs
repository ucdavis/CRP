using System;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using UCDArch.Web.Attributes;
using MvcContrib;

namespace CRP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        /// <summary>
        /// Tested 20200408
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //return View();

            var viewModel = BrowseItemsViewModel.Create(Repository);
            return View(viewModel);
        }

        /// <summary>
        /// Tested 20200408
        /// </summary>
        /// <returns></returns>
        [AnyoneWithRoleAttribute]
        public ActionResult AdminHome()
        {
            return View();
        }

        /// <summary>
        /// Tested 20200408
        /// Don't think we have a link to this page
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Tested 202020408
        /// </summary>
        /// <returns></returns>
        [AdminOnly]
        public ActionResult TestException()
        {
            throw new ApplicationException("Exception successfully thrown.");
        }

        /// <summary>
        /// Tested 20200408
        /// Don't think ServiceMessages is used anymore
        /// </summary>
        /// <returns></returns>
        [AdminOnly]
        public ActionResult ResetCache()
        {
            HttpContext.Cache.Remove("ServiceMessages");

            return this.RedirectToAction(a => a.Index());
        }
    }
}
