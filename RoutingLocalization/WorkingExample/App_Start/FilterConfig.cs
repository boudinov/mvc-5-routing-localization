using System.Configuration;
using System.Web;
using System.Web.Mvc;
using WorkingExample.RouteLocalization;

namespace WorkingExample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new CultureFilter(ConfigurationManager.AppSettings["DefaultCulture"]));
        }
    }
}
