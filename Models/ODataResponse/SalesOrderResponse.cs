using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class SalesOrderResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<SalesOrderItem> value { get; set; }
    }

    public class CreateSalesOrderResponse
    {
        //[JsonProperty("@odata.context")]
        //public string odatacontext { get; set; }
        //[JsonProperty("@odata.etag")]
        //public string etag { get; set; }
        public DateTime DateTimeCreated { get; set; }
        //[Required]
        public string CustAccount { get; set; }
        //[Required]
        public string StaffPersonnelNumber { get; set; }
        public string SalesName { get; set; }
        public string SalesType { get; set; }
        //public string InventSiteId { get; set; }
        //public string InventLocationId { get; set; }
        //[Required]
        public string SalesAgentLongitude { get; set; }
        //[Required]
        public string SalesAgentLatitude { get; set; }
    }

    public class SalesOrderResponseList
    {
        public List<SalesOrderItem> value { get; set; }
    }

    public class SalesOrderItem
    {
        public string SalesOrderNumber { get; set; } // SalesId
        public string SalesOrderName { get; set; }
        public string InvoiceCustomerAccountNumber { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string PersonnelNumber { get; set; }
        public string SalesOrderStatus { get; set; }
    }
}
