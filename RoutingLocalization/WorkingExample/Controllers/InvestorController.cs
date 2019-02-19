using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkingExample.RouteLocalization;

namespace WorkingExample.Controllers
{
    public class InvestorController : Controller
    {
        //for german gets translated to /de/investieren
        [LocalizedRoute("~/invest")]
        public ActionResult Index()
        {
            return View();
        }
    
        //example how to handle old urls in respect to SEO - redirect old /investing to new /invest url
        [LocalizedRoute("~/investing", explicitCulture: "en", translateUrl: false)]
        public ActionResult Index_OLD_EN()
        {
            return RedirectToActionPermanent("Index");
        }
    }
}
