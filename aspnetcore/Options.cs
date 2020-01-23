namespace Niolog.AspNetCore
{
    public class Options
    {
        public bool UseConsole{get;set;} = true;

        /// <summary>
        /// The folder path used to init FileLogWriter
        /// </summary>
        /// <value></value>
        public string FolderPath{get;set;}

        /// <summary>
        /// Log consumption with {batch} logs
        /// </summary>
        /// <value></value>
        public int FileBatch{get;set;} = 5;

        /// <summary>
        /// The url used to init HttpLogWriter
        /// </summary>
        /// <value></value>
        public string HttpUrl{get;set;}

        /// <summary>
        /// Log consumption with {batch} logs
        /// </summary>
        /// <value></value>
        public int HttpBatch{get;set;} = 5;

        /// <summary>
        /// Number of log consuming threads
        /// </summary>
        /// <value></value>
        public int HttpConcurrent{get;set;} = 3;
    }
}