using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers.Filter;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using CRP.Mvc.Models.FinancialModels;
using CRP.Mvc.Services;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;

namespace CRP.Controllers
{
    [AdminOnly]
    public class FinancialAccountController : ApplicationController
    {
        private readonly IFinancialService _financialService;

        public FinancialAccountController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        /// <summary>
        /// Index
        /// 1
        /// Tested 2080408
        /// </summary>
        /// <returns>List of accounts</returns>
        public ActionResult Index()
        {
            var accounts = Repository.OfType<FinancialAccount>().Queryable.ToArray();
            return View(accounts);
        }


        /// <summary>
        /// Detail of the specified account.
        /// Tested 20200408
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int id)
        {
            var account = Repository.OfType<FinancialAccount>().GetNullableById(id);
            if (account == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                    nameof(FinancialAccount));
                return this.RedirectToAction(a => a.Index());
            }

            var accountValidation = await _financialService.IsAccountValidForRegistration(account);
            if (!accountValidation.IsValid)
            {
                ModelState.AddModelError(accountValidation.Field, accountValidation.Message);
                ErrorMessage = $"Invalid Account: Field: {accountValidation.Field} Error: {accountValidation.Message}";
            }
            else
            {
                Message = "Account is still valid!";
            }

            return View(account);
        }

        /// <summary>
        /// Create Financial Account.
        /// Tested 20200408
        /// </summary>
        /// <returns></returns>
        [PageTracker]
        public ActionResult Create()
        {
            return View(new FinancialAccount());
        }

        /// <summary>
        /// Creates the specified financial account.
        /// Tested 20200408 Scott, had to make the unit nullable to get this to work
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [PageTracker]
        public async Task<ActionResult> Create(FinancialAccount model)
        {
            if (!ModelState.IsValid)
            {
                Message = "Validation Failed";
                return View(model);
            }

            var account = new FinancialAccount()
            {
                Name        = model.Name,
                Description = model.Description,
                Chart       = model.Chart.SafeToUpper(),
                Account     = model.Account.SafeToUpper(),
                SubAccount  = model.SubAccount.SafeToUpper(),
                Project     = model.Project.SafeToUpper(), //Not used?
                IsActive    = true, 
                IsUserAdded   = false, 
            };


            if (!ModelState.IsValid)
            {
                Message = "Account Invalid";
                return View(model);
            }

            if (Repository.OfType<FinancialAccount>().Queryable.Any(a =>
                a.Chart == account.Chart &&
                a.Account == account.Account &&
                a.SubAccount == account.SubAccount &&
                a.Project == account.Project))
            {
                ErrorMessage = "That account already exists";
                return View(model);
            }

            Repository.OfType<FinancialAccount>().EnsurePersistent(account);
            Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                   nameof(FinancialAccount));

            return this.RedirectToAction(a => a.Details(account.Id));
            //return this.RedirectToAction(a => a.Index());
        }


        /// <summary>
        /// Edits the specified id.
        /// Tested 20200408
        /// GET
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [PageTracker]
        public ActionResult Edit(int id)
        {
            var account = Repository.OfType<FinancialAccount>().GetNullableById(id);
            if (account == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                      nameof(FinancialAccount));
                return this.RedirectToAction(a => a.Index());
            }

            return View(account);
        }


        /// <summary>
        /// Edits the specified id.
        /// POST
        /// Tested 20200408
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [PageTracker]
        public async Task<ActionResult> Edit(int id, FinancialAccount model)
        {
            var account = Repository.OfType<FinancialAccount>().GetNullableById(id);
            if (account == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                       nameof(FinancialAccount));
                return this.RedirectToAction(a => a.Index());
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            account.Name        = model.Name;
            account.Description = model.Description;
            account.Chart       = model.Chart.SafeToUpper();
            account.Account     = model.Account.SafeToUpper();
            account.SubAccount  = model.SubAccount.SafeToUpper();
            account.Project     = model.Project.SafeToUpper();
            account.IsActive    = model.IsActive;
            account.IsUserAdded   = model.IsUserAdded;


            if (!ModelState.IsValid)
            {
                Message = "Account Invalid";
                return View(model);
            }

            var warning = string.Empty;
            if (Repository.OfType<FinancialAccount>().Queryable.Any(a =>
                a.IsActive && a.Id != account.Id && 
                a.Chart == account.Chart &&
                a.Account == account.Account &&
                a.SubAccount == account.SubAccount &&
                a.Project == account.Project))
            {
                warning = "WARNING That account already exists (But we also updated this)";
            }

            Repository.OfType<FinancialAccount>().EnsurePersistent(account);
            Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                nameof(FinancialAccount));
            Message = $"{Message} {warning}";
            return this.RedirectToAction(a => a.Details(account.Id));
            //return this.RedirectToAction(a => a.Index());

        }
    }
}
