using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using LiteDB;
using System.IO;
using System;

namespace Niolog.Web.Controllers
{
    [Route("/appmetrics")]
    public class AppMetricsController : Controller
    {
        private readonly AppSettings appSettings;

        public AppMetricsController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost, ModelValidation]
        [Route("store")]
        public bool Store()
        {
            using(var streamReader = new StreamReader(this.Request.Body))
            {
                var metrics = JsonSerializer.Deserialize(streamReader.ReadToEndAsync().Result).AsDocument;
                metrics.TryGetValue("timestamp", out BsonValue timestamp);
                var time = DateTime.Now;
                if (timestamp != null)
                {
                    DateTime.TryParse(timestamp.AsString, out time);
                }
                if (metrics.TryGetValue("contexts", out BsonValue contexts))
                {
                    contexts.AsArray.AsParallel().ForAll(context =>
                    {
                        var contextDoc = context.AsDocument;
                        contextDoc.TryGetValue("context", out BsonValue name);
                        using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{name.AsString}.db")))
                        {
                            if (contextDoc.TryGetValue("apdexScores", out BsonValue scores) && scores.AsArray.Count > 0)
                            {
                                var apdexScores = db.GetCollection<BsonDocument>("apdexScores");
                                apdexScores.InsertBulk(scores.AsArray.AsParallel().Select(score => this.GetDocument(score, time)));
                                apdexScores.EnsureIndex("time");
                                apdexScores.EnsureIndex("name");
                            }
                            if (contextDoc.TryGetValue("counters", out BsonValue countersValue) && countersValue.AsArray.Count > 0)
                            {
                                var counters = db.GetCollection<BsonDocument>("counters");
                                counters.InsertBulk(countersValue.AsArray.AsParallel().Select(counter => this.GetDocument(counter, time)));
                                counters.EnsureIndex("time");
                                counters.EnsureIndex("name");
                            }
                            if (contextDoc.TryGetValue("gauges", out BsonValue gaugesValue) && gaugesValue.AsArray.Count > 0)
                            {
                                var gauges = db.GetCollection<BsonDocument>("gauges");
                                gauges.InsertBulk(gaugesValue.AsArray.AsParallel().Select(gauge => this.GetDocument(gauge, time)));
                                gauges.EnsureIndex("time");
                                gauges.EnsureIndex("name");
                            }
                            if (contextDoc.TryGetValue("histograms", out BsonValue histogramsValue) && histogramsValue.AsArray.Count > 0)
                            {
                                var histograms = db.GetCollection<BsonDocument>("histograms");
                                histograms.InsertBulk(histogramsValue.AsArray.AsParallel().Select(histogram => this.GetDocument(histogram, time)));
                                histograms.EnsureIndex("time");
                                histograms.EnsureIndex("name");
                            }
                            if (contextDoc.TryGetValue("meters", out BsonValue metersValue) && metersValue.AsArray.Count > 0)
                            {
                                var meters = db.GetCollection<BsonDocument>("meters");
                                meters.InsertBulk(metersValue.AsArray.AsParallel().Select(meter => this.GetDocument(meter, time)));
                                meters.EnsureIndex("time");
                                meters.EnsureIndex("name");
                            }
                            if (contextDoc.TryGetValue("timers", out BsonValue timersValue) && timersValue.AsArray.Count > 0)
                            {
                                var timers = db.GetCollection<BsonDocument>("timers");
                                timers.InsertBulk(timersValue.AsArray.AsParallel().Select(timer => this.GetDocument(timer, time)));
                                timers.EnsureIndex("time");
                                timers.EnsureIndex("name");
                            }
                        }
                    });
                }
            }

            return true;
        }

        private BsonDocument GetDocument(BsonValue bsonValue, DateTime time)
        {
            var doc = bsonValue.AsDocument;
            doc.Set("Time", time);
            return doc;
        }
    }
}