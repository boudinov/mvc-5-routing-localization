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
        //redirect /investing to /invest
        [LocalizedRoute("~/investing", explicitCulture: "en", translateUrl: false)]
        public ActionResult Index_OLD_EN()
        {
            return RedirectToActionPermanent("Index");
        }


        [LocalizedRoute("~/invest")]
        public ActionResult Index()
        {
            return View();
        }
    }
}