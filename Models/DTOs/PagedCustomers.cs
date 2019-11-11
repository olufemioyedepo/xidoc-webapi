using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class PagedCustomers
    {
        public String  HcmWorkerRecId { get; set; }
        public int Skip { get; set; }
        //public int Top { get; set; }
    }
}
