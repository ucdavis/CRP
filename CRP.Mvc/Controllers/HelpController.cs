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
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [AnyoneWithRoleAttribute]
    public class HelpController : ApplicationController
    {
        //
        // GET: /Help/
        /// <summary>
        /// Tested 20200409
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


    }
}
