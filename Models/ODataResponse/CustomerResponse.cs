using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class CustomerResponse
    {
        //[JsonProperty("@odata.context")]
        //public string odatacontext { get; set; }

        //[JsonProperty("@odata.etag")]
        //public string etag { get; set; }

        //public string dataAreaId { get; set; }
        public string AccountNum { get; set; }
        public string CustGroup { get; set; }
        public string Currency { get; set; }
        public string StaffPersonnelNumber { get; set; }
        //public double CreatorId {get; set; }
    }
}
