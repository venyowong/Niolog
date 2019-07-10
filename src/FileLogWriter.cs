using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Niolog.Interfaces;

namespace Niolog
{
    public class FileLogWriter : LogWriter
    {
        private string path;
        private string date;
        private string filePath;
        private FileStream fileStream;
        private StreamWriter writer;

        public FileLogWriter(string path, int batch, int concurrent)
            : base(batch, concurrent)
        {
            this.path = path;
            if(!Directory.Exists(this.path))
            {
                Directory.CreateDirectory(this.path);
            }
        }

        protected override void Consume(List<ITagger> taggers)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            if(this.date != now)
            {
                this.date = now;
                this.filePath = Path.Combine(this.path, $"{this.date}.log");
                this.fileStream?.Dispose();
                this.fileStream = new FileStream(this.filePath, FileMode.Append, 
                    FileAccess.ReadWrite, FileShare.ReadWrite);
                this.writer?.Dispose();
                this.writer = new StreamWriter(this.fileStream);
            }

            taggers.ForEach(tagger => this.writer.WriteLine(tagger.ToString()));
        }
    }
}