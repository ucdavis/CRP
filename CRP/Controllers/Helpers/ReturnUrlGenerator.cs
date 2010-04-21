using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRP.Controllers.Helpers
{
    public static class ReturnUrlGenerator
    {
        //private static UrlHelper Url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
        // extension methods
        public static string EditItemUrl(this UrlHelper url, int itemId, string tabName)
        {
            var returnUrl = //Url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;
                        url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;

            return returnUrl;
        }

        public static string DetailItemUrl(this UrlHelper url, int itemId, string tabName)
        {
            var returnUrl = url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

            return returnUrl;            
        }

        //public static string EditUrl(this UrlHelper urlHelper, int itemId)
        //{
        //    urlHelper.
        //}
    }
}
