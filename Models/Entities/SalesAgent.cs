using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class SalesAgent
    {
        public string Name { get; set; }
        public string PersonnelNumber { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverargeLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
    }

    public class SalesRep
    {
        public string Name { get; set; }
        public string PersonnelNumber { get; set; }
        public Int64 EmployeeRecId { get; set; }
    }

    public class SalesRepListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<SalesRep> value { get; set; }
    }

    public class HcmEmployee
    {
        public string Name { get; set; }
        public string PersonnelNumber { get; set; }
        public Int64 HcmWorkerRecId { get; set; }
    }

    public class HcmEmployeeListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<HcmEmployee> value { get; set; }
    }

    
}
