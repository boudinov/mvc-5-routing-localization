# mvc-5-routing-localization
Simple localization and translation for both conventional and attribute routing. 

# Features

Normal `Route("~/invest")` automatically(can opt out) gets a language prefix, such as `/invest` for default language, and e.g. `/de/invest` for german language.
You can achieve the same for conventional routing.

For more control and/or url translation use LocalizeRoute("..."), so you can get `/de/investieren`, or disable route localization(opt out).

The effect is that you get Thread.CurrentThread.CurrentCulture and CurrentUICulture set appropriately during controller initialization.

# Setup

Let's say we have a couple of languages we want to support, stored in application config, accessible like his:  
ConfigurationManager.AppSettings["SupportedCultures"] -> **"en,de"**

We have a default culture like so:  
ConfigurationManager.AppSettings["DefaultCulture"] -> **"en"**'

Have a resource files for these languages containing url path segments translated, e.g. for key 'invest' have it with default value 'invest', and in german resource file as 'investieren'

Add global filter:  
```
filters.Add(new CultureFilter(ConfigurationManager.AppSettings["DefaultCulture"]);
```

# Conventional routing

```
routes.MapRoute(
  name: "DefaultWithCulture",
  url: "{culture}/{controller}/{action}/{id}",
  defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
  constraints: new { culture = new CultureConstraint(defaultCulture: ConfigurationManager.AppSettings["DefaultCulture"]) }
  );

routes.MapRoute(
	name: "Default",
	url: "{controller}/{action}/{id}",
	defaults: new { culture = DefaultCulture, controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

Do this in same order in areas route registration too.

# Attribute routing

In order for conventional and attribute routing to work together, initialize them in this order in global.asax.cs :

```
RouteTable.Routes.MapLocalizedMvcAttributeRoutes();
AreaRegistration.RegisterAllAreas();
RouteConfig.RegisterRoutes(RouteTable.Routes);
```

By default, all normal routes are localized by adding a culture prefix e.g. for
```
[Route("~/invest")]
```
you have 2 routes generated:  
~/{culture}/invest  
~/invest (this one has a default route value of {culture} -> 'en')

Default culture urls by default have no language prefix - you can change this by setting PrefixDefaultCulture in web.config to `true`, so you will have `/en/invest`.

To have more control, use **LocalizedRoute**, usually for 2 scenarios:
### 1. Translate url path
```
[LocalizedRoute("~/invest")]
ActionResult Index() { ... }
```
generates these routes:  
~/de/investieren  
~/invest

### 2. Have a route that does not get prefix-localized, e.g. to permanent-redirect old urls to new ones for SEO reasons.
For example if you need to redirect /investing to /invest. You can still opt out of url segments translation.
```
[LocalizeRoute("~/investing", explicitCulture: "en". translateUrl: false)
ActionResult Index_Old()
{
	return RedirectToActionPermanent("Index");
}
```


Ideas from post https://stackoverflow.com/questions/32764989/asp-net-mvc-5-culture-in-route-and-url
