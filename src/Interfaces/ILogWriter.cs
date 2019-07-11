namespace Niolog.Interfaces
{
    public interface ILogWriter
    {
        void Write(ITagger tagger);

        bool Finished();
    }
}