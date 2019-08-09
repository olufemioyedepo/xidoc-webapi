using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class Token
    {
        [Required]
        public string Value { get; set; }
    }
}
