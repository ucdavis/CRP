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

namespace CRP.Controllers
{
    public class HelpController : ApplicationController
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            HelpTopicViewModel viewModel = HelpTopicViewModel.Create(Repository, CurrentUser);
            return View(viewModel);
        }

        //
        // GET: /Help/Details/5

        public ActionResult Details(int id)
        {
            var helpTopic = Repository.OfType<HelpTopic>().GetNullableById(id);
            if (helpTopic == null)
            {
                return this.RedirectToAction(a => a.Index());
            }
            helpTopic.NumberOfReads++;
            Repository.OfType<HelpTopic>().EnsurePersistent(helpTopic);
            return View(helpTopic);
        }

        //
        // GET: /Help/Create
        [AdminOnly]
        public ActionResult Create()
        {
            return View(new HelpTopic());
        } 

        //
        // POST: /Help/Create

        [AdminOnly]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(HelpTopic helpTopic)
        {
            var topic = new HelpTopic();
            topic.Question = helpTopic.Question;
            topic.Answer = helpTopic.Answer;
            topic.AvailableToPublic = helpTopic.AvailableToPublic;
            topic.IsActive = helpTopic.IsActive;
            topic.NumberOfReads = helpTopic.NumberOfReads;
            topic.IsVideo = helpTopic.IsVideo;
            topic.VideoName = helpTopic.VideoName;

            topic.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                Repository.OfType<HelpTopic>().EnsurePersistent(topic);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType,
                                                                       "Help Topic");
                return this.RedirectToAction<HelpController>(a => a.Index());
            }

            return View(helpTopic);
        }

        //
        // GET: /Help/Edit/5
        [AdminOnly]
        public ActionResult Edit(int id)
        {
            var helpTopic = Repository.OfType<HelpTopic>().GetNullableById(id);
            if (helpTopic == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType,
                                                                      "Help Topic");
                return this.RedirectToAction(a => a.Index());
            }
            return View(helpTopic);
        }

        //
        // POST: /Help/Edit/5

        [AdminOnly]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, HelpTopic helpTopic)
        {
            var topic = Repository.OfType<HelpTopic>().GetNullableById(id);
            if (helpTopic == null)
            {
                return this.RedirectToAction(a => a.Index());
            }
            topic.Question = helpTopic.Question;
            topic.Answer = helpTopic.Answer;
            topic.AvailableToPublic = helpTopic.AvailableToPublic;
            topic.IsActive = helpTopic.IsActive;
            topic.NumberOfReads = helpTopic.NumberOfReads;
            topic.IsVideo = helpTopic.IsVideo;
            topic.VideoName = helpTopic.VideoName;
            topic.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                Repository.OfType<HelpTopic>().EnsurePersistent(topic);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                                                                     "Help Topic");
                return this.RedirectToAction<HelpController>(a => a.Index());
            }

            return View(helpTopic);
        }

        /// <summary>
        /// This is only for running the Create Item Demo video. There are links to this on the pages related to creating items.
        /// </summary>
        /// <returns></returns>
        [AnyoneWithRole]
        public ActionResult CreateItem()
        {
            return View();
        }

        [AnyoneWithRole]
        public ActionResult LinkMap()
        {
            return View();
        }

        public ActionResult WatchVideo(int id)
        {
            var helpTopic = Repository.OfType<HelpTopic>().GetNullableById(id);
            if (helpTopic == null)
            {
                return this.RedirectToAction(a => a.Index());
            }
            helpTopic.NumberOfReads++;
            Repository.OfType<HelpTopic>().EnsurePersistent(helpTopic);
            return View(helpTopic);
        }
    }
}
