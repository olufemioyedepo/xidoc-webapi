using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class SalesAgentResponse : SalesAgentForSave
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        
    }

    public class SalesAgentPayload
    {
        [Required]
        public string PersonnelNumber { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        //[Required]
        //public string IsSalesAgent { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverageLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
        [Required]
        public string Token { get; set; }
    }

    public class RemoveSalesAgentPayload
    {
        [Required]
        public string PersonnelNumber { get; set; }
        [Required]
        public string Token { get; set; }
    }

    public class SalesAgentForSave
    {
        public string PersonnelNumber { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public string IsSalesAgent { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverageLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
    }


    public class SalesAgentListItem
    {
        public string PersonnelNumber { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public string IsSalesAgent { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverageLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
        public string Name { get; set; }
    }
    public class SalesAgentsListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public List<SalesAgentListItem> value { get; set; }
    }

}
