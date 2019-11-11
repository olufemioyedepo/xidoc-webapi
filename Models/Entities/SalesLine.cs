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
        public double SalesPrice { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [Required]
        public string SalesId { get; set; }
        [Required]
        public double SalesQty { get; set; }
        public double LineDisc { get; set; }
        public double PriceUnit { get; set; }
        [Required]
        public string Warehouse { get; set; }
        public string ItemId { get; set; }
        //[Required]
        //public string CreatorId { get; set; }
        //[Required]
        public string PersonnelNumber { get; set; }
        [Required]
        public string SalesAgentLongitude { get; set; }
        [Required]
        public string SalesAgentLatitude { get; set; }
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

    

    
}
