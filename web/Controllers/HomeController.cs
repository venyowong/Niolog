using System;
using System.Collections.Generic;
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
        [Route("store")]
        public bool Store([FromBody]List<Tagger> taggers)
        {
            if(taggers?.Count <= 0)
            {
                return false;
            }

            try
            {
                using(var db = new LiteDatabase(this.appSettings.LiteDb))
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
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("search")]
        public object Search(string query)
        {
            IEnumerable<BsonDocument> records = null;
            using(var db = new LiteDatabase(this.appSettings.LiteDb))
            {
                var logs = db.GetCollection<BsonDocument>("logs");

                if(string.IsNullOrWhiteSpace(query))
                {
                    records = logs.Find(Query.Between("Time", DateTime.Now.AddHours(-1), DateTime.Now));
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
            .OrderBy(dic => dic["Time"]).ToList();
            return result;
        }
    }
}