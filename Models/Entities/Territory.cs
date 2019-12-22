using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class Territory
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string LocalGovernment { get; set; }
        
        public Int64 Population { get; set; }
    }

    public class TerritoryInfoForSave
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string LocalGovernment { get; set; }
        public Int64 Population { get; set; }
    }
}
