using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class Customer
    {
        [Required]
        public string AccountNum { get; set; }

        // Will be a dropdown box with the following options (CONT => Contract Staff, DIST => Distributions)
        // EMP => Employee, INST => Institutions, RETAIL => Retail Customers, WALKIN => Walkin Customer
        [Required]
        public string CustGroup { get; set; }

        // public string Currency { get; set; }

        /// <summary>
        /// The User ID of the Agent creating the Customer
        /// </summary>
        public double CreatorId { get; set; } // User ID of agent creating the customer entity

        /// <summary>
        /// JWT Bearer Token 
        /// </summary>
        [Required]
        public string Token { get; set; }

        [Required]
        public string PersonnelNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(255)]
        public string Phone { get; set; } // dirPartyContactInfo.Locator 

        [MaxLength(60, ErrorMessage = "Location cannot exceed 60 characters")]
        public string Location { get; set; } // dirPartyContactInfo.LocationName

        [MaxLength(20, ErrorMessage = "City cannot exceed 20 characters")]
        public string City { get; set; }

        [MaxLength(10, ErrorMessage = "State cannot exceed 10 characters")]
        public string State { get; set; }
    }

    public class CustomerForSave
    {
        [Required]
        public string AccountNum { get; set; }

        [Required]
        public string CustGroup { get; set; }

        [Required]
        public string Currency { get; set; }

        public double CreatorId { get; set; } // User ID of agent creating the customer entity

        [Required]
        public string PersonnelNumber { get; set; }

        [MaxLength(255)]
        public string Phone { get; set; } // dirPartyContactInfo.Locator 

        [MaxLength(60, ErrorMessage = "Location cannot exceed 60 characters")]
        public string Location { get; set; } // dirPartyContactInfo.LocationName

        [MaxLength(20, ErrorMessage = "City cannot exceed 20 characters")]
        public string City { get; set; }

        [MaxLength(10, ErrorMessage = "State cannot exceed 10 characters")]
        public string State { get; set; }
    }
}
