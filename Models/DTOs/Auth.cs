using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class Auth
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string resource { get; set; }
        public string client_secret { get; set; }
    }
}
