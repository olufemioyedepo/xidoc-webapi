using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class EmployeeResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<EmployeeWorker> value { get; set; }
    }

    public class EmployeeListResponse
    {
        public List<EmployeeWorker> value { get; set; }
        //public string ResponseText {get; set;}
    }

    public class EmployeeWorker
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
        public string IsSalesAgent { get; set; }
        public long HcmWorkerRecId { get; set; }
    }

    public class ShortEmployeeWorker
    {
        public string Name { get; set; }
        public string PersonnelNumber { get; set; }
        public string PrimaryContactEmail { get; set; }
        public long HcmWorkerRecId { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public float CoverageRadius { get; set; }
    }

    public class ShortEmployeeWorkerResponse
    {
        [JsonProperty("@odata.context")]
        public string context { get; set; }
        public List<ShortEmployeeWorker> value { get; set; }
    }

    public class Worker
    {
        //[JsonProperty("@odata.etag")]
        //public string etag { get; set; }
        public string PersonnelNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string IsSalesAgent { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverageLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
        public long HcmWorkerRecId { get; set; }
    }

    public class WorkerResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<Worker> value { get; set; }
    }
}
