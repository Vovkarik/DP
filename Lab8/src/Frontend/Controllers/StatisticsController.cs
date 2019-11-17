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
    public class StatisticsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> TextStatistics()
        {
            HttpClient client = new HttpClient();
            string backendUrl = "http://localhost:5000/";
            string getRank = backendUrl + $"api/statistics/TextStatistics";
            var response = client.GetAsync(getRank);
            var content = await response.Result.Content.ReadAsStringAsync();           
            TempData["data"] = content;
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
