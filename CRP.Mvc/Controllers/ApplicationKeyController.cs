using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers.Filter;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using MvcContrib;

namespace CRP.Controllers
{
    [AdminOnly]
    [PageTracker]
    public class ApplicationKeyController : ApplicationController
    {
        private readonly IRepository<ApplicationKey> _applicationKeyRepository;

        public ApplicationKeyController(IRepository<ApplicationKey> applicationKeyRepository)
        {
            _applicationKeyRepository = applicationKeyRepository;
        }

        //
        // GET: /ApplicationKey/

        public ActionResult Index()
        {
            var keys = _applicationKeyRepository.Queryable;
            return View(keys);
        }

        public ActionResult Create()
        {
            var viewModel = ApplicationKeyViewModel.Create(Repository, new ApplicationKey());
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ApplicationKey applicationKey)
        {
            applicationKey.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _applicationKeyRepository.EnsurePersistent(applicationKey);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Application Key");
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = ApplicationKeyViewModel.Create(Repository, applicationKey);
            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var applicationKey = _applicationKeyRepository.GetNullableById(id);
            if (applicationKey == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Application Key");
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = ApplicationKeyViewModel.Create(Repository, applicationKey);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, ApplicationKey applicationKey)
        {
            var srcAppKey = _applicationKeyRepository.GetNullableById(id);
            if (srcAppKey == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Application Key");
                return this.RedirectToAction(a => a.Index());
            }

            srcAppKey.Application = applicationKey.Application;
            srcAppKey.Key = applicationKey.Key;

            srcAppKey.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _applicationKeyRepository.EnsurePersistent(srcAppKey);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Application Key");
                return this.RedirectToAction(a=>a.Index());
            }

            var viewModel = ApplicationKeyViewModel.Create(Repository, applicationKey);
            return View(viewModel);
        }

        public RedirectToRouteResult ToggleActive(int id)
        {
            var applicationKey = _applicationKeyRepository.GetNullableById(id);
            if (applicationKey == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Application Key");
                return this.RedirectToAction(a => a.Index());
            }

            applicationKey.IsActive = !applicationKey.IsActive;
            _applicationKeyRepository.EnsurePersistent(applicationKey);
            Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Application Key");

            return this.RedirectToAction(a => a.Index());
        }
    }

    public class ApplicationKeyViewModel
    {
        public ApplicationKey ApplicationKey { get; set; }

        public static ApplicationKeyViewModel Create(IRepository repository, ApplicationKey applicationKey)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ApplicationKeyViewModel() {ApplicationKey = applicationKey};

            return viewModel;
        }
    }
}
