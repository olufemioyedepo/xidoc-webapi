using GeofencingWebApi.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class WarehouseResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<Warehouse> value { get; set; }
    }

    public class WarehouseItem
    {
        public string InventLocationId { get; set; }
        public string Name { get; set; }
    }
}
