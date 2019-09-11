using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class SalesLineRecIdWithToken
    {
        [Required]
        public long SalesLineId { get; set; }
        //[Required]
        //public string Token { get; set; }
    }

    public class SalesLineRecIdForSave
    {
        public long SalesLineRecId { get; set; }
    }
}
