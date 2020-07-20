using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Niolog.Models;

namespace Niolog.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly AppSettings appSettings;

        private ILogger<HomeController> logger;

        public HomeController(IOptions<AppSettings> appSettings, ILogger<HomeController> logger)
        {
            this.appSettings = appSettings.Value;
            this.logger = logger;
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

        [HttpGet]
        [Route("{project}/collections"), ModelValidation]
        public object GetCollections(string project)
        {
            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                return db.GetCollectionNames();
            }
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

        [Route("{project}/{collection}/query")]
        public object QueryByTime(string project, string collection, string query, DateTime startTime = default, 
            DateTime endTime = default, int skip = 0, int limit = 100)
        {
            using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
            {
                var col = db.GetCollection<BsonDocument>(collection);
                if (startTime == default)
                {
                    startTime = DateTime.Now.AddMinutes(-1 * appSettings.DefaultObservationRange);
                }
                if (endTime == default)
                {
                    endTime = DateTime.Now;
                }
                var q = Query.Between("Time", startTime, endTime);
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var strs = query.Split(':');
                    q = Query.And(q, Query.Contains(strs[0], strs[1]));
                }
                return this.ConvertToResult(col.Find(q, skip, limit));
            }
        }

        private SearchResultModel ConvertToResult(IEnumerable<BsonDocument> records)
        {
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
                        if (pair.Value.IsDocument)
                        {
                            return JsonSerializer.Serialize(pair.Value);
                        }
                        else
                        {
                            return pair.Value.AsString;
                        }
                    }
                });
            })
            .OrderByDescending(dic => dic.ContainsKey("Time") ? dic["Time"] : default).ToList();
            return result;
        }
    }
}