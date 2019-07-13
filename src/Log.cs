using System;
using Niolog.Interfaces;

namespace Niolog
{
    public class Log : Tagger, ILog
    {
        private INiologger logger;

        public DateTime LogTime {get;} = DateTime.Now;

        public Log(string level, INiologger logger)
        {
            this.logger = logger;
            base.Tag("Level", level);
        }

        public ILog Message(string message)
        {
            base.Tag("Message", message);
            return this;
        }

        public ILog Exception(Exception exception, bool withTrace = false)
        {
            var message = exception.Message;
            if(withTrace)
            {
                message += $"\n{exception.StackTrace}";
            }
            base.Tag("Exception", message);
            return this;
        }

        public void Write()
        {
            var tagger = new Tagger();
            tagger.Tag("Time", this.LogTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            tagger.Tags.AddRange(this.logger.Tags);
            tagger.Tags.AddRange(this.Tags);
            this.logger.Write(tagger);
        }
    }
}