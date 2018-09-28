using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using System.Collections.Generic;

namespace Projets.WS.Controllers
{
    [Route("api/[controller]")]
    public class TestRedisController : Controller
    {
        //RedisManagerPool manager = new RedisManagerPool("localhost:6379?db=4");
        RedisManagerPool manager = new RedisManagerPool("redis:6379?db=4");
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
