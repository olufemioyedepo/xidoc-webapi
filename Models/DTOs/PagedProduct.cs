using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class PagedProduct
    {
        public string Token { get; set; }
        public int Page { get; set; }
    }
}
