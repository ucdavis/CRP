using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.Helpers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.Helpers;
using CRP.Mvc.Controllers.ViewModels;
using CRP.Mvc.Controllers.ViewModels.ItemManagement;
using CRP.Mvc.Services;
using CRP.Services;
using Microsoft.Azure;
using MvcContrib;
using Serilog;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class CfTestController : ApplicationController
    {
        private readonly ICopyItemService _copyItemService;
        private readonly IFinancialService _financialService;
        private readonly string CfKey;


        public CfTestController(ICopyItemService copyItemService, IFinancialService financialService)
        {
            _copyItemService = copyItemService;
            _financialService = financialService;
            CfKey = CloudConfigurationManager.GetSetting("CfKey");
        }



        /// <summary>
        /// GET: /ItemManagement/Details/{id}
        /// Tested 20200506
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id, string key)
        {
            if(key == null) {
                Log.Error("Key is null for Item Details with id {Id}", id);
                return RedirectToAction("Index", "Home");
            }
            if(string.IsNullOrWhiteSpace(CfKey) || !CfKey.Equals(key)) {
                Log.Error("Key {Key} does not match expected key for Item Details with id {Id}", key, id);
                return RedirectToAction("Index", "Home");
            }

            if(id != 877) {
                Log.Error("Item Details called with id {Id} but only id 877 is allowed", id);
                return RedirectToAction("Index", "Home");
            }

            var item = Repository.OfType<Item>().GetNullableById(id);
            if(item == null) {
                Log.Error("Item with id {Id} not found", id);
                return RedirectToAction("Index", "Home");
            }

            var viewModel = UserItemDetailViewModel.Create(Repository, item);

            return View(viewModel);
        }


    }
}
