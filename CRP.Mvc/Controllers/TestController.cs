using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Core.Abstractions;
using CRP.Mvc.Services;
using Microsoft.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;


namespace CRP.Mvc.Controllers
{
    [AdminOnly]
    public class TestController : ApplicationController
    {
        private readonly INotificationProvider _notificationProvider;
        private readonly ISlothService _slothService;
        public TestController(INotificationProvider notificationProvider, ISlothService slothService)
        {
            _notificationProvider = notificationProvider;
            _slothService = slothService;
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




        //[HttpPost]
        //public ActionResult TestCaptcha(bool? meh)
        //{
        //    var success = false;

        //    var response = Request.Form["g-Recaptcha-Response"];

        //    using (var client = new WebClient())
        //    {
        //        var www = JsonConvert.DeserializeObject(client.DownloadString(String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", CloudConfigurationManager.GetSetting("NewRecaptchaPrivateKey"), response)));
        //        dynamic data = JObject.FromObject(www);
        //        success = data.success;

        //    }
        //    return Content(success.ToString());
        //}

        public ActionResult TestCaptcha()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> TestCaptcha(bool? meh)
        {
            var success = false;

            var response = Request.Form["g-Recaptcha-Response"];

            using (var client = new HttpClient())
            {

                var googleResponse = await client.PostAsync(String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", CloudConfigurationManager.GetSetting("NewRecaptchaPrivateKey"), response), null);
                googleResponse.EnsureSuccessStatusCode();
                var responseContent = JsonConvert.DeserializeObject(await googleResponse.Content.ReadAsStringAsync());
                dynamic data = JObject.FromObject(responseContent);
                success = data.success;                
            }
            return Content(success.ToString());
        }
    }
}