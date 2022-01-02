using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using K8S.UI.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;

namespace K8S.UI.Controllers
{
    public class HomeController : Controller
    {
        string baseUrl = "http://backend-service.default.svc.cluster.local:8080/";
        //string baseUrl = "http://localhost:4445/";

        public async Task<IActionResult> Index()
        {
            ViewBag.HostName = Environment.MachineName;

            List<string> response = await CallAPI();

            ViewData["API_RESPONSE"] = response;

            bool isQueued = ThreadPool.QueueUserWorkItem(delegate (object o) { Console.WriteLine("emre"); });

            return View();
        }

        public IActionResult Login()
        {
            if (Request.Cookies["username"] != null)
            {
                ViewData["UserName"] = Request.Cookies["username"];
            }

            return View();
        }


        private async Task<List<string>> CallAPI()
        {
            List<string> result = new List<string>();

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/values");

                if (Res.IsSuccessStatusCode)
                {
                    var apiResponse = Res.Content.ReadAsStringAsync().Result;

                    result = JsonConvert.DeserializeObject<List<string>>(apiResponse);

                }
            }

            return result;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Erol()
        {
            string clientSecret = "4f04915674530708a9f819c15029b6d5d633951d";
            string clientID = "e15e3dfe4c4ba33ea4d7";

            var queryString = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(Request.QueryString.Value);

            if (queryString.ContainsKey("code"))
            {
                string code = queryString["code"];

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://github.com");

                    httpClient.DefaultRequestHeaders.Clear();

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var post = new { code = code, client_id = clientID, client_secret = clientSecret, redirect_uri = "http://emrekantar.com.tr:32540/Home/Erol" };

                    var tokenResponse = new { access_token = "", scope = "", token_type = "" };

                    HttpResponseMessage postResult = httpClient.PostAsJsonAsync("/login/oauth/access_token", post).Result;

                    string result = postResult.Content.ReadAsStringAsync().Result;

                    tokenResponse = JsonConvert.DeserializeAnonymousType(result, tokenResponse);

                    using (var httpClient2 = new HttpClient())
                    {
                        httpClient2.BaseAddress = new Uri("https://api.github.com");

                        httpClient2.DefaultRequestHeaders.Clear();

                        httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var tokenResponse2 = new { avatar_url = "", name = "" };

                        httpClient2.DefaultRequestHeaders.Add("Authorization", "token " + tokenResponse.access_token);
                        httpClient2.DefaultRequestHeaders.Add("user-agent", "emre kantar");

                        HttpResponseMessage getUserInfoResult = await httpClient2.GetAsync("/user");

                        string result2 = getUserInfoResult.Content.ReadAsStringAsync().Result;

                        Console.Write("result2 :" + result2);

                        tokenResponse2 = JsonConvert.DeserializeAnonymousType(result2, tokenResponse2);

                        HttpContext.Response.Cookies.Append("username", tokenResponse2.name, new Microsoft.AspNetCore.Http.CookieOptions() { Path = "/", IsEssential = true, Expires = DateTimeOffset.Now.AddHours(2) });
                    }
                }
            }
            return View("Egemen");

        }
    }
}
