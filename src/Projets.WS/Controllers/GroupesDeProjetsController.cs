using Microsoft.AspNetCore.Mvc;
using Projets.Bean;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Projets.WS.Controllers
{
    [Route("api/[controller]")]
    public class GroupesDeProjetsController
    {
        RedisManagerPool manager = new RedisManagerPool("localhost:6379?db=4");
        //RedisManagerPool manager = new RedisManagerPool("redis:6379?db=4");
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                var alls = redisBeanGroupeDeProjets.GetAll();
                return alls.Select(a => string.Concat("/groupesdeprojets/", a.Id));
            }
        }
        [HttpGet("{id}")]
        public BeanGroupeDeProjets Get(int id)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                var ret = redisBeanGroupeDeProjets.GetById(id);
                return ret;
            }
        }
        [HttpPost]
        public void Post([FromBody]BeanGroupeDeProjets value)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                value.Id = redisBeanGroupeDeProjets.GetNextSequence();
                client.Store(value);
            }
        }
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]BeanGroupeDeProjets value)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                var valdb = redisBeanGroupeDeProjets.GetById(id);
                valdb.Libelle = value.Libelle;
                valdb.IsClosed = value.IsClosed;
                client.Store(valdb);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                redisBeanGroupeDeProjets.DeleteById(id);
            }
        }
    }
}
