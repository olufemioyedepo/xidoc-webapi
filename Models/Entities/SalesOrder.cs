using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class SalesOrder
    {
        /// <summary>
        /// The Timestamp of Sales Order creation
        /// </summary>
        public DateTime DateTimeCreated { get; set; }
        /// <summary>
        /// The ID of the Customer raising the Sales Order
        /// </summary>
        [Required]
        public string CustAccount { get; set; }

        /// <summary>
        /// The User ID of the Agent raising the Sales Order
        /// </summary>
        //[Required]
        //public string CreatorId { get; set; }
        
        [Required]
        public string StaffPersonnelNumber { get; set; }

        public string SalesName { get; set; }
        /// <summary>
        /// The type of the current Sales Order. Here are the possible types: Journal, Quotation, Subscription, Sales order, Returned order, Sales agreement, Item requirements
        /// Be advised to make this an enum field with the following properties
        /// Item requirements
        /// Name => Journal,        Value => 0 
        /// Name => DEL_Quotation,  Value => 1
        /// Name => Subscription,   Value =>	2
        /// Name => Sales,          Value =>	3
        /// Name => ReturnItem,     Value => 4
        /// Name => DEL_Blanket,    Value => 5
        /// Name => ItemReq,        Value => 6
        /// </summary>
        public string SalesType { get; set; }

        [MaxLength(10, ErrorMessage = "InventSiteId length can not exceed 10 characters")]
        public string InventSiteId { get; set; }

        [MaxLength(10, ErrorMessage = "InventLocationId length can not exceed 10 characters")]
        public string InventLocationId { get; set; }

        /// <summary>
        /// The Longitude coordinate of the agent making the sales order
        /// </summary>
        //[Required]
        public string SalesAgentLongitude { get; set; }

        /// <summary>
        /// The Latitutude coordinate of the agent making the sales order
        /// </summary>
        //[Required]
        public string SalesAgentLatitude { get; set; }
        public string TotalDiscountPercentage { get; set; }
    }

    public class SalesOrderForSave
    {
        public DateTime DateTimeCreated { get; set; }
        [Required]
        public string CustAccount { get; set; }
        //public string CreatorId { get; set; }
        [Required]
        public string StaffPersonnelNumber { get; set; }
        public string SalesName { get; set; }
        public string UniqueId { get; set; }
        public string SalesType { get; set; }
        //public string InventSiteId { get; set; }
        public string InventLocationId { get; set; }
        //[Required]
        public string SalesAgentLongitude { get; set; }
        //[Required]
        public string SalesAgentLatitude { get; set; }
        public double TotalDiscountPercentage { get; set; }
        //public string Token { get; set; }
    }
}
