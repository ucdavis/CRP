using System.Web;
using System.Web.Security;
using CRP.Core.Resources;

namespace CRP.Controllers.Helpers
{
    public static class UserHelper
    {
        public static bool IsOpenId(this HttpRequestBase request)
        {
            var authCookie = request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket.UserData == StaticValues.OpenId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
