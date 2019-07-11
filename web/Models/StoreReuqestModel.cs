using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Niolog.Interfaces;

namespace Niolog.Web.Models
{
    [BindProperties(SupportsGet=true)]
    public class StoreRequestModel
    {
        public List<ITagger> Taggers{get;set;}
    }
}