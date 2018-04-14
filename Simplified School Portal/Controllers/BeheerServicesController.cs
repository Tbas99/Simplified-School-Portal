using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simplified_School_Portal.Models;
using System.Text;

namespace Simplified_School_Portal.Controllers
{
    public class BeheerServicesController : Controller
    {
        private SSPDatabaseEntities db = new SSPDatabaseEntities();

        // GET: BeheerServices
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Overview()
        {
            return View(db.Info_request.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NewService()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            return View();
        }
    }
}