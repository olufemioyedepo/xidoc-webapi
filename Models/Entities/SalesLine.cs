using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class SalesLine
    {
        //public string CustAccount { get; set; }
        //public double SalesPrice { get; set; }
        //public DateTime DateTimeCreated { get; set; }
        [Required]
        public string SalesOrderNumber { get; set; }
        [Required]
        public int OrderedSalesQuantity { get; set; }
        public string LineDiscountPercentage { get; set; }
        public string LineDiscountAmount { get; set; }
        //public double PriceUnit { get; set; }
        [Required]
        public string ShippingWarehouseId { get; set; }
        [Required]
        public string ItemNumber { get; set; }
        //[Required]
        //public string CreatorId { get; set; }
        //[Required]
        //public string PersonnelNumber { get; set; }
        //[Required]
        //public string SalesAgentLongitude { get; set; }
        //[Required]
        //public string SalesAgentLatitude { get; set; }
    }

    public class SalesLineForSave
    {
        public string SalesId { get; set; }
        public double SalesQty { get; set; }
        //public double SalesPrice { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public double LineDisc { get; set; }
        public string UniqueId { get; set; }
        public double PriceUnit { get; set; }
        //public string CreatorId { get; set; }
        //public string PersonnelNumber { get; set; }
        public string Warehouse { get; set; }
        public string ItemId { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
    }

    public class SalesLineCreate
    {
        public string SalesOrderNumber { get; set; }
        public string ItemNumber { get; set; }
        public double LineDiscountPercentage { get; set; }
        public double LineDiscountAmount { get; set; }
        public string ShippingWarehouseId { get; set; }
        public int OrderedSalesQuantity { get; set; }

    }




}
