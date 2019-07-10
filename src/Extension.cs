using Niolog.Interfaces;

namespace Niolog
{
    public static class Extension
    {
        public static T SetTag<T>(this T tagger, string name, string value) 
            where T : ITagger
        {
            tagger?.Tag(name, value);
            return tagger;
        }
    }
}