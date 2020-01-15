using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using GeofencingWebApi.Models.ODataResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace GeofencingWebApi.Business
{
    public class StatesOperations
    {
        readonly IConfiguration _configuration;
        private string nigerianstates, nigerianstatesbyregion, lgasbystatecode;
        private string jsonResponse;
        public StatesOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            nigerianstates = _configuration.GetSection("Endpoints").GetSection("nigerianstates").Value;
            nigerianstatesbyregion = _configuration.GetSection("Endpoints").GetSection("nigerianstatesbyregion").Value;
            lgasbystatecode = _configuration.GetSection("Endpoints").GetSection("lga").Value;
        }

        public List<StateData> GetStates()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + nigerianstates;

            var statesResponseList = new List<StateData>();

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
                            var statesResponse = new StatesResponse();

                            jsonResponse = sr.ReadToEnd();
                            statesResponse = JsonConvert.DeserializeObject<StatesResponse>(jsonResponse);
                            statesResponseList = statesResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return statesResponseList.OrderBy(s => s.State).ToList();
        }

        public List<StateData> GetStatesByRegion(string region)
        {
            if (!String.IsNullOrEmpty(region))
            {
                region = region.Trim();
            }

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + nigerianstatesbyregion;
            string formattedUrl = String.Format(url, region);

            var statesResponseList = new List<StateData>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var statesResponse = new StatesResponse();

                            jsonResponse = sr.ReadToEnd();
                            statesResponse = JsonConvert.DeserializeObject<StatesResponse>(jsonResponse);
                            statesResponseList = statesResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
            }

            return statesResponseList.OrderBy(s => s.State).ToList();
        }

        public List<LgaData> GetLgas(string stateCode)
        {
            if (!String.IsNullOrEmpty(stateCode))
            {
                stateCode = stateCode.Trim();
            }

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + lgasbystatecode;
            string formattedUrl = String.Format(url, stateCode);


            var lgasResponseList = new List<LgaData>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var lgasResponse = new LgaResponse();

                            jsonResponse = sr.ReadToEnd();
                            lgasResponse = JsonConvert.DeserializeObject<LgaResponse>(jsonResponse);
                            lgasResponseList = lgasResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            //var allLga = new LgaData()
            //{
            //    LgaCode = "All",
            //    LgaName = " All Local Governments"
            //};
            //lgasResponseList.Add(allLga);

            return lgasResponseList.OrderBy(s => s.LgaName).ToList();
        }
    }
}
