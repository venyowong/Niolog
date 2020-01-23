using System.Collections.Generic;

namespace Niolog.Web.Models
{
    public class SearchResultModel
    {
        public List<string> Keys{get;set;}
        public List<Dictionary<string, string>> Logs{get;set;}
    }
}