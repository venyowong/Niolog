using System;
using System.Collections.Generic;
using Niolog.Interfaces;

namespace Niolog
{
    public class Niologger : Tagger, INiologger
    {
        private List<Log> logs = new List<Log>();
        private ILogWriter writer;

        public Niologger(ILogWriter writer)
        {
            base.Tag("Id", Guid.NewGuid().ToString("N"));
            this.writer = writer;
        }

        public ILog Error()
        {
            return new Log("ERROR", this);
        }

        public ILog Info()
        {
            return new Log("INFO", this);
        }

        public ILog Trace()
        {
            return new Log("TRACE", this);
        }

        public ILog Warn()
        {
            return new Log("WARN", this);
        }

        public void Write(ITagger tagger)
        {
            this.writer?.Write(tagger);
        }
    }
}