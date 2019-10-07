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
        //public string AccountNum { get; set; }
        public string CustGroup { get; set; }
        public string Currency { get; set; }
        public string PersonnelNumber { get; set; }
        public string Name { get; set; }
        //public double CreatorId {get; set; }
    }

    public class SingleCustomerResponse
    {
        //[JsonProperty("@odata.etag")]
        //public string etag { get; set; }
        public string CustomerAccount { get; set; }
        public string Name { get; set; }

    }

    public class V3CustomerResponse
    {
        public string CustomerAccount { get; set; }
        public string OrganizationName { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string CustomerGroupId { get; set; }
    }

    public class FetchV3CustomersResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        public List<V3CustomerResponse> value { get; set; }
    }

    public class FetchCustomersResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        public List<SingleCustomerResponse> value { get; set; }
    }
}
