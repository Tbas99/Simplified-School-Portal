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

namespace Simplified_School_Portal.Controllers
{
    public class StandardServicesController : Controller
    {
        private string host = "https://identity.fhict.nl/connect/authorize";

        // GET: StandardServices
        public async Task<ActionResult> Index()
        {
            ViewData["Title"] = "Home page";

            // First, see if the user is already connected
            var authToken = GetAuthTokenFromSession();

            // If the authorisation process returns a code, exchange it for an access token
            string currentUrl = Request.ApplicationPath;

            if (currentUrl.Contains("code"))
            {
                try
                {
                    var code = Request.QueryString["code"];
                    authToken = await GetTokenFromCode(code);
                    ViewData["AccessToken"] = authToken;

                    var user = GetUserInfo(authToken);
                    ViewData["Username"] = user.Result["name"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            // If the user isn't connected, redirect to the authorisation page
            if (string.IsNullOrEmpty(authToken))
            {
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("http://i387766.venus.fhict.nl/StandardServices/Studentenplein"));
            }

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

        // Code that transforms the given "code" into an access token
        private async Task<string> GetTokenFromCode(string code)
        {
            // Make a POST request
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://i387766.venus.fhict.nl/StandardServices/Studentenplein"),
                new KeyValuePair<string, string>("client_id", "i387766-simplified"),
                new KeyValuePair<string, string>("client_secret", "Qfl45x6l38lOdiaMZE14l82RmIc3D3WG5IptSjJG")
            });

            /* Extra auth doesn't seem to be needed on this POST call
            var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("6220730c-d0e4-4fde-af3d-5ffgdca94e22:BC2GlOXcBXof56PSR8CA0TB6tHdlj3DLPEQ8hwf87kI"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            */

            // The actual POST request
            var response = await client.PostAsync("https://identity.fhict.nl/connect/token", content);
            var token = JObject.Parse(await response.Content.ReadAsStringAsync())["access_token"].ToString();

            // Finally return the token
            return token;
        }

        // Receive some basic information
        private async Task<JObject> GetUserInfo(string accessToken)
        {
            // Set up a GET call
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // The actual GET call
            var response = await client.GetAsync("https://identity.fhict.nl/connect/userinfo");
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private string GetAuthTokenFromSession()
        {
            //TODO: Session handeling
            return "";
        }
    }
}