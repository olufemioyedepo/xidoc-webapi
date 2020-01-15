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
using System.Net;
using System.IO;
using GeofencingWebApi.Models.DTOs;

namespace GeofencingWebApi.Business
{
    public class TerritoryOperations
    {
        readonly IConfiguration _configuration;
        private readonly string territoriesendpoint, createterritorycoordinateendpoint, territoriesbylga;
        private string jsonResponse, mapsalesreptoterritory, unmapsalesreptoterritory, territoryidbyemployeeid, territoryinfobyrecid;
        private string shortterritoryinfo, coordinatesbyrecid, employeesbyterritoryrecid, employeebyid;

        public TerritoryOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            territoriesendpoint = _configuration.GetSection("Endpoints").GetSection("territories").Value;
            createterritorycoordinateendpoint = _configuration.GetSection("Endpoints").GetSection("territorycoordinates").Value;
            territoriesbylga = _configuration.GetSection("Endpoints").GetSection("territoriesbylga").Value;
            mapsalesreptoterritory = _configuration.GetSection("Endpoints").GetSection("salesrepterritories").Value;
            unmapsalesreptoterritory = _configuration.GetSection("Endpoints").GetSection("salesrepterritoryremove").Value;
            territoryidbyemployeeid = _configuration.GetSection("Endpoints").GetSection("territoryidbyemployeeid").Value;
            territoryinfobyrecid = _configuration.GetSection("Endpoints").GetSection("territoryinfobyrecid").Value;
            shortterritoryinfo = _configuration.GetSection("Endpoints").GetSection("shortterritoryinfo").Value;
            coordinatesbyrecid = _configuration.GetSection("Endpoints").GetSection("coordinatesbyrecid").Value;
            employeesbyterritoryrecid = _configuration.GetSection("Endpoints").GetSection("employeesbyterritoryrecid").Value;
            employeebyid = _configuration.GetSection("Endpoints").GetSection("employeebyid").Value;
        }

