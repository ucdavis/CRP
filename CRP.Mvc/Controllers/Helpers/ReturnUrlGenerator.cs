using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRP.Controllers.Helpers 
{
    //public static class ReturnUrlGenerator
    //{
    //    private static UrlHelper Url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

    //    //public static string EditItemUrl(int itemId, string tabName)
    //    //{
    //    //    var returnUrl = Url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;

    //    //    return returnUrl;
    //    //}

    //    //public static string DetailItemUrl(int itemId, string tabName)
    //    //{
    //    //    var returnUrl = Url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

    //    //    return returnUrl;
    //    //}


    //    public static string EditItemUrl(this HtmlHelper html, int itemId, string tabName)
    //    {
    //        var url = Url.RouteUrl(new {controller = "ItemManagement", action = "Edit", id = itemId}) + "#" + tabName;

    //        var link = string.Format("<a href='{0}'>Back to Item</a>", url);

    //        return link;
    //    }

    //    public static string DetailItemUrl(this HtmlHelper htmlHelper, int itemId, string tabName)
    //    {
    //        var url = Url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

    //        var link = string.Format("<a href='{0}'>Back to Item</a>", url);
            
    //        return link;
    //    }
    //}

    public static class ControllerReturnUrlGenerator
    {
        private const string link = @"<a href='{0}'>Back to Item</a>";

        public static string DetailItemUrl(this UrlHelper url, int itemId, string tabName)
        {
            return url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;
        }
        public static string DetailItemUrl(this UrlHelper url, int itemId, string tabName, string sort, string page)
        {
            return url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) +
                   "?" + tabName + "-orderBy=" + sort + "&" + tabName + "-page=" + page + "#" + tabName;
        } 
        public static string DetailItemLink(this UrlHelper url, int itemId, string tabName)
        {
            var returnUrl = url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) + "#" + tabName;

            return string.Format(link, returnUrl);
        }
        public static string DetailItemLink(this UrlHelper url, int itemId, string tabName, string sort, string page)
        {
            var pageAndSort = ValidateParameters.PageAndSort("ItemDetail", sort, page);
            var returnUrl = url.RouteUrl(new { controller = "ItemManagement", action = "Details", id = itemId }) +
                   "?" + tabName + "-orderBy=" + pageAndSort["sort"] + "&" + tabName + "-page=" + pageAndSort["page"] + "#" + tabName;

            return string.Format(link, returnUrl);
        }

        public static string EditItemUrl(this UrlHelper url, int itemId, string tabName)
        {
            return url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;
        }

        public static string EditItemLink(this UrlHelper url, int itemId, string tabName)
        {
            var returnUrl = url.RouteUrl(new { controller = "ItemManagement", action = "Edit", id = itemId }) + "#" + tabName;

            return string.Format(link, returnUrl);
        }

        public static string ItemManagementListLink(this UrlHelper url, string linkText)
        {            
            var returnUrl = url.RouteUrl(new { controller = "ItemManagement", action = "List"}) +
                   "?orderBy=DateCreated-desc&page=1";

            return string.Format(@"<a href='{0}'>{1}</a>", returnUrl, linkText);
        }

        public static string ItemDetailsPath(this UrlHelper url, int? itemId)
        {
            if(itemId==null || itemId == 0)
            {
                return "Save Item to see link.";
            }
            var returnUrl = url.Action("Details", "Item", new { id = itemId }, "https");
            return returnUrl;
        }
        
    }
}
