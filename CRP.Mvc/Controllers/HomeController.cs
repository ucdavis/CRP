using System;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
//using Elmah;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;
using MvcContrib;

namespace CRP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            //return View();

            var viewModel = BrowseItemsViewModel.Create(Repository);
            return View(viewModel);
        }

        public ActionResult IndexNew()
        {
            //return View();

            var viewModel = BrowseItemsViewModel.Create(Repository);
            return View(viewModel);
        }

        [AnyoneWithRoleAttribute]
        public ActionResult AdminHome()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [AdminOnly]
        public ActionResult TestException()
        {
            throw new ApplicationException("Exception successfully thrown.");
        }

        [AdminOnly]
        public ActionResult ResetCache()
        {
            HttpContext.Cache.Remove("ServiceMessages");

            return this.RedirectToAction(a => a.Index());
        }
    }
}
