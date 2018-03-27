using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Simplified_School_Portal.Controllers
{
    public class StandardServicesController : Controller
    {
        // GET: StandardServices
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Studentenplein()
        {
            return View();
        }

        public ActionResult Canvas()
        {
            return View();
        }

        public ActionResult Lesrooster()
        {
            return View();
        }

        public ActionResult ServiceRequest()
        {
            return View();
        }
    }
}