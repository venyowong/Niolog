using System;
using Niolog.Interfaces;

namespace Niolog
{
    public static class NiologManager
    {
        [ThreadStatic]
        public static INiologger _logger = null;

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

        public static INiologger CreateLogger(ILogWriter writer = null)
        {
            if(_logger == null)
            {
                _logger = new Niologger(writer);
            }

            return _logger;
        }
    }
}
