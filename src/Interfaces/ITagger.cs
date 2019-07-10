using System.Collections.Generic;

namespace Niolog.Interfaces
{
    public interface ITagger
    {
        List<LogTag> Tags {get;}
        ITagger Tag(string name, string value);
    }
}