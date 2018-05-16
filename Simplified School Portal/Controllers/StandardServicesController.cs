using System;
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

namespace Simplified_School_Portal.Controllers
{
    public class StandardServicesController : Controller
    {
        private string host = "https://identity.fhict.nl/connect/authorize";
        private string lastAction = "";

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
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("https://localhost:44363/StandardServices/Callback"));
            }

            return View();
        }

        public ActionResult Canvas()
        {
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token");

            if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }

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

            return RedirectToAction(lastAction);
        }

        public async Task<ActionResult> AuthorizationCodeCallback()
        {
            // received authorization code from authorization server
            string[] codes = Request.Params.GetValues("code");
            var authorizationCode = "";
            if (codes.Length > 0)
                authorizationCode = codes[0];

            // exchange authorization code at authorization server for an access and refresh token
            Dictionary<string, string> post = null;
            post = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", authorizationCode},
                {"redirect_uri", "_redirectUrl"},
                {"client_id", "i387766-simplified"},
                {"client_secret", "Qfl45x6l38lOdiaMZE14l82RmIc3D3WG5IptSjJG"}
            };

            var client = new HttpClient();
            var postContent = new FormUrlEncodedContent(post);
            var response = await client.PostAsync("https://identity.fhict.nl/connect/token", postContent);
            var content = await response.Content.ReadAsStringAsync();

            // received tokens from authorization server
            var json = JObject.Parse(content);
            string _accessToken = json["access_token"].ToString();
            string _authorizationScheme = json["token_type"].ToString();
            string _expiresIn = json["expires_in"].ToString();
            if (json["refresh_token"] != null)
            {
                string _refreshToken = json["refresh_token"].ToString();
            }

            //SignIn with Token, SignOut and create new identity for SignIn
            Request.Headers.Add("Authorization", _authorizationScheme + " " + _accessToken);
            var ctx = Request.GetOwinContext();
            var authenticateResult = await ctx.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);
            ctx.Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            var applicationCookieIdentity = new ClaimsIdentity(authenticateResult.Identity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
            ctx.Authentication.SignIn(applicationCookieIdentity);

            var ctxUser = ctx.Authentication.User;
            var user = Request.RequestContext.HttpContext.User;

            //redirect back to the view which required authentication
            string decodedUrl = "";
            if (!string.IsNullOrEmpty("_returnUrl"))
            {
                decodedUrl = Server.UrlDecode("_returnUrl");
            }

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(decodedUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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