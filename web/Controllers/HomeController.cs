using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
        [HttpPost, ModelValidation]
        [Route("{project}/store")]
        public bool Store([Required][FromBody]List<Tagger> taggers, [Required]string project)
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
                logs.EnsureIndex("Time");
                logs.EnsureIndex("Id");
                logs.EnsureIndex("Level");

                return true;
            }
        }

        [Route("{project}/search"), ModelValidation]
        public object Search(string query, [Required]string project, int skip = 0, int limit = 100)
        {
            IEnumerable<BsonDocument> records = null;
            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                var logs = db.GetCollection<BsonDocument>("logs");

                if(string.IsNullOrWhiteSpace(query))
                {
                    records = logs.Find(Query.Between("Time", 
                        DateTime.Now.AddMinutes(-1 * appSettings.DefaultObservationRange), DateTime.Now),
                        skip, limit);
                }
                else
                {
                    var strs = query.Split(':');
                    records = logs.Find(Query.Contains(strs[0], strs[1]), skip, limit);
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

        [HttpDelete, ModelValidation]
        [Route("{project}/log")]
        public bool DeleteLog([Required]string project, [Required]string beyond)
        {
            var unit = beyond[beyond.Length - 1];
            double.TryParse(beyond.Substring(0, beyond.Length - 1), out double num);
            DateTime timePoint;
            switch(unit)
            {
                case 's':
                    timePoint = DateTime.Now.AddSeconds(num * -1);
                    break;
                case 'm':
                    timePoint = DateTime.Now.AddMinutes(num * -1);
                    break;
                case 'h':
                    timePoint = DateTime.Now.AddDays(num * -1);
                    break;
                case 'd':
                    timePoint = DateTime.Now.AddDays(num * -1);
                    break;
                case 'w':
                    timePoint = DateTime.Now.AddDays(num * -7);
                    break;
                case 'M':
                    timePoint = DateTime.Now.AddMonths((int)num * -1);
                    break;
                case 'y':
                    timePoint = DateTime.Now.AddYears((int)num * -1);
                    break;
                default:
                    return false;
            }

            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                var logs = db.GetCollection<BsonDocument>("logs");
                logs.Delete(log => log["Time"].AsDateTime <= timePoint);
                return true;
            }
        }
    }
}