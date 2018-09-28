using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Projets.Bean;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projets.WS.Controllers
{
    [Route("api/GroupesDeProjets/{idgrp}/[controller]")]
    public class ProjetsController
    {
        RedisManagerPool manager;
        public ProjetsController(IConfiguration configuration)
        {
            var valuefromconf = configuration.GetValue<string>("MyConfig:RedisConnectionString");
            if (null == valuefromconf)
                throw new ArgumentNullException("Dans le appsettings.json, on doit trouver la clé suivante pour une config du server redis MyConfig:RedisConnectionString => redis:6379?db=4");
            manager = new RedisManagerPool(valuefromconf);
        }
        [HttpGet]
        public IEnumerable<string> Get(int idgrp)
        {
            using (var client = manager.GetClient())
            {
                var redisProjet = client.As<BeanProjet>();
                var alls = redisProjet.GetAll();
                return alls.Select(a => string.Concat("/groupesdeprojets/", idgrp, "/projets/", a.Id));
            }
        }
        [HttpGet("{id}")]
        public BeanProjet Get(int idgrp, int id)
        {
            using (var client = manager.GetClient())
            {
                var redisProjet = client.As<BeanProjet>();
                var ret = redisProjet.GetById(id);
                return ret;
            }
        }
        [HttpPost]
        public void Post(int idgrp, [FromBody]BeanProjet value)
        {
            using (var client = manager.GetClient())
            {
                var redisProjet = client.As<BeanProjet>();
                var valDb = new BeanProjet
                {
                    Id = redisProjet.GetNextSequence(),
                    Groupe  = string.Concat("/groupesdeprojets/", idgrp),
                    Libelle = value.Libelle,
                    IsClosed = value.IsClosed,
                };
                client.Store(valDb);
            }
        }
        [HttpPut("{id}")]
        public void Put(int idgrp, int id, [FromBody]BeanProjet value)
        {
            using (var client = manager.GetClient())
            {
                var redisProjet = client.As<BeanProjet>();
                var valdb = redisProjet.GetById(id);
                valdb.Libelle = value.Libelle;
                valdb.IsClosed = value.IsClosed;
                client.Store(valdb);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(int idgrp, int id)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanGroupeDeProjets = client.As<BeanProjet>();
                redisBeanGroupeDeProjets.DeleteById(id);
            }
        }
    }
}
