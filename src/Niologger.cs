using System;
using System.Collections.Generic;
using Niolog.Interfaces;

namespace Niolog
{
    public class Niologger : Tagger, INiologger
    {
        private List<Log> logs = new List<Log>();
        private ILogWriter[] writers;

        public Niologger(params ILogWriter[] writers)
        {
            base.Tag("Id", Guid.NewGuid().ToString("N"));
            this.writers = writers;
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
            if(this.writers?.Length <= 0)
            {
                return;
            }

            foreach(var writer in writers)
            {
                writer.Write(tagger);
            }
        }
    }
}