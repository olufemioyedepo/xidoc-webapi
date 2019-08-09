using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class SalesOrderNumberWithToken
    {
        public string Token { get; set; }
        public string SalesOrderNumber { get; set; }
    }
}
