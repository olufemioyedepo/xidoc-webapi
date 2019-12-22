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
    public class RegionOperations
    {
        readonly IConfiguration _configuration;
        private string regions;
        private string jsonResponse;

        public RegionOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            regions = _configuration.GetSection("Endpoints").GetSection("regions").Value;
        }

        public List<Region> GetRegions()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + regions;

            var regionsResponseList = new List<Region>();

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
                            var regionsResponse = new RegionsResponse();

                            jsonResponse = sr.ReadToEnd();
                            regionsResponse = JsonConvert.DeserializeObject<RegionsResponse>(jsonResponse);
                            regionsResponseList = regionsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return regionsResponseList.OrderBy(s => s.Name).ToList();
        }
    }
}
