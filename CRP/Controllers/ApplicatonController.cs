using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers.Helpers;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    [LocServiceMessage("ConferenceRegistrationAndPayments", ViewDataKey = "ServiceMessages", MessageServiceAppSettingsKey = "MessageServer")]
    public class ApplicationController : SuperController { }
}
