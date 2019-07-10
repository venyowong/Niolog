namespace Niolog.Interfaces
{
    public interface INiologger : ITagger
    {
        ILog Trace();

        ILog Info();

        ILog Warn();

        ILog Error();

        void Write(ITagger tagger);
    }
}