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
using RazorEngine;
using RazorEngine.Templating;

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
        public ActionResult savePage(IEnumerable<Position> positions)
        {
            // Variable to check (highest) row position
            int highestY = 0;
            int lastY = 0;

            // First order the list
            List<Position> orderedPositions = positions.OrderBy(o => o.y).ToList();

            // Variables to calculate rows.
            int rows = 0;
            int nextRowNumber = 0;
            List<int> rowNumbers = new List<int>();
            int row = 0;
            int lastRow = 0;


            // Actual content in html
            string totalHtmlContent = "";
            string htmlContentLeft = "";
            string htmlContentRight = "";

            // htmlTemplates
            string rowContentLeft = "";
            string rowContentRight = "";
            string newRow = "";

            // Sort incoming content based on x and y positions

            // Check how many rows the page contains
            foreach (Position p in orderedPositions)
            {
                int y = Convert.ToInt16(p.y);

                if (highestY < y)
                {
                    highestY = y;
                }
            }

            // Calculate amount of rows
            if (highestY != 0)
            {
                rows = (highestY / 2) + 1; // plus 1 because y=0 exists.
            }
            else
            {
                rows = 1;
            }

            // Add y coords to each row
            for (int i = 0; i < rows; i++)
            {
                rowNumbers.Add(nextRowNumber);
                nextRowNumber += 2;
            }

            foreach (Position p in orderedPositions)
            {
                // Convert the variables first
                int y = Convert.ToInt16(p.y);
                int x = Convert.ToInt16(p.x);

                if (rowNumbers.IndexOf(y) <= lastRow)
                {
                    // Determine left or right position
                    if (x < 3)
                    {
                        // Position left
                        htmlContentLeft = p.content;
                    }
                    else if (x > 3)
                    {
                        // Position right
                        htmlContentRight = p.content;
                    }
                    else
                    {
                        // TODO: Position middle

                    }
                }
                else
                {
                    // append html row
                    rowContentLeft = "<div class=\"col-md-6\"><p>" + htmlContentLeft + "</p></div>";
                    rowContentRight = "<div class=\"col-md-6\"><p>" + htmlContentRight + "</p></div>";
                    newRow = "<div class=\"row\">" + rowContentLeft + rowContentRight + "</div>";
                    totalHtmlContent += newRow;
                    lastRow++;
                }
            }

            // Create a view with razor engine
            string template = totalHtmlContent;
            var result = Engine.Razor.RunCompile(template, "newPage", null, new { Name = "Max" });
            CustomView view = new CustomView();
            view.htmlContent = totalHtmlContent;

            return View(result);
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