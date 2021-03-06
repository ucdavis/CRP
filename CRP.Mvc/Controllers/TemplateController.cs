using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;

namespace CRP.Controllers
{
    [AdminOnly]
    [PageTracker]
    public class TemplateController : ApplicationController
    {
        /// <summary>
        /// GET: /Template/Edit
        /// Tested 20200408
        /// Called from ApplicationManagement Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            // get the default template
            var template = Repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault();

            // check to see if it's null, just send a blank one
            if (template == null) template = new Template();
            var viewModel = ConfirmationTemplateViewModel.Create(template);

            return View(viewModel);
        }

        /// <summary>
        /// Tested 20200408
        /// </summary>
        /// <param name="paidText"></param>
        /// <param name="unpaidText"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string paidText, string unpaidText)
        {
            // get the default template
            var template = Repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault();

            // check to see if it's null, just send a blank one
            if (template == null) template = new Template();

            // update the text
            template.Text = paidText + StaticValues.ConfirmationTemplateDelimiter + unpaidText;
            if(template.Text.Trim() == StaticValues.ConfirmationTemplateDelimiter)
            {
                ModelState.AddModelError("Text", "text may not be null or empty");
            } else if (string.IsNullOrWhiteSpace(paidText))
            {
                ModelState.AddModelError("Text", "text may not be null or empty");
            }

            // ensure the default value
            template.Default = true;

            // validate
            template.TransferValidationMessagesTo(ModelState);
            var viewModel = ConfirmationTemplateViewModel.Create(template);
            if (ModelState.IsValid)
            {
                Repository.OfType<Template>().EnsurePersistent(template);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Template");
                
                return View(viewModel);
            }

            Message = NotificationMessages.STR_UnableToUpdate.Replace(NotificationMessages.ObjectType, "Template");
            return View(viewModel);

        }
    }
}
