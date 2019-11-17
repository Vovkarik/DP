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
    public class ValuesController : Controller
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            IDatabase db = redis.GetDatabase();
            string value = db.StringGet(id);
            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromForm]string value)
        {
            IDatabase db = redis.GetDatabase();
            var id = Guid.NewGuid().ToString();
            db.StringSet(id, value);
            ISubscriber sub = redis.GetSubscriber();
            sub.Publish("events", "TextCreated: " + id);
            return id;
        }

        [HttpGet("rank/{id?}")]
        public IActionResult GetRank(string id)
        {
            string value = null;
            int countOfTries = 5;
            while(countOfTries != 0)
            {
                IDatabase db = redis.GetDatabase();
                value = db.StringGet(id);
                if(value != null)
                { 
                    break;                    
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                    countOfTries -= 1;
                }
            }
            if(value != null)
            {
                return Ok(value);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
