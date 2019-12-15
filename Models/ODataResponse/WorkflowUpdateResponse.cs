using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class WorkflowUpdateResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string etag { get; set; }
        public string SalesId { get; set; }
        public string WorkflowStatusAction { get; set; }
    }

    public class CurrentStatusItem
    {
        public string WorkflowStatus { get; set; }
    }

    public class WorkflowStatusResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<CurrentStatusItem> value { get; set; }
    }
}
