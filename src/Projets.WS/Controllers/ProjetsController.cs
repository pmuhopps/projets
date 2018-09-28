using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Projets.Bean;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

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
                var keys = client.SearchKeys(string.Concat("/groupesdeprojets/", idgrp, "/projets/[0-9]*"));
                return keys;
            }
        }

        [HttpGet("{id}")]
        public BeanProjet Get(int idgrp, int id)
        {
            using (var client = manager.GetClient())
            {
                var ret = client.Get<BeanProjet>(
                     string.Concat("/groupesdeprojets/", idgrp, "/projets/", id));
                return ret;
            }
        }
        [HttpPost]
        public void Post(int idgrp, [FromBody]BeanProjet value)
        {
            using (var client = manager.GetClient())
            {
                var redisBeanProjet = client.As<BeanProjet>();
                value.Id = redisBeanProjet.GetNextSequence();
                value.Groupe = string.Concat("/groupesdeprojets/", idgrp);
                client.Set(string.Concat("/groupesdeprojets/", idgrp, "/projets/", value.Id), value);
            }
        }
        [HttpPut("{id}")]
        public void Put(int idgrp, int id, [FromBody]BeanProjet value)
        {
            using (var client = manager.GetClient())
            {
                var key = string.Concat("/groupesdeprojets/", idgrp, "/projets/", id);
                var valdb = client.Get<BeanProjet>(key);
                valdb.Libelle = value.Libelle;
                valdb.IsClosed = value.IsClosed;
                client.Set(key, valdb);
            }
        }
        [HttpDelete]
        public void Delete(int idgrp)
        {
            using (var client = manager.GetClient())
            {
                var keys = client.SearchKeys(string.Concat("/groupesdeprojets/", idgrp, "/projets/[0-9]*"));
                client.RemoveAll(keys);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(int idgrp, int id)
        {
            using (var client = manager.GetClient())
            {
                var key = string.Concat("/groupesdeprojets/", idgrp, "/projets/", id);
                client.Remove(key);
            }
        }
    }
}
