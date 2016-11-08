using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using CRP.Authentication;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.Helpers.Filter;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;
using OpenIdUser=CRP.Core.Domain.OpenIdUser;

namespace CRP.Controllers
{
    public class AccountController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;

        private string ReturnUrl
        {
            get
            {
                if (Session["URL"] != null)
                {
                    return Session["URL"].ToString();
                }

                return string.Empty;
            }
            set
            {
                Session["URL"] = value;
            }
        }

        public AccountController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository)
        {
            _openIdUserRepository = openIdUserRepository;
        }

        public ActionResult LogOn(string returnUrl, bool? openIdLogin)
        {
            ReturnUrl = returnUrl;

            if (openIdLogin.HasValue && openIdLogin.Value)
            {
                return View();
            }

            return this.RedirectToAction(a => a.CasLogon(returnUrl));
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            // build a return url
            var returnUrl = String.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Action("Index", "Home"));

            // figure out if the user is cas? or openid
            if (!Request.IsOpenId())
            {
                return Redirect("https://cas.ucdavis.edu/cas/logout?service=" + returnUrl);
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }
        
        public ActionResult CasLogon(string returnUrl)
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                return Redirect(resultUrl);
            }

            //return this.RedirectToAction(a => a.LogOn(returnUrl, null));

            // not authorized?
            return View();
        }

        #region Open Id authentication section
        /// <summary>
        /// Submit request for authentication from open id provider
        /// </summary>
        /// <param name="openid_identifier"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Authenticate(string openid_identifier)
        {
            return OpenIdHelper.Login(openid_identifier, OpenIdHelper.CreateClaimsRequest(OpenIdHelper.RequestInformation.Email));
        }

        /// <summary>
        /// Response from OpenId provider telling is user is authentic or not
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Authenticate()
        {
            Authentication.OpenIdUser openIdUser;
            string message;

            if (OpenIdHelper.ValidateResponse(out openIdUser, out message))
            {
                // check to see if we need to create an account in the system
                var user = _openIdUserRepository.GetNullableById(openIdUser.ClaimedIdentifier);

                // does not exist create a new one
                if (user == null)
                {
                    user = new OpenIdUser() { UserId = openIdUser.ClaimedIdentifier };

                    openIdUser.Email = openIdUser.Email;

                    _openIdUserRepository.EnsurePersistent(user);

                    // redirect to the edit page
                    return this.RedirectToAction(a => a.OpenIdAccount());
                }
                // user exists but email was never filled out
                else if (string.IsNullOrEmpty(user.Email))
                {
                    Message = "Please fill in at least an email address.";

                    return this.RedirectToAction(a => a.OpenIdAccount());
                }

                // redirect to the return url
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            // failure, give the message to the user
            Message = message;
            return this.RedirectToAction<AccountController>(a => a.LogOn(ReturnUrl, true));
        }

        #endregion

        #region OpenId
        [RequireOpenId]
        public ActionResult OpenIdAccount()
        {
            var openIdUser = _openIdUserRepository.GetNullableById(CurrentUser.Identity.Name);

            if (openIdUser == null)
            {
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            return View(openIdUser);
        }

        [RequireOpenId]
        [HttpPost]
        public ActionResult OpenIdAccount(OpenIdUser openIDUser)
        {
            var destOpenIdUser = _openIdUserRepository.GetNullableById(CurrentUser.Identity.Name);

            if (destOpenIdUser == null)
            {
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            destOpenIdUser = Copiers.CopyOpenIdUser(openIDUser, destOpenIdUser);

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, destOpenIdUser.ValidationResults());

            if (ModelState.IsValid)
            {
                _openIdUserRepository.EnsurePersistent(destOpenIdUser);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType,
                                                                       "User information");
            }

            return View(destOpenIdUser);
        }
        #endregion

        [UserAdminOnly]
        [PageTracker]
        public ActionResult ManageUsers()
        {
            //var adminPageUrl = CloudConfigurationManager.GetSetting("AdminPageUrl");

            return View();
        }
    }
}
