using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using Microsoft.Azure;

namespace CRP.Mvc.Controllers
{
    [AdminOnly]
    public class TestController : ApplicationController
    {
        public ActionResult TestEmail()
        {

            var client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(CloudConfigurationManager.GetSetting("CrpEmail"), CloudConfigurationManager.GetSetting("EmailToken")),
                Port = 587,
                Host = "smtp.ucdavis.edu",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            var message2 = new MailMessage
            {
                From = new MailAddress(CloudConfigurationManager.GetSetting("NoReplyEmail"), "Online Registration (Do Not Reply)"),
                Subject = "Test",
                Body = "<p>Test</p>",
                IsBodyHtml = true
            };

            message2.To.Add(CloudConfigurationManager.GetSetting("TestSendEmail"));

            client.Send(message2);

            return null;
        }
    }
}