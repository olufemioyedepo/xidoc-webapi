using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class SalesLineRecIdWithToken
    {
        public long SalesLineRecId { get; set; }
        public string Token { get; set; }
    }

    public class SalesLineRecIdForSave
    {
        public long SalesLineRecId { get; set; }
    }
}
