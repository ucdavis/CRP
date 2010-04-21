using System.Web.Mvc;
using System.Web.Security;
using CRP.Core.Domain;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.Messaging;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Authentication;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class AccountController : SuperController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;

        public AccountController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository)
        {
            _openIdUserRepository = openIdUserRepository;
        }

        public ActionResult LogOn(string returnUrl)
        {
            TempData["URL"] = returnUrl;

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            
            // figure out if the user is cas? or openid
            if (!CurrentUser.Identity.Name.StartsWith("http"))
            {
                return Redirect("https://cas.ucdavis.edu/cas/logout");
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        public ActionResult CasLogon()
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                return Redirect(resultUrl);
            }

            return this.RedirectToAction(a => a.LogOn(TempData["URL"].ToString()));
        }

        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();

        [ValidateInput(false)]
        public ActionResult Authenticate(string returnUrl)
        {
            var response = openid.GetResponse();

            var identifier = Request.Form["openid_identifier"];

            if (response == null)
            {
                // Stage 2: user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(Request.Form["openid_identifier"], out id))
                {
                    try
                    {
                        return openid.CreateRequest(Request.Form["openid_identifier"]).RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException ex)
                    {
                        Message = ex.Message;
                        return View("Logon");
                    }
                }
                else
                {
                    ViewData["Message"] = "Invalid identifier";
                    return View("Logon");
                }
            }
            else
            {
                // Stage 3: OpenID Provider sending assertion response
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        Session["FriendlyIdentifier"] = response.FriendlyIdentifierForDisplay;
                        FormsAuthentication.SetAuthCookie(response.ClaimedIdentifier, false);
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    case AuthenticationStatus.Canceled:
                        ViewData["Message"] = "Canceled at provider";
                        return View("Logon");
                    case AuthenticationStatus.Failed:
                        ViewData["Message"] = response.Exception.Message;
                        return View("Logon");
                }
            }
            return new EmptyResult();
        }


        //public ActionResult OpenIdLogon()
        //{
        //    var openId = new OpenIdRelyingParty();
        //    IAuthenticationResponse response = openId.GetResponse();

        //    if (response != null)
        //    {
        //        switch (response.Status)
        //        {
        //            case AuthenticationStatus.Authenticated:

        //                var friendly = response.FriendlyIdentifierForDisplay;

        //                var sreg = response.GetExtension<ClaimsResponse>();
        //                if (sreg != null)
        //                {
        //                    // check to see if a user with that already exists
        //                    var user = _openIdUserRepository.GetNullableByID(response.ClaimedIdentifier);
        //                    if (user != null)
        //                    {
        //                        user.Email = sreg.Email;
        //                    }
        //                    else
        //                    {
        //                        user = new OpenIdUser();
        //                        user.Email = sreg.Email;
        //                        user.UserId = response.ClaimedIdentifier;
        //                    }

        //                    // user is valid, save
        //                    if (user.IsValid())
        //                    {
        //                        _openIdUserRepository.EnsurePersistent(user);
        //                    }
        //                }

        //                // redirect
        //                FormsAuthentication.RedirectFromLoginPage(
        //                    response.ClaimedIdentifier, false);
        //                break;
        //            case AuthenticationStatus.Canceled:
        //                ModelState.AddModelError("loginIdentifier",
        //                                         "Login was cancelled at the provider");
        //                break;
        //            case AuthenticationStatus.Failed:
        //                ModelState.AddModelError("loginIdentifier",
        //                                         "Login failed using the provided OpenID identifier");
        //                break;
        //        }
        //    }

        //    return View();
        //}

        //[AcceptPost]
        //public ActionResult OpenIdLogon(string loginIdentifier)
        //{
        //    if (!Identifier.IsValid(loginIdentifier))
        //    {
        //        ModelState.AddModelError("loginIdentifier", "The specified login identifier is invalid");
        //        return this.RedirectToAction(a => a.LogOn(TempData["URL"].ToString()));
        //    }
        //    else
        //    {
        //        var openId = new OpenIdRelyingParty();

        //        IAuthenticationRequest request = openId.CreateRequest(Identifier.Parse(loginIdentifier));

        //        request.AddExtension(new ClaimsRequest
        //                                 {
        //                                     Email = DemandLevel.Require
        //                                 });

        //        return request.RedirectingResponse.AsActionResult();
        //    }
        //}
    }
}
