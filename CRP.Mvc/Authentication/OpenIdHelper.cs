using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace CRP.Authentication
{   
    public static class OpenIdHelper
    {
        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();
        private const string CancelledAuthentication = "Authentication was cancelled at the provider.";
        /// <summary>
        /// String to use to check if the forms authenticated user is openid.  Really only necessary if using dual forms authentication.
        /// </summary>
        public const string OpenId = "OpenId";
        /// <summary>
        /// Enumeration of possible fields that the open id provider might provide
        /// </summary>
        public enum RequestInformation { Birthdate, Country, Email, FullName, Gender, Language, Nickname, PostalCode, TimeZone }

        /// <summary>
        /// Takes the user's selected openid provider and attempts to send the request to get the user authenticated
        /// </summary>
        /// <param name="openIdIdentifier"></param>
        /// <param name="claimsRequest"></param>
        /// <returns></returns>
        public static ActionResult Login(string openIdIdentifier, ClaimsRequest claimsRequest)
        {
            Identifier id;
            if (Identifier.TryParse(openIdIdentifier, out id))
            {
                // create and submit the request to the user's open id provider
                var request = openid.CreateRequest(id);
                if (claimsRequest != null) request.AddExtension(claimsRequest);
                return request.RedirectingResponse.AsActionResult();
            }
            
            // not a valid identifier, just go back to the view
            return new ViewResult();
        }

        /// <summary>
        /// Handles the response from the openid provider
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="openIdUser"></param>
        /// <param name="message">if return value is false, this will give the reason</param>
        /// <returns></returns>
        public static bool ValidateResponse(out OpenIdUser openIdUser, out string message)
        {
            var response = openid.GetResponse();

            // set the defaults on the out parameters
            openIdUser = new OpenIdUser();
            message = string.Empty;

            if (response != null)
            {
                switch(response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        CreateTicket(response.ClaimedIdentifier);       // create a forms authentication ticket, user is authenticated
                        openIdUser = ParseClaimsResponse(response.ClaimedIdentifier, response.GetExtension<ClaimsResponse>());  // parse the returned information
                        return true;
                    case AuthenticationStatus.Canceled:
                        message = CancelledAuthentication;
                        return false;
                    case AuthenticationStatus.Failed:
                        message = response.Exception.Message;
                        return false;
                };
            }
            return false;
        }

        /// <summary>
        /// Creates a claims request for the proper fields you want.
        /// </summary>
        /// <param name="requestedFields"></param>
        /// <returns></returns>
        public static ClaimsRequest CreateClaimsRequest(params RequestInformation[] requestedFields)
        {
            var claimsRequest = new ClaimsRequest();

            foreach (var rf in requestedFields)
            {
                switch (rf)
                {
                    case RequestInformation.Birthdate:
                        claimsRequest.BirthDate = DemandLevel.Require;
                        break;
                    case RequestInformation.Country:
                        claimsRequest.Country = DemandLevel.Require;
                        break;
                    case RequestInformation.Email:
                        claimsRequest.Email = DemandLevel.Require;
                        break;
                    case RequestInformation.FullName:
                        claimsRequest.FullName = DemandLevel.Require;
                        break;
                    case RequestInformation.Gender:
                        claimsRequest.Gender = DemandLevel.Require;
                        break;
                    case RequestInformation.Language:
                        claimsRequest.Language = DemandLevel.Require;
                        break;
                    case RequestInformation.Nickname:
                        claimsRequest.Nickname = DemandLevel.Require;
                        break;
                    case RequestInformation.PostalCode:
                        claimsRequest.PostalCode = DemandLevel.Require;
                        break;
                    case RequestInformation.TimeZone:
                        claimsRequest.TimeZone = DemandLevel.Require;
                        break;
                };
            }

            return claimsRequest;
        }

        /// <summary>
        /// This creates a forms authentication ticket and logins in the user
        /// </summary>
        /// <param name="userId"></param>
        private static void CreateTicket(string userId)
        {
            // create authenication ticket
            var authTicket = new FormsAuthenticationTicket(1,
                                                           userId,
                                                           DateTime.Now, DateTime.Now.AddMinutes(15), false, OpenIdHelper.OpenId,
                                                           FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(authTicket);

            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }
        /// <summary>
        /// Parse the result from the open id provider and populat an OpenIdUser object.
        /// </summary>
        /// <param name="claimedIdentifier"></param>
        /// <param name="claimsResponse"></param>
        /// <returns></returns>
        private static OpenIdUser ParseClaimsResponse(string claimedIdentifier, ClaimsResponse claimsResponse)
        {
            var user = new OpenIdUser();

            user.ClaimedIdentifier = claimedIdentifier;
            
            if (claimsResponse != null)
            {
                user.Birthdate = claimsResponse.BirthDate;
                user.Country = claimsResponse.Country;
                user.Email = claimsResponse.Email;
                user.FullName = claimsResponse.FullName;
                user.Gender = claimsResponse.Gender.ToString();
                user.Language = claimsResponse.Language;
                user.Nickname = claimsResponse.Nickname;
                user.PostalCode = claimsResponse.PostalCode;
                user.TimeZone = claimsResponse.TimeZone;
            }

            return user;
        }
    }

    public class OpenIdUser
    {
        /// <summary>
        /// Essentially the userid
        /// </summary>
        public string ClaimedIdentifier { get; set; }

        public DateTime? Birthdate { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Language { get; set; }
        public string Nickname { get; set; }
        public string PostalCode { get; set; }
        public string TimeZone { get; set; }
    }
}
