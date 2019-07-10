using System;
using System.Collections.Generic;
using Niolog.Interfaces;

namespace Niolog
{
    public class ConsoleLogWriter : LogWriter
    {
        public ConsoleLogWriter() : base(1, 1)
        {
        }

        protected override void Consume(List<ITagger> taggers)
        {
            taggers.ForEach(tagger => Console.WriteLine(tagger.ToString()));
        }
    }
}