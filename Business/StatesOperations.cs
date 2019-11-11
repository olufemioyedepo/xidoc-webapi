using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using GeofencingWebApi.Models.ODataResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeofencingWebApi.Business
{
    public class StatesOperations
    {
        readonly IConfiguration _configuration;
        private string nigerianstates;
        private string jsonResponse;
        public StatesOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            nigerianstates = _configuration.GetSection("Endpoints").GetSection("nigerianstates").Value;
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
                //Log.Error(ex.Message);
            }

            return statesResponseList.OrderBy(s => s.State).ToList();
        }
    }
}
