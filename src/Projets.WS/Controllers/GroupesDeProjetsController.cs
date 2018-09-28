using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Projets.Bean;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace Projets.WS.Controllers
{
    [Route("api/[controller]")]
    public class GroupesDeProjetsController
    {
        RedisManagerPool manager;
        public GroupesDeProjetsController(IConfiguration configuration)
        {
            var valuefromconf = configuration.GetValue<string>("MyConfig:RedisConnectionString");
            if (null == valuefromconf)
                throw new ArgumentNullException("Dans le appsettings.json, on doit trouver la clé suivante pour une config du server redis MyConfig:RedisConnectionString => redis:6379?db=4");
            manager = new RedisManagerPool(valuefromconf);
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (var client = manager.GetClient())
            {
                var keys = client.SearchKeys("/groupesdeprojets/[0-9]*");
                return keys;
            }
        }
        [HttpGet("{id}")]
        public BeanGroupeDeProjets Get(int id)
        {
            using (var client = manager.GetClient())
            {
                var ret = client.Get<BeanGroupeDeProjets>(
                     string.Concat("/groupesdeprojets/", id));
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
                client.Set(string.Concat("/groupesdeprojets/", value.Id), value);
            }
        }
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]BeanGroupeDeProjets value)
        {
            using (var client = manager.GetClient())
            {
                var key = string.Concat("/groupesdeprojets/", id);
                var valdb = client.Get<BeanGroupeDeProjets>(key);
                valdb.Libelle = value.Libelle;
                valdb.IsClosed = value.IsClosed;
                client.Set(key, valdb);
            }
        }
        [HttpDelete]
        public void Delete()
        {
            using (var client = manager.GetClient())
            {
                var redisBeanProjet = client.As<BeanProjet>();
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                var keys = client.SearchKeys("/groupesdeprojets/*");
                client.RemoveAll(keys);
                redisBeanProjet.SetSequence(0);
                redisBeanGroupeDeProjets.SetSequence(0);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (var client = manager.GetClient())
            {
                client.Remove(string.Concat("/groupesdeprojets/", id));
            }
        }
    }
}
