using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class Territory
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string LocalGovernment { get; set; }
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }

        public Int64 Population { get; set; }
    }

    public class TerritoryInfoForSave
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string LocalGovernment { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Int64 Population { get; set; }
    }

    public class ShortTerritoryResponse
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string LocalGovernment { get; set; }
        public Int64 TerritoryInfoRecId { get; set; }
    }

    public class TerritoryResponse : TerritoryInfoForSave
    {
        public Int64 TerritoryInfoRecId { get; set; }
    }

    public class TerritoryListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<TerritoryResponse> value { get; set; }
    }

    public class ShortTerritoryListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<ShortTerritoryResponse> value { get; set; }
    }

    public class TerritoryCoordinate
    {
        public Int64 RecId { get; set; }
        public CoordinateInfo[] Coordinates { get; set; }
    }

    public class CoordinateInfo
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    public class CoordinateInfoForSave
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Int64 TerritoryRecId { get; set; }
    }

    

    public class CoordinateInfoListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<CoordinateInfoForSave> value { get; set; }
    }

    public class EmployeeTerritoryMap
    {
        public Int64 TerritoryRecId { get; set; }
        public string EmployeeId { get; set; }
    }

    public class EmployeeInTerritoryListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<EmployeeTerritoryMap> value { get; set; }
    }

    public class TerritoryId
    {
        public Int64 TerritoryRecId { get; set; }
    }

    public class TerritoryIdListResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<TerritoryId> value { get; set; }
    }

    public class CoordinateInfoItem
    {
        public List<CoordinateInfoForSave> CoordinatesInfoList { get; set; }
    }
    public class TerritoryCoordinatesList
    {
        public List<CoordinateInfoItem> CoordinatesInfoItems { get; set; }
    }
}
