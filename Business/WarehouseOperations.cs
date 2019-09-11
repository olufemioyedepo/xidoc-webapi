using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class WarehouseOperations
    {
        readonly IConfiguration _configuration;
        private string warehousesendpoint;
        private string jsonResponse;

        public WarehouseOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            warehousesendpoint = _configuration.GetSection("Endpoints").GetSection("warehouses").Value;
        }

        public List<Warehouse> GetWarehouses()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + warehousesendpoint;

            var warehousesResponseList = new List<Warehouse>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var warehousesResponse = new WarehouseResponse();

                            jsonResponse = sr.ReadToEnd();
                            warehousesResponse = JsonConvert.DeserializeObject<WarehouseResponse>(jsonResponse);
                            warehousesResponseList = warehousesResponse.value;
                        }
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return warehousesResponseList;
        }
    }
}
