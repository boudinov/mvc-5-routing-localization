using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace WorkingExample.RouteLocalization
{
    public class CultureConstraint : IRouteConstraint
    {
        private readonly string _defaultCulture;
        private readonly Regex _cultureRegex = new Regex("^[a-z]{2}$");

        public CultureConstraint(string defaultCulture)
        {
            this._defaultCulture = defaultCulture;
        }

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration &&
                this._defaultCulture.Equals(values[parameterName]))
            {
                return false;
            }
            else
            {
                //TODO can check if cutlure is a valid ISO culture
                return _cultureRegex.IsMatch((string) values[parameterName]);
            }
        }
    }
}