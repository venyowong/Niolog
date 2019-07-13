using System;
using Microsoft.Extensions.Logging;
using Niolog.Interfaces;

namespace Niolog
{
    public class Logger : ILogger
    {
        private string categoryName;

        public Logger(string categoryName)
        {
            this.categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if(logLevel == LogLevel.Trace || logLevel == LogLevel.Information || 
                logLevel == LogLevel.Warning || logLevel == LogLevel.Error)
            {
                return true;
            }

            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if(!this.IsEnabled(logLevel))
            {
                return;
            }

            var logger = NiologManager.CreateLogger();
            ILog log = null;
            if(logLevel == LogLevel.Trace)
            {
                log = logger.Trace();
            }
            else if(logLevel == LogLevel.Information)
            {
                log = logger.Info();
            }
            else if(logLevel == LogLevel.Warning)
            {
                log = logger.Warn();
            }
            else if(logLevel == LogLevel.Error)
            {
                log = logger.Error();
            }
            if(log == null)
            {
                return;
            }

            if(!string.IsNullOrWhiteSpace(this.categoryName))
            {
                log.Tag("Category", this.categoryName);
            }
            log.Message(formatter?.Invoke(state, exception));
            if(exception != null)
            {
                log.Exception(exception, true);
            }
            log.Write();
        }
    }
}
