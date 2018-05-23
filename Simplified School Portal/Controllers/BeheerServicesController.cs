using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simplified_School_Portal.Models;
using Simplified_School_Portal.DAL;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace Simplified_School_Portal.Controllers
{
    public class BeheerServicesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

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
            var info_requests = from m in unitOfWork.Info_requestRepository.dbSet.ToList() select m;

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
                return View(unitOfWork.Info_requestRepository.dbSet.ToList());
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NewService()
        {
            // Pass model for partial view to viewdata:
            ViewData["partialModel"] = unitOfWork.Api_packageRepository.dbSet.ToList();

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
                foreach (Package_call call in unitOfWork.Package_callRepository.dbSet.ToList())
                {

                    if (SelectedCalls.Contains(call.Package_callId))
                    {
                        package.Package_call.Add(call);
                    }
                }

                // Finally add it to the db
                unitOfWork.Api_packageRepository.Insert(package);
                unitOfWork.Save();
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

            // Finally add it to the db
            unitOfWork.Package_callRepository.Insert(call);
            unitOfWork.Save();

            return View("NewService", listCalls());
        }

        [HttpGet]
        public ActionResult _CreatePagePartial()
        {
            List<API_package> packages = new List<API_package>();

            foreach (API_package package in unitOfWork.Api_packageRepository.dbSet.ToList())
            {
                packages.Add(package);
            }

            return PartialView(packages);
        }

        [HttpPost]
        public string savePage(string[] positions)
        {
            //some code
            var json = positions;
            var test = "";
            CreatePagemodelJson obj = new JavaScriptSerializer().Deserialize<CreatePagemodelJson>(positions.ToString());
            //var jsonObject = JsonConvert.DeserializeObject<CreatePagemodel>(positionsJson.ToString());

            return "";
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            return View();
        }

        public APImodel listCalls()
        {
            List<SelectListItem> AllCalls = new List<SelectListItem>();

            foreach (Package_call call in unitOfWork.Package_callRepository.dbSet.ToList())
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