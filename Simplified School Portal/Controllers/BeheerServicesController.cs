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
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simplified_School_Portal.Controllers
{
    public class BeheerServicesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private Pages activeLayout = new Pages();

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
        public ActionResult NewService(APImodel apimodel, IEnumerable<int> SelectedCalls)
        {
            if (SelectedCalls == null)
            {
                // handle it..

            }
            else
            {
                API_package package = new API_package();
                package.Package_name = apimodel.package_name;
                package.Package_description = apimodel.package_descr;
                package.Package_url = apimodel.package_url;

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

            return RedirectToAction("NewService");
        }

        public PartialViewResult _APIcallPartial()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _APIcallPartial(Callmodel callmodel)
        {
            Package_call call = new Package_call();
            call.Call = callmodel.call;
            call.Call_url = callmodel.call_url;
            call.Call_verificationNeeded = callmodel.call_auth_needed;
            call.Call_type = callmodel.call_type;
            call.Call_data_section = callmodel.call_data_section;
            call.Call_content_key = callmodel.call_content_key;

            // Finally add it to the db
            unitOfWork.Package_callRepository.Insert(call);
            unitOfWork.Save();

            return RedirectToAction("NewService");
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
        public ActionResult savePage(IEnumerable<Positionmodel> positions)
        {
            int rowcount = unitOfWork.PagesRepository.dbSet.Count();
            rowcount++;

            Pages page = new Pages();
            page.PageId = Guid.NewGuid();
            page.Title = "Page: " + rowcount.ToString();
            page.Body = extractCorrectHtmlOutput(positions);
            page.Activepage = false;
            unitOfWork.PagesRepository.Insert(page);
            unitOfWork.Save();

            return RedirectToAction("NewService");
        }

        public ActionResult SetActivePage()
        {
            List<Pages> unsortedlist = unitOfWork.PagesRepository.dbSet.ToList();
            List<Pages> sortedlist = unsortedlist.OrderBy(page => int.Parse(page.Title.Substring(6))).ToList();

            return View(sortedlist);
        }

        [HttpPost]
        public ActionResult SetActivePage(bool activePage, string pageName)
        {
            // First check if a page is already active
            bool allowedActivation = true;
            foreach (Pages page in unitOfWork.PagesRepository.dbSet.ToList())
            {
                if (page.Activepage == true)
                {
                    allowedActivation = false;
                }
            }

            foreach (Pages page in unitOfWork.PagesRepository.dbSet.ToList())
            {
                // if its not allowed to activate the page but the user still wants to activate it(acivepage = true)
                if (!allowedActivation && activePage)
                {
                    ViewBag.info = "Only 1 page is allowed to be active at the time.";
                    break;
                }

                // if active and input is to disable the page
                if (page.Title == pageName && page.Activepage != activePage)
                {
                    page.Activepage = activePage;
                    unitOfWork.PagesRepository.Update(page);
                    unitOfWork.Save();
                }
                // if not active and input is to enable the page
                else if (page.Title == pageName && page.Activepage == activePage && allowedActivation)
                {
                    page.Activepage = activePage;
                    unitOfWork.PagesRepository.Update(page);
                    unitOfWork.Save();
                }
            }

            return RedirectToAction("SetActivePage");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            List<Statisticsmodel> stats = new List<Statisticsmodel>();

            foreach (Logins login in unitOfWork.LoginsRepository.dbSet.ToList())
            {
                Statisticsmodel m = new Statisticsmodel();

                m.date = login.LoginDate.Value.Date;
                m.timestamp = login.LoginDate.Value.TimeOfDay;
                m.loginId = login.LoginsId.ToString();

                stats.Add(m);
            }

            return View(stats);
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

        public async Task<string> apiCall(string callUrl, string dataSection, string desiredContentKey)
        {
            string callResult = "";

            var client = new HttpClient();

            var response = await client.GetAsync(callUrl);
            JObject data = new JObject();
            try
            {
                data = JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return callResult;
            }

            // If user wants data from a key that isn't nested and easly accesible 
            if (dataSection == "front")
            {
                callResult = (string)data[desiredContentKey];
            }
            // if it is nested in another key, create an array and get the correct content
            else if (dataSection == "nested")
            {
                callResult = (string)data.SelectToken("data." + desiredContentKey);
            }
            return callResult;
        }

        public string startCallProcedure(string content)
        {
            string apiCallContent = "";

            // First strip the unncessary html code from the given content
            string formattedContent = formatContent(content);

            // Get the properties of the call
            foreach (Package_call call in unitOfWork.Package_callRepository.dbSet.ToList())
            {
                if (call.Call == formattedContent)
                {
                    if (call.Call_content_key.Length > 0 || call.Call_data_section.Length > 0)
                    {
                        apiCallContent = Task.Run(async () => { return await apiCall(call.Call_url, call.Call_data_section, call.Call_content_key); }).Result;
                    }
                }
            }

            if (apiCallContent == "" || apiCallContent == null)
            {
                return formattedContent;
            }
            else
            {
                return apiCallContent;
            }
        }

        public string extractCorrectHtmlOutput(IEnumerable<Positionmodel> positions)
        {
            // Variable to check (highest) row position
            int highestY = 0;

            // First order the list
            List<Positionmodel> orderedPositions = positions.OrderBy(o => o.y).ThenBy(o => o.x).ToList();

            // Determine last position
            Positionmodel lastPosition = new Positionmodel();

            // Variables to calculate rows.
            int rows = 0;
            int nextRowNumber = 0;
            List<int> rowNumbers = new List<int>();
            int lastRow = 0;


            // Actual content in html
            string totalHtmlContent = "";
            string htmlContentLeft = "";
            string htmlContentRight = "";

            // Sort incoming content based on x and y positions

            // Check how many rows the page contains
            foreach (Positionmodel p in orderedPositions)
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

            foreach (Positionmodel p in orderedPositions)
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
                        htmlContentLeft = startCallProcedure(p.content);

                    }
                    else if (x > 3)
                    {
                        // Position right
                        htmlContentRight = startCallProcedure(p.content);

                        if (Convert.ToInt16(lastPosition.x) < 3)
                        {
                            // append html row
                            totalHtmlContent += appendRow(htmlContentLeft, htmlContentRight);
                            lastRow++;

                            // clean the rows
                            htmlContentLeft = "";
                            htmlContentRight = "";
                        }
                        // If boxes are stacked on eachother to the right
                        else if (Convert.ToInt16(lastPosition.x) > 3)
                        {
                            // append html row
                            totalHtmlContent += appendRow(htmlContentLeft, htmlContentRight);
                            lastRow++;

                            // clean the rows
                            htmlContentLeft = "";
                            htmlContentRight = "";
                        }
                    }

                    // Save last position
                    lastPosition = p;
                }
                // 1 row behind
                else
                {
                    // append html row
                    totalHtmlContent += appendRow(htmlContentLeft, htmlContentRight);
                    lastRow++;

                    // clean the rows
                    htmlContentLeft = "";
                    htmlContentRight = "";

                    // Determine left or right position
                    if (x < 3)
                    {
                        // Position left
                        htmlContentLeft = startCallProcedure(p.content);

                        // append html row
                        totalHtmlContent += appendRow(htmlContentLeft, htmlContentRight);
                        lastRow++;

                        // clean the rows
                        htmlContentLeft = "";
                        htmlContentRight = "";
                    }
                    else if (x > 3)
                    {
                        // Position right
                        htmlContentRight = startCallProcedure(p.content);

                        // append html row
                        totalHtmlContent += appendRow(htmlContentLeft, htmlContentRight);
                        lastRow++;

                        // clean the rows
                        htmlContentLeft = "";
                        htmlContentRight = "";
                    }

                    // Save last position
                    lastPosition = p;
                }
            }

            return totalHtmlContent;
        }

        // Functions to handle page saving
        private string formatContent(string content)
        {
            string formattedContent = content.Replace("\n", "").Replace("<div>", "").Replace("</div>", "");

            return formattedContent;
        }

        private string appendRow(string leftPageContent, string rightPageContent)
        {
            string rowContentLeft = "<div class=\"col-md-5 dynamicBlock\"><p>" + leftPageContent + "</p></div>";
            string rowContentRight = "<div class=\"col-md-5 col-md-offset-2 dynamicBlock\"><p>" + rightPageContent + "</p></div>";
            string totalRowContent = "<div class=\"row\">" + rowContentLeft + rowContentRight + "</div>";

            return totalRowContent;
        }
    }
}