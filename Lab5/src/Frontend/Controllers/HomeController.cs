using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TextDetails(string id)
        {
            HttpClient client = new HttpClient();
            string backendUrl = "http://localhost:5000/";
            string getRank = backendUrl + $"api/values/rank/{id}";
            var response = client.GetAsync(getRank);
            var content = response.Result.Content.ReadAsStringAsync();
            string rank = content.Result;            
            TempData["data"] = rank;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(string data)
        {
            string id = "";
            StringContent stringContent = new StringContent("value=" + data ,Encoding.UTF8, "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            HttpResponseMessage response = await client.PostAsync("http://localhost:5000/api/values", stringContent);
            response.EnsureSuccessStatusCode();
            using (HttpContent content = response.Content) 
            { 
                id = content.ReadAsStringAsync().Result; 
            } 
            return Redirect($"/Home/TextDetails/{id}");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
