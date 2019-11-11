using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class PagedSalesOrder
    {
        public int PageNumber { get; set; }
        public string HcmWorkerRecId { get; set; }
    }
}
