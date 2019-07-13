using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Niolog.Interfaces;

namespace Niolog
{
    public class HttpLogWriter : LogWriter
    {
        private string url;
        private HttpClient httpClient = new HttpClient();

        public HttpLogWriter(string url, int batch, int concurrent) : base(batch, concurrent)
        {
            this.url = url;
        }

        protected override void Consume(List<ITagger> taggers)
        {
            try
            {
                Task.WaitAll(this.httpClient.PostAsync(this.url, new StringContent(
                    JsonConvert.SerializeObject(taggers), Encoding.UTF8, 
                    "application/json")));
            }
            catch{}
        }
    }
}