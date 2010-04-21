using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRP.Controllers.Helpers
{
    public static class ReturnUrlGenerator
    {
        private static UrlHelper Url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

        public static string EditItemUrl(int itemId, string tabName)
        {
            var returnUrl = Url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;

            return returnUrl;
        }

        public static string DetailItemUrl(int itemId, string tabName)
        {
            var returnUrl = Url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

            return returnUrl;
        }


        public static string EditItemUrl(this HtmlHelper html, int itemId, string tabName)
        {
            var url = Url.RouteUrl(new {controller = "ItemManagement", action = "Edit", id = itemId}) + "#" + tabName;

            var link = string.Format("<a href='{0}>Back to Item</a>", url);

            return link;
        }

        public static string DetailItemUrl(this HtmlHelper htmlHelper, int itemId, string tabName)
        {
            var url = Url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

            var link = string.Format("<a href='{0}>Back to Item</a>", url);
            
            return link;
        }

    }
}
