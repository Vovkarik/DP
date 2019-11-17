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
            int databaseNumber = 0;
            string[] serializedValue = value.Split(":");
            switch(serializedValue[1])
            {
                case "RUS":
                    databaseNumber = 1;
                    break;
                case "EU":
                    databaseNumber = 2;
                    break;
                case "USA":
                    databaseNumber = 3;
                    break;
            }
            IDatabase db = redis.GetDatabase(databaseNumber);
            var id = Guid.NewGuid().ToString();
            db.StringSet(id, serializedValue[0]);
            ISubscriber sub = redis.GetSubscriber();
            IDatabase map = redis.GetDatabase(0);
            map.StringSet(id, databaseNumber);
            sub.Publish("events", "Text Created : " + id);
            return id;
        }

        [HttpGet("rank/{id}:{region}")]
        public IActionResult GetRank(string id, string region)
        {
            IDatabase map = redis.GetDatabase(0);
            string value = null;
            int countOfTries = 5;
            string databaseNumberString = map.StringGet(id);
            int databaseNumber = Convert.ToInt32(databaseNumberString);
            while(countOfTries != 0)
            {
                IDatabase db = redis.GetDatabase(databaseNumber);
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
