
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace WorkingExample.RouteLocalization
{
    public static class RouteCollectionExtensions
    {
        public static void MapLocalizedMvcAttributeRoutes(this RouteCollection routes)
        {
            var routeProvider = new LocalizeDirectRouteProvider(
                "{culture}/",
                defaultCulture: ConfigurationManager.AppSettings["DefaultCulture"],
                allCultures: ConfigurationManager.AppSettings["SupportedCultures"].Split(new[] { ',' })
                );
            routes.MapMvcAttributeRoutes(routeProvider);
        }
    }

    class LocalizeDirectRouteProvider : DefaultDirectRouteProvider
    {
        string _urlPrefix;
        string _defaultCulture;
        string[] _allCultures;
        bool _prefixDefaultCulture = false;

        RouteValueDictionary _constraints;
        RouteValueDictionary _defaults;

        public LocalizeDirectRouteProvider(string urlPrefix, string defaultCulture, string[] allCultures)
        {
            _urlPrefix = urlPrefix;
            _defaultCulture = defaultCulture;
            _allCultures = allCultures;
            //the default language must be last, so route without /en prefix is defined last
            Array.Sort(_allCultures, (x, y) => x == defaultCulture ? 1 : x.CompareTo(y));
            _constraints = new RouteValueDictionary() { { "culture", new CultureConstraint(defaultCulture: defaultCulture) } };
            _defaults = new RouteValueDictionary() { { "culture", defaultCulture } };
            _prefixDefaultCulture = Convert.ToBoolean(ConfigurationManager.AppSettings["PrefixDefaultCulture"]);
        }

        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(
                    ActionDescriptor actionDescriptor,
                    IReadOnlyList<IDirectRouteFactory> factories,
                    IInlineConstraintResolver constraintResolver)
        {
            var originalEntries = base.GetActionDirectRoutes(actionDescriptor, factories, constraintResolver);
            var finalEntries = new List<RouteEntry>();

            foreach (RouteEntry originalEntry in originalEntries)
            {
                string explicitCulture = "";
                bool translateUrl = false;

                var asLocalizedRouteEntry = originalEntry as LocalizedRouteEntry;
                if (asLocalizedRouteEntry != null)
                {
                    explicitCulture = asLocalizedRouteEntry.ExplicitCulture;
                    translateUrl = asLocalizedRouteEntry.TranslateUrl;
                }

                //scenario with LocalizedRoute(...,ExplcitCulture="bg"..)
                if (explicitCulture != "")
                {
                    var newEntry = CreateExplicitCultureRouteEntry(originalEntry, explicitCulture, translateUrl);
                    finalEntries.Add(newEntry);
                }
                else
                {
                    //this is the most common scenario, when Route(...) is used
                    if (!translateUrl)
                    {
                        var localizedRoute = CreateLocalizedRoute(originalEntry.Route, _urlPrefix, new RouteValueDictionary(), _constraints, "");
                        var localizedRouteEntry = CreateLocalizedRouteEntry(originalEntry.Name, localizedRoute);
                        finalEntries.Add(localizedRouteEntry);
                        //for default culture use the original entry (without prefix)
                        //not making a copy for performance
                        originalEntry.Route.Defaults.Add("culture", _defaultCulture);
                        finalEntries.Add(originalEntry);
                    }
                    //default scenario with LocalizedRoute(...)
                    else
                    {
                        foreach (var culture in _allCultures)
                        {
                            var newEntry = CreateExplicitCultureRouteEntry(originalEntry, culture, true);
                            finalEntries.Add(newEntry);
                        }
                    }
                }
            }

            return finalEntries;
        }

        private RouteEntry CreateExplicitCultureRouteEntry(RouteEntry originalEntry, string culture, bool translateUrl)
        {
            bool cultureIsDefault = culture == _defaultCulture;
            var route = CreateLocalizedRoute(
                originalEntry.Route,
                cultureIsDefault && !_prefixDefaultCulture ? "" : _urlPrefix,
                cultureIsDefault ? new RouteValueDictionary { { "culture", culture } } : new RouteValueDictionary(),
                cultureIsDefault ? new RouteValueDictionary() : new RouteValueDictionary { { "culture", culture } }, //constraint culture route parameter to e.g. 'en'
                translateUrl ? culture : ""
                );
            return cultureIsDefault ? new RouteEntry(originalEntry.Name, route) : CreateLocalizedRouteEntry(originalEntry.Name, route, culture);
        }

        private Route CreateLocalizedRoute(Route route, string urlPrefix, RouteValueDictionary defaults, RouteValueDictionary constraints, string translateForCulture)
        {
            string routeUrl = string.IsNullOrEmpty(translateForCulture) ? route.Url : TranslateUrl(route.Url, translateForCulture);

            // Add the URL prefix
            var culturePrefixedUrl = urlPrefix + routeUrl;

            var routeDefaults = new RouteValueDictionary(defaults);
            foreach (var def in route.Defaults)
            {
                routeDefaults.Add(def.Key, def.Value);
            }

            // Combine the constraints
            var routeConstraints = new RouteValueDictionary(constraints);
            foreach (var constraint in route.Constraints)
            {
                routeConstraints.Add(constraint.Key, constraint.Value);
            }

            return new Route(culturePrefixedUrl, routeDefaults, routeConstraints, route.DataTokens, route.RouteHandler);
        }

        string TranslateUrl(string url, string culture)
        {
            var parts = url.Split(new char[] { '/' });
            foreach (var part in parts)
            {
                // If url contains parameters like {id}, don't try to translate them.
                // e.g. [LocalizedRoute("~/invest/{id}")]
                if (part.StartsWith("{"))
                    continue;
                
                var translatedPart = TranslatedUrls.ResourceManager.GetString(part, new System.Globalization.CultureInfo(culture));
                if (string.IsNullOrEmpty(translatedPart))
                    throw new Exception($"Could not find translation for url part {part} for culture {culture}");
                url = url.Replace(part, translatedPart);
            }
            return url;
        }

        private RouteEntry CreateLocalizedRouteEntry(string name, Route route, string explicitCulture = "")
        {
            var suffix = string.IsNullOrEmpty(explicitCulture) ? string.Empty : $"_{explicitCulture}";
            var localizedRouteEntryName = string.IsNullOrEmpty(name) ? null : $"{name}_Localized{suffix}";
            return new RouteEntry(localizedRouteEntryName, route);
        }
    }
}
