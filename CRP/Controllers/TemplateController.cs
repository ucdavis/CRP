using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;

namespace CRP.Controllers
{
    [AdminOnly]
    public class TemplateController : SuperController
    {
        //
        // GET: /Template/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// GET: /Template/Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            // get the default template
            var template = Repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault();

            // check to see if it's null, just send a blank one
            if (template == null) template = new Template(); 

            return View(template);
        }

        [AcceptPost]
        [ValidateInput(false)]
        public ActionResult Edit(string text)
        {
            // get the default template
            var template = Repository.OfType<Template>().Queryable.Where(a => a.Default).FirstOrDefault();

            // check to see if it's null, just send a blank one
            if (template == null) template = new Template();

            // update the text
            template.Text = text;

            // ensure the default value
            template.Default = true;

            // validate
            template.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                Repository.OfType<Template>().EnsurePersistent(template);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Template");
                return View(template);
            }

            Message = NotificationMessages.STR_UnableToUpdate.Replace(NotificationMessages.ObjectType, "Template");
            return View(template);

        }
    }
}
