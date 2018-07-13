using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace WorkingExample.RouteLocalization
{
    public class CultureFilter : IAuthorizationFilter
    {
        private readonly string _defaultCulture;

        public CultureFilter(string defaultCulture)
        {
            this._defaultCulture = defaultCulture;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var attributes = filterContext.ActionDescriptor.GetFilterAttributes(true);
            if (attributes.OfType<ForceCultureAttribute>().Any())
                return;

            var values = filterContext.RouteData.Values;

            string culture = (string) values["culture"] ?? _defaultCulture;

            CultureInfo ci = new CultureInfo(culture);

            //application wide!
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            ci.NumberFormat.PercentDecimalSeparator = ".";
            ci.NumberFormat.NumberGroupSeparator = " ";
            ci.NumberFormat.CurrencyGroupSeparator = " ";

            Thread.CurrentThread.CurrentCulture = ci;
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }

    public class ForceCultureAttribute : FilterAttribute, IAuthorizationFilter
    {
        public string Culture { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            CultureInfo ci = new CultureInfo(Culture);

            Thread.CurrentThread.CurrentCulture = ci;
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}