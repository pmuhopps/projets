using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Projets.Bean;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Projets.BatchIndexProjetOpen
{
    class Program
    {
        public static string elUrl;
        public static string backUrl;

        static void Main(string[] args)
        {
            setUrls();

            var sb = new StringBuilder();
            var grpsprjs = loadgrpsprjs();
            foreach (var grpprjs in grpsprjs)
            {
                if (!isGrpprjsClosed(grpprjs))
                {
                    var prjs = loadprjs(grpprjs);
                    foreach (var prj in prjs)
                    {
                        var prjobj = loadprj(prj);
                        if (!prjobj.IsClosed)
                        {
                            var idgrp = idgrpByUrlgrpprjs(grpprjs);
                            addCompleteStringBody(sb, prjobj, idgrp);
                        }
                    }
                }
            }

            if (isIndexProjetOpenExist())
                deleteIndexProjetOpen();
            createIndexProjetOpen();
            insertInIndexProjetOpenBatch(sb.ToString());

            Console.WriteLine("Hello World!");
        }

        private static void setUrls()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            elUrl = configuration.GetSection("Urls:el").Value;
            backUrl = configuration.GetSection("Urls:back").Value;
        }

        private static List<string> loadgrpsprjs()
        {
            var client = new RestClient(string.Concat(backUrl, "api/groupesdeprojets"));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            var grpsprjs = JsonConvert.DeserializeObject<List<string>>(response.Content);
            return grpsprjs;
        }
        private static bool isGrpprjsClosed(string grpprjs)
        {
            var client = new RestClient(string.Concat(backUrl, "api", grpprjs));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            var grpsprjs = JsonConvert.DeserializeObject<BeanGroupeDeProjets>(response.Content);
            return grpsprjs.IsClosed;
        }
        private static List<string> loadprjs(string grpprjs)
        {
            var client = new RestClient(string.Concat(backUrl, "api", grpprjs, "/projets"));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            var prjs = JsonConvert.DeserializeObject<List<string>>(response.Content);
            return prjs;
        }
        private static BeanProjet loadprj(string prj)
        {
            var client = new RestClient(string.Concat(backUrl, "api", prj));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            var prjObj = JsonConvert.DeserializeObject<BeanProjet>(response.Content);
            return prjObj;
        }

        private static bool isIndexProjetOpenExist()
        {
            var client = new RestClient(string.Concat(elUrl, "_cat/indices?v="));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            bool isIndexExist = response.Content.Contains("prjopen");
            return isIndexExist;
        }
        private static void deleteIndexProjetOpen()
        {
            var client = new RestClient(string.Concat(elUrl, "prjopen"));
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
        }
        private static void createIndexProjetOpen()
        {
            var client = new RestClient(string.Concat(elUrl, "prjopen"));
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\r\n  \"mappings\": {\r\n    \"prj\": {\r\n      \"properties\": {\r\n      \t \"grp\": { \r\n          \"type\": \"integer\"\r\n        },\r\n        \"libelle\": { \r\n          \"type\": \"text\",\r\n          \"fielddata\": true\r\n        }\r\n      }\r\n    }\r\n  }\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        private static void insertInIndexProjetOpenBatch(string p_Body)
        {
            var client = new RestClient(string.Concat(elUrl, "prjopen/prj/_bulk"));
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", p_Body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private static int idgrpByUrlgrpprjs(string grpprjs)
        {
            var id = Convert.ToInt32( grpprjs.Substring("/groupesdeprojets/".Length));
            return id;
        }

        private static void addCompleteStringBody(StringBuilder sb, BeanProjet prjobj, int idgrp)
        {
            sb.Append("{\"index\":{\"_id\":\" ");
            sb.Append(idgrp);
            sb.Append('-');
            sb.Append(prjobj.Id);
            sb.Append(" \"}}");
            sb.AppendLine();
            sb.Append("{\"libelle\": \"");
            sb.Append(prjobj.Libelle);
            sb.Append("\", \"grp\": ");
            sb.Append(idgrp);
            sb.Append("}");
            sb.AppendLine();
        }


    }
}
