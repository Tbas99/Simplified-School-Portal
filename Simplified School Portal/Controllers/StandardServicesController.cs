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

            var authToken = GetAuthTokenFromSession();

            // https://developer.mypurecloud.com/api/tutorials/oauth-auth-code/#csharp
            /*
            if (Request.Query)
            {
                try
                {
                    var code = Request.Query["code"];
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
            */

            if (string.IsNullOrEmpty(authToken))
            {
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("http://i387766.venus.fhict.nl/StandardServices/Studentenplein"));

            }

            string currentUrl = Request.ApplicationPath;

            if (currentUrl.Contains("code"))
            {
                try
                {
                    var code = Request.QueryString["code"];
                    authToken = await GetTokenFromCode(code);
                    ViewData["AccessToken"] = authToken;
                }
                catch
                {

                }
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

        private async Task<string> GetTokenFromCode(string code)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://i387766.venus.fhict.nl/StandardServices/Studentenplein")
            });
            var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("6220730c-d0e4-4fde-af3d-5ffgdca94e22:BC2GlOXcBXof56PSR8CA0TB6tHdlj3DLPEQ8hwf87kI"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            var response = await client.PostAsync("https://login." + host + "/oauth/token", content);
            var token = JObject.Parse(await response.Content.ReadAsStringAsync())["access_token"].ToString();
            return token;
        }

        private async Task<JObject> GetUserInfo(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://api." + host + "/api/v2/users/me");
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private string GetAuthTokenFromSession()
        {
            //TODO: ASP.NET 5 MVC 6 preview does not have native sessions. Implement your own session handling here
            return "";
        }
    }
}