using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace Projets.WS.Controllers
{
    [Route("api/[controller]")]
    public class TestRedisController : Controller
    {
        RedisManagerPool manager;
        public TestRedisController(IConfiguration configuration)
        {
            var valuefromconf = configuration.GetValue<string>("MyConfig:RedisConnectionString");
            if (null == valuefromconf)
                throw new ArgumentNullException("Dans le appsettings.json, on doit trouver la clé suivante pour une config du server redis MyConfig:RedisConnectionString => redis:6379?db=4");
            manager = new RedisManagerPool(valuefromconf);
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (var client = manager.GetClient())
            {
                client.Set("foo", "bar");
                yield return client.Get<string>("foo");
            }
        }
    }
}
