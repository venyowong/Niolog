using System;
using System.Collections.Generic;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Niolog.Interfaces;
using Niolog.Web.Models;

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
        public bool Store(StoreRequestModel model)
        {
            if(model?.Taggers?.Count <= 0)
            {
                return false;
            }

            try
            {
                using(var db = new LiteDatabase(this.appSettings.LiteDb))
                {
                    var logs = db.GetCollection<BsonDocument>("logs");
                    foreach(var tagger in model.Taggers)
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
            using(var db = new LiteDatabase(this.appSettings.LiteDb))
            {
                var logs = db.GetCollection<BsonDocument>("logs");

                if(string.IsNullOrWhiteSpace(query))
                {
                    return logs.Find(Query.Between("Time", DateTime.Now.AddHours(-1), DateTime.Now));
                }
                else
                {
                    var strs = query.Split(':');
                    return logs.Find(Query.Contains(strs[0], strs[1]));
                }
            }
        }
    }
}