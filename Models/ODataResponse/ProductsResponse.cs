using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class ProductsResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<ProductItem> value { get; set; }
    }

    public class ProductsResponseList
    {
        public List<ProductItem> value { get; set; }
    }

    public class ProductItem
    {
        public string ProductNumber { get; set; }
        // public string ProductType { get; set; }
        public string ProductName { get; set; }
    }
}
