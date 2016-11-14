using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Core.Abstractions;
using Microsoft.Azure;
using Serilog;

namespace CRP.Mvc.Controllers
{
    [AdminOnly]
    public class TestController : ApplicationController
    {
        private readonly INotificationProvider _notificationProvider;
        public TestController(INotificationProvider notificationProvider)
        {
            _notificationProvider = notificationProvider;
        }
        public ActionResult TestEmail()
        {

            Log.Information("Testing Log. About to send test email");
            var message2 = new MailMessage
            {
                From = new MailAddress(CloudConfigurationManager.GetSetting("NoReplyEmail"), "Online Registration (Do Not Reply)"),
                Subject = "Test",
                Body = "<p>Test</p>",
                IsBodyHtml = true
            };

            message2.To.Add(CloudConfigurationManager.GetSetting("TestSendEmail"));

            _notificationProvider.TestEmailFromService(message2);
            Log.Information("Testing Log. Sent test email");
            return null;
        }

        public ActionResult TestException(int id = 0)
        {
            Log.Information("About to hit exception");
            throw new NotImplementedException("Har Har har. We tested it.");
            Log.Information("Hit exception");
        }
    }
}