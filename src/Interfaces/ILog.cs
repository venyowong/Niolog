using System;

namespace Niolog.Interfaces
{
    public interface ILog : ITagger
    {
        DateTime LogTime{get;}
        
        ILog Message(string message);

        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="withTrace">是否需要输出 stack trace</param>
        /// <returns></returns>
        ILog Exception(Exception exception, bool withTrace = false);

        void Write();
    }
}