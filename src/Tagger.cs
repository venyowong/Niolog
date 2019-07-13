using System.Collections.Generic;
using System.Text;
using Niolog.Interfaces;

namespace Niolog
{
    public class Tagger : ITagger
    {
        public List<LogTag> Tags {get;set;} = new List<LogTag>();

        public virtual ITagger Tag(string name, string value)
        {
            Tags.Add(new LogTag{Name = name, Value = value});
            return this;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach(var tag in this.Tags)
            {
                stringBuilder.Append($"{tag.ToString()} ");
            }
            return stringBuilder.ToString();
        }
    }
}