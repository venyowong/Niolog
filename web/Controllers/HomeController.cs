using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Niolog.Models;

namespace Niolog.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly AppSettings appSettings;

        public HomeController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }
        
        [HttpPost]
        [Route("{project}/store")]
        public bool Store([FromBody]List<Tagger> taggers, string project)
        {
            if(taggers?.Count <= 0)
            {
                return false;
            }

            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                var logs = db.GetCollection<BsonDocument>("logs");
                foreach(var tagger in taggers)
                {
                    var log = new BsonDocument();
                    foreach(var tag in tagger.Tags)
                    {
                        if(tag.Name == "Time" && DateTime.TryParse(tag.Value, out DateTime time))
                        {
                            log.Set(tag.Name, time);
                        }
                        else
                        {
                            log.Set(tag.Name, tag.Value);
                        }
                    }
                    logs.Insert(log);
                }

                return true;
            }
        }

        [Route("{project}/search")]
        public object Search(string query, string project)
        {
            IEnumerable<BsonDocument> records = null;
            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                var logs = db.GetCollection<BsonDocument>("logs");

                if(string.IsNullOrWhiteSpace(query))
                {
                    records = logs.Find(Query.Between("Time", DateTime.Now.AddMinutes(-1 * appSettings.DefaultObservationRange), DateTime.Now));
                }
                else
                {
                    var strs = query.Split(':');
                    records = logs.Find(Query.Contains(strs[0], strs[1]));
                }
            }

            var result = new SearchResultModel
            {
                Keys = new List<string>()
            };
            result.Logs = records.Select(doc => 
            {
                foreach(var key in doc.Keys)
                {
                    if(!result.Keys.Contains(key))
                    {
                        result.Keys.Add(key);
                    }
                }
                return doc.ToDictionary(pair => pair.Key, pair => 
                {
                    if(pair.Key == "Time")
                    {
                        return pair.Value.AsDateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                    }
                    else
                    {
                        return pair.Value.AsString;
                    }
                });
            })
            .OrderByDescending(dic => dic["Time"]).ToList();
            return result;
        }
    
        [HttpGet]
        [Route("projects")]
        public object GetProjects()
        {
            if(!Directory.Exists(this.appSettings.LiteDb))
            {
                return null;
            }

            var start = this.appSettings.LiteDb.Length + 1;
            return Directory.GetFiles(this.appSettings.LiteDb)
                .Select(file => file.Substring(start, file.Length - start - 3))
                .ToArray();
        }
    }
}