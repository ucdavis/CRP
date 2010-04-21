using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using CRP.Controllers;
using CRP.ModelBinder;
using Microsoft.Practices.ServiceLocation;
using MvcContrib.Castle;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;
using UCDArch.Web.Validator;

namespace CRP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            #if DEBUG
            HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
            #endif

            xVal.ActiveRuleProviders.Providers.Add(new ValidatorRulesProvider());

            //Register the routes for this site
            new RouteConfigurator().RegisterRoutes();

            ModelBinders.Binders.DefaultBinder = new CustomModelBinder(); //new UCDArchModelBinder();

            InitializeServiceLocator();
        }

        private static void InitializeServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer();
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return;
        }
    }
}