using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Projets.Bean;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [HttpDelete]
        public void Delete()
        {
            using (var client = manager.GetClient())
            {
                var redisBeanProjet = client.As<BeanProjet>();
                var redisBeanGroupeDeProjets = client.As<BeanGroupeDeProjets>();
                redisBeanProjet.DeleteAll();
                redisBeanProjet.SetSequence(0);
                redisBeanGroupeDeProjets.DeleteAll();
                redisBeanGroupeDeProjets.SetSequence(0);
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
