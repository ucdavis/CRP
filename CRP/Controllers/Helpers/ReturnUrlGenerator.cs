using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRP.Controllers.Helpers
{
    public class ReturnUrlGenerator
    {
        private static UrlHelper Url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

        public static string EditItemUrl(int itemId, string tabName)
        {
            var returnUrl = Url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;

            return returnUrl;
        }
    }
}
