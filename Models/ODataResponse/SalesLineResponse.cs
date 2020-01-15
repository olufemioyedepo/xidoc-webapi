using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class SalesLineItemResponse
    {
        // [JsonProperty("@odata.context")]
        // public string odatacontext { get; set; }
        public string SalesId { get; set; }
        public double SalesQty { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public double LineDisc { get; set; }
        //public string CreatorId { get; set; }
        public string StaffPersonnelNumber { get; set; }
        public string Warehouse { get; set; }
        public string ItemId { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
    }

    public class SalesLineCreateResponse
    {
        public string SalesOrderNumber { get; set; }
        public string ItemNumber { get; set; }
        public int LineDiscountPercentage { get; set; }
        public string ShippingWarehouseId { get; set; }
        public int OrderedSalesQuantity { get; set; }
    }

    public class SalesLineListItem
    {
        //[JsonProperty("@odata.etag")]
        // public string etag { get; set; }
        public string ProductName { get; set; }
        public string ItemNumber { get; set; }
        public string SalesOrderNumber { get; set; }
        public string ShippingWarehouseId { get; set; }
        //public string SalesUnitSymbol { get; set; }
        //public DateTime RequestedReceiptDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public double SalesPrice { get; set; }
        public double LineDiscountAmount { get; set; }
        public double LineDiscountPercentage { get; set; }
        public double LineAmount { get; set; }
        public int OrderedSalesQuantity { get; set; }
        public long SalesLineRecId { get; set; }

    }

    public class SalesLineListResponse
    {
        public List<SalesLineListItem> value { get; set; }
    }
}
