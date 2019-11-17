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
            sub.Publish("values", "TextCreated: "+id);
            return id;
        }
    }
}
