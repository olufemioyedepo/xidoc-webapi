using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class TerritoryOperations
    {
        readonly IConfiguration _configuration;
        private string createterritoryendpoint;

        public TerritoryOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            createterritoryendpoint = _configuration.GetSection("Endpoints").GetSection("territories").Value;
        }

        public async Task<bool> Save(Territory salesTerritory)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool customerCreateResponse = false;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            //string endpoint = currentEnvironment + createterritoryendpoint;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var territoryForSave = new TerritoryInfoForSave()
                    {
                        LocalGovernment = salesTerritory.LocalGovernment,
                        Location = salesTerritory.Location,
                        Name = salesTerritory.Name,
                        Population = salesTerritory.Population,
                        Region = salesTerritory.Region,
                        State = salesTerritory.State
                    };

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var response = await client.PostAsJsonAsync(createterritoryendpoint, territoryForSave);

                    if (response.IsSuccessStatusCode)
                    {
                        customerCreateResponse = true;
                    }
                    //bool returnValue = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customerCreateResponse;
        }
    }
}
