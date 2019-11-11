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

    public class ReleasedProductsResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<ReleasedProductItem> value { get; set; }
    }

    public class ProductsResponseList
    {
        public List<ProductItem> value { get; set; }
    }

    public class ProductItem
    {
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
    }

    public class ReleasedProductsResponseList
    {
        public List<ReleasedProductItem> value { get; set; }
    }

    public class ReleasedProductItem
    {
        public string ItemNumber { get; set; }
        public string ItemName { get; set; } // same as product name
        public double SalesPrice { get; set; }
    }
}
