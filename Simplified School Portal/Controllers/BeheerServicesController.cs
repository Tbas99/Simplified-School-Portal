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
        public ActionResult Overview(string searchString, string searchDay, string searchWeek)
        {
            var info_requests = from m in db.Info_request select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                info_requests = info_requests.Where(s => s.Request_user.Contains(searchString));
                ModelState.Clear();
                return View(info_requests);
            }
            else if (!String.IsNullOrEmpty(searchDay))
            {
                DateTime today = DateTime.Parse(searchDay);
                info_requests = info_requests.Where(s => s.Request_date == today);
                ModelState.Clear();
                return View(info_requests);
            }
            else if (!String.IsNullOrEmpty(searchWeek)) {
                DateTime lastweek = DateTime.Parse(searchWeek);
                info_requests = info_requests.Where(s => s.Request_date >= lastweek && s.Request_date <= DateTime.Now);
                ModelState.Clear();
                return View(info_requests);
            }
            else
            {
                ModelState.Clear();
                return View(db.Info_request.ToList());
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NewService()
        {
            return View(listCalls());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewService(APImodel m, IEnumerable<int> SelectedCalls)
        {
            if (SelectedCalls == null)
            {
                // handle it..

            }
            else
            {
                API_package package = new API_package();
                package.Package_name = m.package_name;
                package.Package_description = m.package_descr;
                package.Package_url = m.package_url;

                // Search for calls
                foreach (Package_call call in db.Package_call)
                {

                    if (SelectedCalls.Contains(call.Package_callId))
                    {
                        package.Package_call.Add(call);
                    }
                }

                // Finally add it to the db
                db.API_package.Add(package);
                db.SaveChanges();
            }

            // Cleanup
            ViewBag.Message = "Service succesfully registered.";
            ModelState.Clear();

            return View(listCalls());
        }

        [HttpGet]
        public PartialViewResult _APIcallPartial()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _APIcallPartial(Callmodel a)
        {
            Package_call call = new Package_call();
            call.Call = a.call;
            call.Call_url = a.call_url;
            call.Call_verificationNeeded = a.call_auth_needed;
            call.Call_type = a.call_type;

            db.Package_call.Add(call);
            db.SaveChanges();

            return View("NewService", listCalls());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            return View();
        }

        public APImodel listCalls()
        {
            List<SelectListItem> AllCalls = new List<SelectListItem>();

            foreach (Package_call call in db.Package_call)
            {
                SelectListItem selectList = new SelectListItem()
                {
                    Text = call.Call,
                    Value = call.Package_callId.ToString()
                };
                AllCalls.Add(selectList);
            }

            APImodel calls = new APImodel()
            {
                Calls = AllCalls
            };

            return calls;
        }
    }
}