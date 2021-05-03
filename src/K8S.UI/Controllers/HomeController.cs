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
    }
}
