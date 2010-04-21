using System.Web.Mvc;
using UCDArch.Web.Authentication;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class AccountController : SuperController
    {
        public ActionResult LogOn(string returnUrl)
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                return Redirect(resultUrl);
            }

            TempData["URL"] = returnUrl;

            return View();
        }

        public ActionResult LogOut()
        {
            return Redirect("https://cas.ucdavis.edu/cas/logout");
        }
    }
}
