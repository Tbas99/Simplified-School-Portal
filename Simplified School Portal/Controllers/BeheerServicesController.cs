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
        public ActionResult Overview(string searchString)
        {
            var info_requests = from m in db.Info_request select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                info_requests = info_requests.Where(s => s.Request_user.Contains(searchString));
                return View(info_requests);
            }
            else
            {
                return View(db.Info_request.ToList());
            }
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