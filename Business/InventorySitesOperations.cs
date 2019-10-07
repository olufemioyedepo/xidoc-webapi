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
    public class InventorySitesOperations
    {
        readonly IConfiguration _configuration;
        private string inventorysitesendpoint;
        private string jsonResponse;

        public InventorySitesOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            inventorysitesendpoint = _configuration.GetSection("Endpoints").GetSection("inventorysites").Value;
        }

        public List<InventorySite> GetInventorySites()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + inventorysitesendpoint;

            var inventorySitesList = new List<InventorySite>();

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
                            var warehousesResponse = new InventorySiteResponse();

                            jsonResponse = sr.ReadToEnd();
                            warehousesResponse = JsonConvert.DeserializeObject<InventorySiteResponse>(jsonResponse);
                            inventorySitesList = warehousesResponse.value;
                        }
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return inventorySitesList;
        }
    }
}
