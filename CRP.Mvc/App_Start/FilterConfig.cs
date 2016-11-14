using System;
using System.Web;
using System.Web.Mvc;
using Serilog;

namespace CRP.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
         Inherited = true, AllowMultiple = true)]
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            // log exception here via stackify
            Log.Error(filterContext.Exception, filterContext.Exception.Message);
            base.OnException(filterContext);
        }
    }
}
