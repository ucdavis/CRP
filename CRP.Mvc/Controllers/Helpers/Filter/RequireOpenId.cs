using System;
using System.Web.Mvc;
using System.Web.Routing;
using CRP.Controllers.Helpers;

namespace CRP.Controllers.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireOpenId : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsOpenId())
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Logon", openIdLogin = true }));
            }
        }
    }
}
