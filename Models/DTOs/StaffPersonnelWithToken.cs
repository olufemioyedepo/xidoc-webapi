using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class StaffPersonnelWithToken
    {
        [Required]
        public string PersonnelNumber { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
