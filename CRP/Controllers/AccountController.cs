using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.Messaging;
using MvcContrib.Attributes;
using CRP.Core.Resources;
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

        public ActionResult LogOn(string returnUrl, bool? casLogon)
        {
            TempData["URL"] = returnUrl;

            if (casLogon.HasValue && casLogon.Value)
            {
                return this.RedirectToAction(a => a.CasLogon());
            }

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            
            // figure out if the user is cas? or openid
            if (!Request.IsOpenId())
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

            return this.RedirectToAction(a => a.LogOn(TempData["URL"].ToString(), null));
        }

        #region Open Id authentication section
        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();
        private const string openid_identifier = "openid_identifier";

        /// <summary>
        /// Submit request for authentication from open id provider
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="openid_identifier"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Authenticate(string returnUrl, string openid_identifier)
        {
            // Stage 2: user submitting Identifier
            Identifier id;
            if (Identifier.TryParse(openid_identifier, out id))
            {
                try
                {
                    Session[openid_identifier] = openid_identifier;
                    return openid.CreateRequest(id).RedirectingResponse.AsActionResult();
                }
                catch (ProtocolException ex)
                {
                    Message = ex.Message;
                    return this.RedirectToAction(a => a.LogOn(returnUrl, false));
                }
            }
            else
            {
                Message = "Invalid identifier";
                return this.RedirectToAction(a => a.LogOn(returnUrl, false));
            }
        }

        /// <summary>
        /// Response from OpenId provider telling is user is authentic or not
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Authenticate(string returnUrl)
        {
            var response = openid.GetResponse();

            if (response != null)
            {
                // Stage 3: OpenID Provider sending assertion response
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        Session["FriendlyIdentifier"] = response.FriendlyIdentifierForDisplay;

                        var authTicket = new FormsAuthenticationTicket(1,
                                                                       response.ClaimedIdentifier,
                                                                       DateTime.Now, DateTime.Now.AddMinutes(15), false, StaticValues.OpenId,
                                                                       FormsAuthentication.FormsCookiePath);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);

                        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return this.RedirectToAction<HomeController>(a => a.Index());
                        }
                    case AuthenticationStatus.Canceled:
                        Message = "Canceled at provider";
                        return this.RedirectToAction(a => a.LogOn(returnUrl, false));
                    case AuthenticationStatus.Failed:
                        Message = response.Exception.Message;
                        return this.RedirectToAction(a => a.LogOn(returnUrl, false));
                }
            }

            return new EmptyResult();
        }
        #endregion

        #region OpenId
        [RequireOpenId]
        public ActionResult OpenIdAccount()
        {
            var openIdUser = _openIdUserRepository.GetNullableByID(CurrentUser.Identity.Name);

            if (openIdUser == null)
            {
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            return View(openIdUser);
        }
        #endregion
    }
}
