using CRP.Controllers.Filter;
using CRP.Controllers.Helpers.Filter;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.ViewModels.Financial;
using CRP.Mvc.Services;
using Microsoft.Azure;
using MvcContrib;
using NHibernate.Linq.Functions;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRP.Controllers
{
    [AdminOnly]
    public class FinancialAccountController : ApplicationController
    {
        private readonly IFinancialService _financialService;
        private IAggieEnterpriseService aggieEnterpriseService;
        private readonly bool RequireKfs ;
        private readonly bool UseCoa;

        public FinancialAccountController(IFinancialService financialService, IAggieEnterpriseService aggieEnterpriseService)
        {
            _financialService = financialService;
            this.aggieEnterpriseService = aggieEnterpriseService;
            RequireKfs = CloudConfigurationManager.GetSetting("RequireKfs").SafeToUpper() == "TRUE";
            UseCoa = CloudConfigurationManager.GetSetting("UseCoa").SafeToUpper() == "TRUE";
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

            if (UseCoa)
            {
                //This checks Coa or KFS depending on config settings.
                var accountValidation = await _financialService.IsAccountValidForRegistration(account.FinancialSegmentString);
                if (!accountValidation.IsValid)
                {
                    ModelState.AddModelError("FinancialSegmentString", "Is not valid.");
                    ErrorMessage = $"Financial Segment String in not valid: {accountValidation.Message}";
                }
                else
                {
                    if (accountValidation.IsWarning)
                    {
                        Message = $"Warning: {accountValidation.Message}";
                    }
                    else
                    {
                        Message = "COA Financial Segment String is still valid!";
                    }
                }
            }
            else
            {               
                var accountValidation = await _financialService.IsAccountValidForRegistration(account);
                if (!accountValidation.IsValid)
                {
                    ModelState.AddModelError(accountValidation.Field, accountValidation.Message);
                    ErrorMessage = $"Invalid KFS Account: Field: {accountValidation.Field} Error: {accountValidation.Message}";
                }
                else
                {
                    Message = "Account is still valid!";
                }
                //If it has both, check both
                if (!string.IsNullOrWhiteSpace(account.FinancialSegmentString))
                {
                    var aEaccountValidation = await _financialService.IsCoaValidForRegistration(account.FinancialSegmentString);
                    if (!aEaccountValidation.IsValid)
                    {
                        ModelState.AddModelError("FinancialSegmentString", aEaccountValidation.Message);
                        ErrorMessage = $"{ErrorMessage} - Financial Segment String in not valid: {aEaccountValidation.Message}";
                    }
                    if(aEaccountValidation.IsWarning)
                    {
                        Message = $"Coa Warning: {aEaccountValidation.Message}";
                    }
                }
            }


            var model = new FinancialAccountDetailsViewModel();
            model.FinancialAccount = account;
            model.RelatedItems = Repository.OfType<Item>().Queryable.Where(a => a.FinancialAccount.Id == account.Id).Select(a => new ItemModel { Id = a.Id, Name = a.Name, Created = a.DateCreated.ToPacificTime()}).ToList();

            if (UseCoa)
            {
                if (!string.IsNullOrWhiteSpace(account.FinancialSegmentString))
                {
                    model.Duplicates = Repository.OfType<FinancialAccount>().Queryable.Where(a => a.Id != account.Id && a.FinancialSegmentString == account.FinancialSegmentString).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(account.Account))
                {
                    model.Duplicates = Repository.OfType<FinancialAccount>().Queryable.Where(a => a.Id != account.Id && a.Chart == account.Chart && a.Account == account.Account).ToList();
                }                
            }
            return View(model);
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
            model.FinancialSegmentString = model.FinancialSegmentString.SafeToUpper()?.Trim();
            if (UseCoa)
            {
                if (string.IsNullOrWhiteSpace(model.FinancialSegmentString))
                {
                    ModelState.AddModelError("FinancialSegmentString", "Financial Segment String is required");
                }
                var accountValidation = await _financialService.IsAccountValidForRegistration(model.FinancialSegmentString);
                if (!accountValidation.IsValid)
                {
                    ModelState.AddModelError("FinancialSegmentString", accountValidation.Message);
                }
                else
                {
                    if (accountValidation.IsWarning)
                    {
                        Message = $"Warning: COA may not be valid for use with Registration: {accountValidation.Message}";
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Chart))
                {
                    ModelState.AddModelError("Chart", "Chart is currently required");
                }
                if (string.IsNullOrWhiteSpace(model.Account))
                {
                    ModelState.AddModelError("Account", "Account is currently required");
                }

                if (ModelState.IsValid)
                {
                    //Ok, we have values so validate them
                    var accountValidation = await _financialService.IsAccountValidForRegistration(model);
                    if(!accountValidation.IsValid)
                    {
                        ModelState.AddModelError(accountValidation.Field, accountValidation.Message);
                        ErrorMessage = $"Invalid KFS Account: Field: {accountValidation.Field} Error: {accountValidation.Message}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.FinancialSegmentString))
                {
                    var CoaValidation = await aggieEnterpriseService.ValidateAccount(model.FinancialSegmentString); //Replace with call into financial service? DOn't worry as this code will be removed after the go live date?
                    if (!CoaValidation.IsValid)
                    {
                        ModelState.AddModelError("FinancialSegmentString", CoaValidation.Message);
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                Message = "Validation Failed";
                return View(model);
            }

            var account = new FinancialAccount()
            {
                Name                   = model.Name,
                Description            = model.Description,
                Chart                  = model.Chart.SafeToUpper(),
                Account                = model.Account.SafeToUpper(),
                SubAccount             = model.SubAccount.SafeToUpper(),
                Project                = model.Project.SafeToUpper(), //Not used
                FinancialSegmentString = model.FinancialSegmentString.SafeToUpper()?.Trim(),
                IsActive               = true, 
                IsUserAdded            = false, 
            };


            if (!ModelState.IsValid)
            {
                Message = "Account Invalid";
                return View(model);
            }

            if (UseCoa)
            {
                if (Repository.OfType<FinancialAccount>().Queryable.Any(a => a.FinancialSegmentString == account.FinancialSegmentString))
                {
                    ErrorMessage = "That Financial Segment String already exists";
                    return View(model);
                }
            }
            else
            {
                if (Repository.OfType<FinancialAccount>().Queryable.Any(a =>
                    a.Chart == account.Chart &&
                    a.Account == account.Account &&
                    a.SubAccount == account.SubAccount &&
                    a.Project == account.Project))
                {
                    ErrorMessage = "That account already exists";
                    return View(model);
                }
                //Ok, we want/need to allow dups because the existing accounts may map to more than one.
                if (!string.IsNullOrWhiteSpace(account.FinancialSegmentString))
                {
                    if (Repository.OfType<FinancialAccount>().Queryable.Any(a => a.FinancialSegmentString == account.FinancialSegmentString))
                    {
                        Message = "Warning! That Financial Segment String already exists";
                    }
                }
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
            model.FinancialSegmentString = model.FinancialSegmentString.SafeToUpper();
            var account = Repository.OfType<FinancialAccount>().GetNullableById(id);
            if (account == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                       nameof(FinancialAccount));
                return this.RedirectToAction(a => a.Index());
            }

            if (RequireKfs)
            {
                if (string.IsNullOrWhiteSpace(model.Chart))
                {
                    ModelState.AddModelError("Chart", "Chart is currently required");
                }
                if (string.IsNullOrWhiteSpace(model.Account))
                {
                    ModelState.AddModelError("Account", "Account is currently required");
                }

                if (!string.IsNullOrWhiteSpace(model.FinancialSegmentString))
                {
                    var CoaValidation = await aggieEnterpriseService.ValidateAccount(model.FinancialSegmentString);
                    if (!CoaValidation.IsValid)
                    {
                        ModelState.AddModelError("FinancialSegmentString", CoaValidation.Message);
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.FinancialSegmentString))
                {
                    ModelState.AddModelError("FinancialSegmentString", "Financial Segment String is required");
                }
                var accountValidation = await _financialService.IsAccountValidForRegistration(model.FinancialSegmentString);
                if (!accountValidation.IsValid)
                {
                    ModelState.AddModelError("FinancialSegmentString", accountValidation.Message);
                }
                else
                {
                    if (accountValidation.IsWarning)
                    {
                        Message = $"Warning: COA may not be valid for use with Registration: {accountValidation.Message}";
                    }
                }
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
            account.FinancialSegmentString = model.FinancialSegmentString.SafeToUpper();


            if (!ModelState.IsValid)
            {
                Message = "Account Invalid";
                return View(model);
            }

            var warning = string.Empty;
            if (RequireKfs)
            {
                if (Repository.OfType<FinancialAccount>().Queryable.Any(a =>
                    a.IsActive && a.Id != account.Id &&
                    a.Chart == account.Chart &&
                    a.Account == account.Account &&
                    a.SubAccount == account.SubAccount &&
                    a.Project == account.Project))
                {
                    warning = "WARNING That account already exists (But we also updated this)";
                }
            }
            else
            {
                if (Repository.OfType<FinancialAccount>().Queryable.Any(a =>
                    a.IsActive && a.Id != account.Id &&
                    a.FinancialSegmentString == account.FinancialSegmentString))
                {
                    warning = "WARNING That Financial Segment String already exists (But we also updated this)";
                }
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
