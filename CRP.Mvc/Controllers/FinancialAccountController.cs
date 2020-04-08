using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers.Filter;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;

namespace CRP.Controllers
{
    [AdminOnly]
    public class FinancialAccountController : ApplicationController
    {

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
        public ActionResult Details(int id)
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
        public ActionResult Create(FinancialAccount model)
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
                Chart       = model.Chart,
                Account     = model.Account,
                SubAccount  = model.SubAccount,
                Project     = model.Project,
            };

            Repository.OfType<FinancialAccount>().EnsurePersistent(account);
            Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                   nameof(FinancialAccount));
            return this.RedirectToAction(a => a.Index());
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
        public ActionResult Edit(int id, FinancialAccount model)
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

            account.Name = model.Name;
            account.Description = model.Description;
            account.Chart = model.Chart;
            account.Account = model.Account;
            account.SubAccount = model.SubAccount;
            account.Project = model.Project;

            Repository.OfType<FinancialAccount>().EnsurePersistent(account);
            Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                nameof(FinancialAccount));
            return this.RedirectToAction(a => a.Index());

        }
    }
}
