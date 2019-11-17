using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        
        [HttpGet("{TextStatistics}")]
        public IActionResult Get()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase(1);
            string statistics = db.StringGet("TextStatistics");
            return Ok(statistics);
        }

    }
}
