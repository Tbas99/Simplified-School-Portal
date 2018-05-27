﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet;
using Microsoft.IdentityModel;
using System.IdentityModel.Claims;
using System.IdentityModel;
using IdentityServer3.Core.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Simplified_School_Portal.Models;
using Simplified_School_Portal.DAL;

namespace Simplified_School_Portal.Controllers
{
    public class StandardServicesController : Controller
    {
        private string host = "https://identity.fhict.nl/connect/authorize";
        private string canvasHost = "https://fhict.instructure.com/login/oauth2/auth";
        private static string lastAction = "";

        // GET: StandardServices
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Studentenplein()
        {
            // First, see if the user is already connected
            var authToken = GetAuthTokenFromSession();

            // If the user isn't connected, redirect to the authorisation page
            if (string.IsNullOrEmpty(authToken))
            {
                lastAction = "Studentenplein";
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("https://localhost:44363/StandardServices/Callback"));
            }

            return View();
        }

        public async Task<ActionResult> Canvas()
        {
            /*
            // First, see if the user is already connected
            var authToken = GetCanvasAuthTokenFromSession();

            // If the user isn't connected, redirect to the authorisation page
            if (string.IsNullOrEmpty(authToken))
            {
                lastAction = "Canvas";
                return Redirect(canvasHost + "?client_id=i387766-simplified&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("https://localhost:44363/StandardServices/Callback"));
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // The actual GET call
            var response = await client.GetAsync("https://api.fhict.nl/canvas/courses/me");
            var data = JObject.Parse(await response.Content.ReadAsStringAsync());
            var dataArray = (JArray)data["data"];
            */

            return View();
        }

        public async Task<ActionResult> Lesrooster()
        {
            // First, see if the user is already connected
            var authToken = GetAuthTokenFromSession();

            // If the user isn't connected, redirect to the authorisation page
            if (string.IsNullOrEmpty(authToken))
            {
                lastAction = "Lesrooster";
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("https://localhost:44363/StandardServices/Callback"));
            }

            List<Course> courses = new List<Course>();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // The actual GET call
            var response = await client.GetAsync("https://api.fhict.nl/schedule/me?days=7&startLastMonday=true");
            var data = JObject.Parse(await response.Content.ReadAsStringAsync());
            var dataArray = (JArray)data["data"];

            var title = (string)data["title"];

            ViewData["data"] = data;
            ViewData["title"] = title;

            foreach (JObject date in dataArray)
            {
                Course course = new Course();

                // add all properties to fill the model
                foreach (var property in date.Properties())
                {
                    // add each property to their designated model value
                    switch (property.Name)
                    {
                        case "room":
                            course.room = (string)property.Value;
                            break;
                        case "subject":
                            course.subject = (string)property.Value;
                            break;
                        case "teacherAbbreviation":
                            course.teacher = (string)property.Value;
                            break;
                        case "start":
                            course.startTime = extractCorrectOutput((string)property.Value, "Time");
                            course.startDate = extractCorrectOutput((string)property.Value, "Date");
                            break;
                        case "end":
                            course.endTime = extractCorrectOutput((string)property.Value, "Time");
                            course.endDate = extractCorrectOutput((string)property.Value, "Date");
                            break;
                        case "description":
                            course.description = (string)property.Value;
                            break;
                        default:
                            break;
                    }
                }

                // Finally, add the course to the list
                courses.Add(course);
            }

            return View(courses);
        }

        public ActionResult ServiceRequest()
        {
            return View();
        }

        public async Task<ActionResult> Callback()
        {
            // If the authorisation process returns a code, exchange it for an access token
            string currentUrl = Request.Url.ToString();

            if (currentUrl.Contains("code"))
            {
                try
                {
                    var code = Request.QueryString["code"];
                    var authToken = await GetTokenFromCode(code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return RedirectToAction(lastAction, "StandardServices");
        }

        // Code that transforms the given "code" into an access token
        private async Task<string> GetTokenFromCode(string code)
        {
            // Make a POST request
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "https://localhost:44363/StandardServices/Callback"),
                new KeyValuePair<string, string>("client_id", "i387766-simplified"),
                new KeyValuePair<string, string>("client_secret", "Qfl45x6l38lOdiaMZE14l82RmIc3D3WG5IptSjJG")
            });

            // The actual POST request
            var response = await client.PostAsync("https://identity.fhict.nl/connect/token", content);
            var token = JObject.Parse(await response.Content.ReadAsStringAsync())["access_token"].ToString();
            var expirationTime = JObject.Parse(await response.Content.ReadAsStringAsync())["expires_in"].ToString();

            // Store the token in a cookie
            DateTime expiredate = DateTime.Now.AddMinutes(double.Parse(expirationTime));

            HttpCookie cookie = new HttpCookie("token");
            cookie.Value = token.ToString();
            cookie.Expires = expiredate;

            Response.Cookies.Add(cookie);

            // Add userinfo to another cookie
            await GetUserInfo(token, expiredate);

            // Finally return the token
            return token;
        }

        // Receive some basic information
        private async Task GetUserInfo(string accessToken, DateTime expirationDate)
        {
            // Set up a GET call
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // The actual GET call
            var response = await client.GetAsync("https://identity.fhict.nl/connect/userinfo");

            // Parse json and store it in a cookie
            var name = JObject.Parse(await response.Content.ReadAsStringAsync())["name"].ToString();
            HttpCookie usercookie = new HttpCookie("usercookie");
            usercookie.Value = name;
            usercookie.Expires = expirationDate;

            Response.Cookies.Add(usercookie);

            return;
        }


        private string GetAuthTokenFromSession()
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains("token"))
            {
                HttpCookie cookie = HttpContext.Request.Cookies["token"];
                return cookie.Value;
            }
            else
            {
                return "";
            }
        }

        private string GetCanvasAuthTokenFromSession()
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains("canvasToken"))
            {
                HttpCookie cookie = HttpContext.Request.Cookies["canvasToken"];
                return cookie.Value;
            }
            else
            {
                return "";
            }
        }

        public RedirectToRouteResult log_out()
        {
            // Check for usercookie value, if it has value, also delete access token.
            // No second if statement for access token, because usercookie wouldn't exist without access token.
            if (Request.Cookies["usercookie"].Value != null)
            {
                Response.Cookies["usercookie"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["token"].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Index", "Home");
        }

        // function to extract date-time from a single string provided by the API.
        private string extractCorrectOutput(string unformattedTimeDate, string desiredOutput)
        {
            string dateTime = unformattedTimeDate;
            string[] seperateDateTime = dateTime.Split(' ');

            string correctOutput = "";

            switch (desiredOutput)
            {
                case "Date":
                    correctOutput = seperateDateTime[0];
                    break;
                case "Time":
                    correctOutput = seperateDateTime[1];
                    break;
                default:
                    break;
            }

            return correctOutput;
        }
    }
}