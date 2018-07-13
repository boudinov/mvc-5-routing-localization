﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WorkingExample.RouteLocalization;

namespace WorkingExample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                      name: "DefaultWithCulture",
                      url: "{culture}/{controller}/{action}/{id}",
                      defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                      constraints: new { culture = new CultureConstraint(defaultCulture: ConfigurationManager.AppSettings["DefaultCulture"]) }
                      );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { culture = ConfigurationManager.AppSettings["DefaultCulture"], controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
