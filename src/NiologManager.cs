using System;
using Niolog.Interfaces;

namespace Niolog
{
    public static class NiologManager
    {
        [ThreadStatic]
        public static INiologger _logger = null;

        public static ILogWriter[] DefaultWriters{get;set;}

        public static INiologger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        public static INiologger CreateLogger(params ILogWriter[] writers)
        {
            if(_logger == null)
            {
                if(writers?.Length > 0)
                {
                    _logger = new Niologger(writers);
                }
                else
                {
                    _logger = new Niologger(DefaultWriters);
                }
            }

            return _logger;
        }
    }
}
