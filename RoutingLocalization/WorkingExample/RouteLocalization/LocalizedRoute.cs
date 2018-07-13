using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing;

namespace WorkingExample.RouteLocalization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class LocalizedRouteAttribute : Attribute, IDirectRouteFactory, IRouteInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        public LocalizedRouteAttribute()
        {
            Template = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        /// <param name="template">The route template describing the URI pattern to match against.</param>
        public LocalizedRouteAttribute(string template, string explicitCulture = "", bool translateUrl = true)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }
            Template = template;
            ExplicitCulture = explicitCulture;
            TranslateUrl = translateUrl;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public string Template { get; private set; }

        public string ExplicitCulture { get; private set; } = "";
        public bool TranslateUrl { get; private set; } = true;

        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            Contract.Assert(context != null);

            IDirectRouteBuilder builder = context.CreateBuilder(Template);
            Contract.Assert(builder != null);

            builder.Name = Name;
            builder.Order = Order;
            var entry = builder.Build();
            return new LocalizedRouteEntry(entry.Name, entry.Route, ExplicitCulture, TranslateUrl);
        }
    }

    public class LocalizedRouteEntry : RouteEntry
    {
        public LocalizedRouteEntry(string name, System.Web.Routing.Route route, string explicitCulture, bool translateUrl)
            : base(name, route)
        {
            ExplicitCulture = explicitCulture;
            TranslateUrl = translateUrl;
        }

        public string ExplicitCulture { get; private set; } = "";
        public bool TranslateUrl { get; private set; } = true;

    }
}