        public async Task<TerritoryResponse> Save(Territory salesTerritory)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            //bool territoryCreateResponse = false;
            TerritoryResponse territoryCreateResponse = new TerritoryResponse();

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
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
                        LocalGovernment = salesTerritory.LocalGovernment.Trim(),
                        Location = salesTerritory.Location.Trim(),
                        Name = salesTerritory.Name.Trim(),
                        Population = salesTerritory.Population,
                        Region = salesTerritory.Region.Trim(),
                        State = salesTerritory.State.Trim(),
                        Latitude = salesTerritory.Latitude,
                        Longitude = salesTerritory.Longitude
                    };

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var response = await client.PostAsJsonAsync(territoriesendpoint, territoryForSave);

                    if (response.IsSuccessStatusCode)
                    {
                        //territoryCreateResponse = true;
                        territoryCreateResponse = await response.Content.ReadAsAsync<TerritoryResponse>();
                    }
                    //bool returnValue = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return territoryCreateResponse;
        }

        public async Task<List<ShortTerritoryResponse>> GetAllTerritories()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + shortterritoryinfo;
            var territoriesListResponse = new ShortTerritoryListResponse();
            var territoriesResponse = new List<ShortTerritoryResponse>();

            try
            {
                var webRequest = WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    // Get the stream containing all content returned by the requested server.
                    Stream dataStream = response.GetResponseStream();

                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content fully up to the end.
                    jsonResponse = reader.ReadToEnd();

                    territoriesListResponse = JsonConvert.DeserializeObject<ShortTerritoryListResponse>(jsonResponse);
                    territoriesResponse = territoriesListResponse.value;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return territoriesResponse;
        }

        public async Task<bool> SaveCoordinates(TerritoryCoordinate territoryCoordinates)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool territoryCreateResponse = false;
            // TerritoryResponse territoryCreateResponse = new TerritoryResponse();

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    foreach (var item in territoryCoordinates.Coordinates)
                    {
                        var coordinateInfoForSave = new CoordinateInfoForSave()
                        {
                            Latitude = item.Lat.ToString(),
                            Longitude = item.Lng.ToString(),
                            TerritoryRecId = territoryCoordinates.RecId
                        };

                        var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                        var response = await client.PostAsJsonAsync(createterritorycoordinateendpoint, coordinateInfoForSave);

                        if (response.IsSuccessStatusCode)
                        {
                            territoryCreateResponse = true;
                            // territoryCreateResponse = await response.Content.ReadAsAsync<TerritoryResponse>();
                        }
                    }

                    
                    //bool returnValue = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return territoryCreateResponse;
        }

        public List<TerritoryResponse> GetTerritoriesByLocalGovernment(string localGovernment)
        {
            if (!String.IsNullOrEmpty(localGovernment))
            {
                localGovernment = localGovernment.Trim();
            }

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + territoriesbylga;
            string formattedUrl = String.Format(url, localGovernment);

            var lgasResponseList = new List<TerritoryResponse>();

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
                            var lgasResponse = new TerritoryListResponse();

                            jsonResponse = sr.ReadToEnd();
                            lgasResponse = JsonConvert.DeserializeObject<TerritoryListResponse>(jsonResponse);
                            lgasResponseList = lgasResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return lgasResponseList.OrderBy(s => s.Name).ToList();
        }

        public async Task<bool> MapSalesRep(EmployeeTerritoryMap employeeTerritoryMap)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool mappedResponse = false;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    var response = await client.PostAsJsonAsync(mapsalesreptoterritory, employeeTerritoryMap);

                    if (response.IsSuccessStatusCode)
                    {
                        mappedResponse = true;
                    }
                    //bool returnValue = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return mappedResponse;
        }

        public async Task<bool> UnMapSalesRep(EmployeeTerritoryMap employeeTerritoryMap)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool mappedResponse = false;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    var response = await client.PostAsJsonAsync(unmapsalesreptoterritory, employeeTerritoryMap);

                    if (response.IsSuccessStatusCode)
                    {
                        mappedResponse = true;
                    }
                    //bool returnValue = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return mappedResponse;
        }

        public async Task<List<TerritoryResponse>> GetMappedTerritories(PersonnelNumber employeeId)
        {
            var territoryIdResponse = new List<TerritoryId>();
            var territoriesDetailsResponse = new List<TerritoryResponse>();

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + territoryidbyemployeeid;
            string formattedUrl = String.Format(url, employeeId.Value);

            try
            {
                var webRequest = WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    // Get the stream containing all content returned by the requested server.
                    Stream dataStream = response.GetResponseStream();

                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content fully up to the end.
                    jsonResponse = reader.ReadToEnd();

                    var territoryIdListResponse = new TerritoryIdListResponse();

                    territoryIdListResponse = JsonConvert.DeserializeObject<TerritoryIdListResponse>(jsonResponse);
                    territoryIdResponse = territoryIdListResponse.value;

                    territoriesDetailsResponse = await GetTerritoriesByRecIds(territoryIdResponse);

                    response.Dispose();
                    dataStream.Close();
                    dataStream.Dispose();
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return territoriesDetailsResponse;
        }

        public async Task<List<TerritoryResponse>> GetTerritoriesByRecIds(List<TerritoryId> territoryRecIds)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var lgasResponseList = new List<TerritoryResponse>();

            foreach (var territory in territoryRecIds)
            {
                string url = currentEnvironment + territoryinfobyrecid;
                string formattedUrl = String.Format(url, territory.TerritoryRecId);

                try
                {
                    var webRequest = WebRequest.Create(formattedUrl);
                    if (webRequest != null)
                    {
                        webRequest.Method = "GET";
                        webRequest.Timeout = 120000;
                        webRequest.Headers.Add("Authorization", "Bearer " + token);

                        WebResponse response = await webRequest.GetResponseAsync();
                        // Get the stream containing all content returned by the requested server.
                        Stream dataStream = response.GetResponseStream();

                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);

                        // Read the content fully up to the end.
                        jsonResponse = reader.ReadToEnd();

                        var lgasResponse = new TerritoryListResponse();
                        lgasResponse = JsonConvert.DeserializeObject<TerritoryListResponse>(jsonResponse);

                        var territoryItem = lgasResponse.value[0];
                        lgasResponseList.Add(territoryItem);

                        response.Dispose();
                        dataStream.Close();
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            

            return lgasResponseList.OrderBy(s => s.Name).ToList();
        }

        public async Task<List<TerritoryResponse>> GetTerritoryByRecId(long territoryRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var lgasResponseList = new List<TerritoryResponse>();

            string url = currentEnvironment + territoryinfobyrecid;
            string formattedUrl = String.Format(url, territoryRecId);

            try
            {
                var webRequest = WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    // Get the stream containing all content returned by the requested server.
                    Stream dataStream = response.GetResponseStream();

                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content fully up to the end.
                    jsonResponse = reader.ReadToEnd();

                    var lgasResponse = new TerritoryListResponse();
                    lgasResponse = JsonConvert.DeserializeObject<TerritoryListResponse>(jsonResponse);

                    var territoryItem = lgasResponse.value[0];
                    lgasResponseList.Add(territoryItem);

                    response.Close();
                    response.Dispose();
                    dataStream.Close();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return lgasResponseList;
        }

        public async Task<List<CoordinateInfoForSave>> GetTerritoryCoordinatesByRecId(long territoryRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var territoryCoordinates = new List<CoordinateInfoForSave>();

            string url = currentEnvironment + coordinatesbyrecid;
            string formattedUrl = String.Format(url, territoryRecId);

            try
            {
                var webRequest = WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    jsonResponse = reader.ReadToEnd();

                    var coordinatesResponse = new CoordinateInfoListResponse();
                    coordinatesResponse = JsonConvert.DeserializeObject<CoordinateInfoListResponse>(jsonResponse);

                    territoryCoordinates = coordinatesResponse.value;
                    
                    response.Dispose();
                    dataStream.Close();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return territoryCoordinates;
        }

        public async Task<List<SalesRep>> GetSalesRepsByTerritoryRecId(long territoryRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var salesRepsList = new List<SalesRep>();

            var employeeList = await this.GetEmployeeIdsInTerritory(territoryRecId);
            foreach (var employee in employeeList)
            {
                string url = currentEnvironment + employeebyid;
                string formattedUrl = String.Format(url, employee.EmployeeId);

                try
                {
                    var webRequest = WebRequest.Create(formattedUrl);
                    if (webRequest != null)
                    {
                        webRequest.Method = "GET";
                        webRequest.Timeout = 120000;
                        webRequest.Headers.Add("Authorization", "Bearer " + token);

                        WebResponse response = await webRequest.GetResponseAsync();
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);

                        jsonResponse = reader.ReadToEnd();

                        var employeesInTerritoryResponse = new HcmEmployeeListResponse();
                        employeesInTerritoryResponse = JsonConvert.DeserializeObject<HcmEmployeeListResponse>(jsonResponse);

                        var employeeInfo = employeesInTerritoryResponse.value[0];
                        var salesRep = new SalesRep()
                        {
                            EmployeeRecId = employeeInfo.HcmWorkerRecId,
                            Name = employeeInfo.Name,
                            PersonnelNumber = employeeInfo.PersonnelNumber
                        };

                        salesRepsList.Add(salesRep);

                        response.Close();
                        response.Dispose();
                        dataStream.Close();
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            return salesRepsList;
        }

        /// <summary>
        /// Gets a list of Employeed Ids belonging to a particular territory
        /// </summary>
        /// <param name="territoryRecId"></param>
        /// <returns>List of EmployeeId, TerritoryRecId</returns>
        public async Task<List<EmployeeTerritoryMap>> GetEmployeeIdsInTerritory(long territoryRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var employeeIdsInTerritory = new List<EmployeeTerritoryMap>();

            string url = currentEnvironment + employeesbyterritoryrecid;
            string formattedUrl = String.Format(url, territoryRecId);

            try
            {
                var webRequest = WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    jsonResponse = reader.ReadToEnd();

                    var employeesInTerritoryResponse = new EmployeeInTerritoryListResponse();
                    employeesInTerritoryResponse = JsonConvert.DeserializeObject<EmployeeInTerritoryListResponse>(jsonResponse);

                    employeeIdsInTerritory = employeesInTerritoryResponse.value;

                    response.Dispose();
                    dataStream.Close();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return employeeIdsInTerritory;
        }
    }
}
