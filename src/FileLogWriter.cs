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

        public FileLogWriter(string path, int batch)
            : base(batch, 1)
        {
            this.path = path;
            if(!Directory.Exists(this.path))
            {
                Directory.CreateDirectory(this.path);
            }
        }

        protected override void Consume(List<ITagger> taggers)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var filePath = Path.Combine(this.path, $"{date}.log");
            using(var writer = new StreamWriter(new FileStream(filePath, 
                FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                taggers.ForEach(tagger => writer.WriteLine(tagger.ToString()));
            }
        }
    }
}