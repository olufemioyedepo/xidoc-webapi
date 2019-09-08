using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class Currency
    {
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
    }

    public class CurrencyItem
    {
        public string CurrencyCode { get; set; }
        public string NameCurrencyCode { get; set; }
    }
}
