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

namespace Simplified_School_Portal.Controllers
{
    public class StandardServicesController : Controller
    {
        private string host = "https://identity.fhict.nl/connect/authorize";

        // GET: StandardServices
        public ActionResult Index()
        {            
            return View();
        }

        public async Task<ActionResult> Studentenplein()
        {
            ViewData["Title"] = "Home page";

            // First, see if the user is already connected
            var authToken = GetAuthTokenFromSession();

            // If the authorisation process returns a code, exchange it for an access token
            string currentUrl = Request.Url.ToString();

            if (currentUrl.Contains("code"))
            {
                try
                {
                    var code = Request.QueryString["code"];
                    authToken = await GetTokenFromCode(code);
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
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=" + HttpUtility.HtmlEncode("http://localhost:54680/StandardServices/Studentenplein"));
            }
            else
            {
                ViewData["token"] = GetAuthTokenFromSession();
            }

            return View();
        }

        [Authorize]
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

        [Authorize]
        public ActionResult Lesrooster()
        {
            return View();
        }

        public ActionResult ServiceRequest()
        {
            return View();
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
                new KeyValuePair<string, string>("redirect_uri", "http://localhost:54680/StandardServices/Studentenplein"),
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

            // Store the token in a cookie
            var expirationTime = JObject.Parse(await response.Content.ReadAsStringAsync())["expires_in"].ToString();

            HttpCookie cookie = new HttpCookie("token");
            cookie.Value = token.ToString();
            //cookie.Expires = DateTime.Now.AddMinutes(Double.Parse(expirationTime));

            Response.Cookies.Add(cookie);

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

            HttpCookie myCookie = Response.Cookies["token"];

            if (myCookie.Value != null)
            {
                return myCookie.Value;
            }
            else
            {
                return "";
            }
        }
    }
}