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

        // GET: StandardServices
        public ActionResult Index()
        {            
            return View();
        }

        public async Task<ActionResult> Studentenplein()
        {
            /*
            ViewData["Title"] = "Home page";

            // First, see if the user is already connected
            var authToken = GetAuthTokenFromSession();

            // If the authorisation process returns a code, exchange it for an access token
            string currentUrl = Request.Url.ToString();

            if (currentUrl.Contains("access_token"))
            {
                try
                {
                    var code = Request.QueryString["code"];
                    //authToken = await GetTokenFromCode(code);
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
                return Redirect(host + "?client_id=i387766-simplified&scope=fhict%20fhict_personal%20openid%20profile%20email%20roles&response_type=token&redirect_uri=" + HttpUtility.HtmlEncode("http://localhost:54680/StandardServices/Studentenplein"));
            }
            else
            {
                ViewData["token"] = GetAuthTokenFromSession();
            }

            */
            // Set up a GET call
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyIsImtpZCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyJ9.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LmZoaWN0Lm5sIiwiYXVkIjoiaHR0cHM6Ly9pZGVudGl0eS5maGljdC5ubC9yZXNvdXJjZXMiLCJleHAiOjE1MjU3MTk5ODYsIm5iZiI6MTUyNTcxMjc4NiwiY2xpZW50X2lkIjoiYXBpLWNsaWVudCIsInVybjpubC5maGljdDp0cnVzdGVkX2NsaWVudCI6InRydWUiLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZW1haWwiLCJmaGljdCIsImZoaWN0X3BlcnNvbmFsIiwiZmhpY3RfbG9jYXRpb24iXSwic3ViIjoiYWFhYTQ3ZGEtZGU2OS00YmMwLTk0NjktMDA4NDU2MTI4YTZiIiwiYXV0aF90aW1lIjoxNTI1NzEyNzg2LCJpZHAiOiJmaGljdC1zc28iLCJyb2xlIjpbInVzZXIiLCJzdHVkZW50Il0sInVwbiI6IkkzODc3NjZAZmhpY3QubmwiLCJuYW1lIjoiU2FnaXMsVG9iaWFzIFQuRy5NLiIsImVtYWlsIjoidC5zYWdpc0BzdHVkZW50LmZvbnR5cy5ubCIsInVybjpubC5maGljdDpzY2hlZHVsZSI6ImNsYXNzfFMyMiIsImZvbnR5c191cG4iOiIzODc3NjZAc3R1ZGVudC5mb250eXMubmwiLCJhbXIiOlsiZXh0ZXJuYWwiXX0.WnwZL7iQZesUrCJdId9c9wQAXBxEKVx-Aj6lG0IhJk6GuCtvUBsl7F-vVKNzdx9CNkEE6B9QP13lVhadDCQnlZu8kKgLut31UqW4zmI12gBcpgLiuQxlNNtciNPkIUuVVFhdMeq8XDfnTqIGD1TUyP9zNq5e0NgfRZss9TPlFt0yt4eOcXBx6oN-4qrFeiIVVtDHT8gQXkqpECK-9YxCvxrFhk1K-c8a2lqM_MbxA2ibx-MnoMTdg-TQmQQ2hhJxuQ9IkDya5-MzF4BphwDJhBZruYhfujII995BvQwm6pADF_wOPXtEr3-LJvyAR3SGBcVJ6q6X_yRPKgd69sBPpQ");

            // The actual GET call
            var response = await client.GetAsync("https://api.fhict.nl/schedule/me?days=7");
            var data = JObject.Parse(await response.Content.ReadAsStringAsync());

            var title = (string)data["title"];

            ViewData["data"] = data;
            ViewData["title"] = title;


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

        public async Task<ActionResult> Lesrooster()
        {
            int loopcount = 0;
            List<Course> courses = new List<Course>();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyIsImtpZCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyJ9.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LmZoaWN0Lm5sIiwiYXVkIjoiaHR0cHM6Ly9pZGVudGl0eS5maGljdC5ubC9yZXNvdXJjZXMiLCJleHAiOjE1MjU3MTk5ODYsIm5iZiI6MTUyNTcxMjc4NiwiY2xpZW50X2lkIjoiYXBpLWNsaWVudCIsInVybjpubC5maGljdDp0cnVzdGVkX2NsaWVudCI6InRydWUiLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZW1haWwiLCJmaGljdCIsImZoaWN0X3BlcnNvbmFsIiwiZmhpY3RfbG9jYXRpb24iXSwic3ViIjoiYWFhYTQ3ZGEtZGU2OS00YmMwLTk0NjktMDA4NDU2MTI4YTZiIiwiYXV0aF90aW1lIjoxNTI1NzEyNzg2LCJpZHAiOiJmaGljdC1zc28iLCJyb2xlIjpbInVzZXIiLCJzdHVkZW50Il0sInVwbiI6IkkzODc3NjZAZmhpY3QubmwiLCJuYW1lIjoiU2FnaXMsVG9iaWFzIFQuRy5NLiIsImVtYWlsIjoidC5zYWdpc0BzdHVkZW50LmZvbnR5cy5ubCIsInVybjpubC5maGljdDpzY2hlZHVsZSI6ImNsYXNzfFMyMiIsImZvbnR5c191cG4iOiIzODc3NjZAc3R1ZGVudC5mb250eXMubmwiLCJhbXIiOlsiZXh0ZXJuYWwiXX0.WnwZL7iQZesUrCJdId9c9wQAXBxEKVx-Aj6lG0IhJk6GuCtvUBsl7F-vVKNzdx9CNkEE6B9QP13lVhadDCQnlZu8kKgLut31UqW4zmI12gBcpgLiuQxlNNtciNPkIUuVVFhdMeq8XDfnTqIGD1TUyP9zNq5e0NgfRZss9TPlFt0yt4eOcXBx6oN-4qrFeiIVVtDHT8gQXkqpECK-9YxCvxrFhk1K-c8a2lqM_MbxA2ibx-MnoMTdg-TQmQQ2hhJxuQ9IkDya5-MzF4BphwDJhBZruYhfujII995BvQwm6pADF_wOPXtEr3-LJvyAR3SGBcVJ6q6X_yRPKgd69sBPpQ");

            // The actual GET call
            var response = await client.GetAsync("https://api.fhict.nl/schedule/me?days=7");
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
            var response = await client.GetAsync("https://api.fhict.nl/schedule/me");
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private string GetAuthTokenFromSession()
        {
            //TODO: Session handeling

            HttpCookie myCookie = new HttpCookie("token");
            myCookie.Value = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyIsImtpZCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyJ9.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LmZoaWN0Lm5sIiwiYXVkIjoiaHR0cHM6Ly9pZGVudGl0eS5maGljdC5ubC9yZXNvdXJjZXMiLCJleHAiOjE1MjU3MDA2NjksIm5iZiI6MTUyNTY5MzQ2OSwiY2xpZW50X2lkIjoiYXBpLWNsaWVudCIsInVybjpubC5maGljdDp0cnVzdGVkX2NsaWVudCI6InRydWUiLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZW1haWwiLCJmaGljdCIsImZoaWN0X3BlcnNvbmFsIiwiZmhpY3RfbG9jYXRpb24iXSwic3ViIjoiYWFhYTQ3ZGEtZGU2OS00YmMwLTk0NjktMDA4NDU2MTI4YTZiIiwiYXV0aF90aW1lIjoxNTI1NjkxNzIwLCJpZHAiOiJmaGljdC1zc28iLCJyb2xlIjpbInVzZXIiLCJzdHVkZW50Il0sInVwbiI6IkkzODc3NjZAZmhpY3QubmwiLCJuYW1lIjoiU2FnaXMsVG9iaWFzIFQuRy5NLiIsImVtYWlsIjoidC5zYWdpc0BzdHVkZW50LmZvbnR5cy5ubCIsInVybjpubC5maGljdDpzY2hlZHVsZSI6ImNsYXNzfFMyMiIsImZvbnR5c191cG4iOiIzODc3NjZAc3R1ZGVudC5mb250eXMubmwiLCJhbXIiOlsiZXh0ZXJuYWwiXX0.bDSbv-HZeOvcYeiNu82YCrKuG4F70v8jfDzlsxxMOrrTJsnQoahoWWH-Mop_gFWOkZ1B8AxNYKEyBdXRis3smaFx_Ni1quJt5nUuKXu4DgMqVusxK25DIsFRf07d8Ttu1omQZmRWu5Pl9RSBTRnPGomPurznDr0Q1SyHYEa-cY3NFfPO7A4CcdWryzgEeDEt4ibLq6NxUW0dPdXMRLCq0QeC1iDqbGV3xei7h9KlYG6tsbBuPF6IKJc1vURWIZyY310MMvMSSxAMzyBGmksjxyVoSowkc4Mf6KIvLc8sVtC4fFVUCDoGk5JN1T0EDGhGtMxOeQLxP9eFn2tm_7BKqQ";
            Response.Cookies.Add(myCookie);

            if (myCookie.Value != null)
            {
                return myCookie.Value;
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