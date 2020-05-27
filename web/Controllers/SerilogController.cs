using System.IO;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Niolog.Web.Controllers
{
    [Route("/serilog")]
    public class SerilogController : Controller
    {
        private readonly AppSettings appSettings;

        public SerilogController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost, ModelValidation]
        [Route("{project}/store")]
        public bool Store(string project)
        {
            using(var streamReader = new StreamReader(this.Request.Body))
            {
                var events = JsonSerializer.Deserialize(streamReader.ReadToEndAsync().Result).AsDocument;
                events.TryGetValue("events", out BsonValue array);
                using(var db = new LiteDatabase(Path.Combine(this.appSettings.LiteDb, $"{project}.db")))
                {
                    var logs = db.GetCollection<BsonDocument>("logs");
                    foreach (var log in array.AsArray)
                    {
                        var doc = log.AsDocument;
                        doc.TryGetValue("Timestamp", out BsonValue timestamp);
                        DateTime.TryParse(timestamp.AsString, out DateTime time);
                        doc.Set("Time", time);
                        logs.Insert(log.AsDocument);
                    }
                    logs.EnsureIndex("Timestamp");
                    logs.EnsureIndex("Level");
                    logs.EnsureIndex("RenderedMessage");
                    logs.EnsureIndex("Time");
                    return true;
                }
            }
        }
    }
}