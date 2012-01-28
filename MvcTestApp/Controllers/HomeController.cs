using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using DynamicCsToPowerShell;
using DynamicSugar;

namespace MvcTestApp.Controllers {

    public class HomeController : Dynamic4MVC.ControllerExtended {

        public ActionResult Index() {
    
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }
        public ActionResult About() {

            return View();
        }
    }
}
