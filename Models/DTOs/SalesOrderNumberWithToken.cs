using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class SalesOrderNumber
    {
        [Required]
        public string OrderNumber { get; set; }
    }

    public class SalesOrderNumbeForSave
    {
        public string SalesOrderNumber { get; set; }
    }
}
