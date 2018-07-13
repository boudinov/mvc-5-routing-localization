using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WorkingExample.RouteLocalization;

namespace WorkingExample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //routing localization
            RouteTable.Routes.MapLocalizedMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
