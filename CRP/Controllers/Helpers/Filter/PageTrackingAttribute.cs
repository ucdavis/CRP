using System.Web.Mvc;
using CRP.Core.Domain;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Controllers.Helpers.Filter
{
    public class PageTrackerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {                
                var url = filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri;
                var login = filterContext.RequestContext.HttpContext.User.Identity.Name;
                var address = filterContext.RequestContext.HttpContext.Request.UserHostAddress;

                var pageTracking = new PageTracking(login, url, address);

                var repository = SmartServiceLocator<IRepository>.GetService();

                repository.OfType<PageTracking>().EnsurePersistent(pageTracking);
            }
            catch
            {
                // do nothing, problem with tracking
            }

            base.OnActionExecuted(filterContext);
        }

    }
}