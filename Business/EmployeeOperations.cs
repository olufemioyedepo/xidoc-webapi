using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class EmployeeOperations
    {
        readonly IConfiguration _configuration;
        private readonly string employees, geolocationparameters, salesagents, single_employee;
        private string jsonResponse, employeeterritorycount, employeescount, salesrepselection;

        public EmployeeOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            employees = _configuration.GetSection("Endpoints").GetSection("employees").Value;
            geolocationparameters = _configuration.GetSection("Endpoints").GetSection("geolocationparameters").Value;
            salesagents = _configuration.GetSection("Endpoints").GetSection("salesagents").Value;
            single_employee = _configuration.GetSection("Endpoints").GetSection("single_employee").Value;
            employeeterritorycount = _configuration.GetSection("Endpoints").GetSection("employeeterritorycount").Value;
            employeescount = _configuration.GetSection("Endpoints").GetSection("employeescount").Value;
            salesrepselection = _configuration.GetSection("Endpoints").GetSection("salesrepselection").Value;
        }

        public List<EmployeeWorker> GetEmployees()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + employees;

            var employeesList = new List<EmployeeWorker>();
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
                            var employeeResponse = new EmployeeListResponse();

                            jsonResponse = sr.ReadToEnd();
                            employeeResponse = JsonConvert.DeserializeObject<EmployeeListResponse>(jsonResponse);
                            // employeeResponse.ResponseText = "success";
                            employeesList = employeeResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return employeesList;
        }

        public SalesAgentResponse CreateEmployeeAsSalesAgent(SalesAgentPayload salesAgentPayload)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var salesAgentResponse = new SalesAgentResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var salesForSave = new SalesAgentForSave()
                    {
                        AgentLocation = salesAgentPayload.AgentLocation,
                        CommissionPercentageRate = salesAgentPayload.CommissionPercentageRate,
                        CoverageRadius = salesAgentPayload.CoverageRadius,
                        IsSalesAgent = "true",
                        OutOfCoverageLimit = salesAgentPayload.OutOfCoverageLimit,
                        PersonnelNumber = salesAgentPayload.PersonnelNumber,
                        SalesAgentLatitude = salesAgentPayload.SalesAgentLatitude,
                        SalesAgentLongitude = salesAgentPayload.SalesAgentLongitude
                    };

                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(geolocationparameters, salesForSave).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    salesAgentResponse = responseMessage.Content.ReadAsAsync<SalesAgentResponse>().Result;

                    // return responseMessage.Content.ReadAsAsync<SalesAgentResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesAgentResponse;
        }

        public SalesAgentForSave FormatOdataResponse(SalesAgentResponse salesAgentResponse)
        {
            var salesAgentInfo = new SalesAgentForSave()
            {
                AgentLocation = salesAgentResponse.AgentLocation,
                CommissionPercentageRate = salesAgentResponse.CommissionPercentageRate,
                CoverageRadius = salesAgentResponse.CoverageRadius,
                IsSalesAgent = salesAgentResponse.IsSalesAgent,
                OutOfCoverageLimit = salesAgentResponse.OutOfCoverageLimit,
                PersonnelNumber = salesAgentResponse.PersonnelNumber,
                SalesAgentLatitude = salesAgentResponse.SalesAgentLatitude,
                SalesAgentLongitude = salesAgentResponse.SalesAgentLongitude
            };

            return salesAgentInfo;
        }

        public string RemoveEmployeeAsSalesAgent(RemoveSalesAgentPayload removeSalesAgentPayload)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string response = String.Empty;


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var salesAgentParamsForSave = new SalesAgentForSave()
                    {
                        PersonnelNumber = removeSalesAgentPayload.PersonnelNumber,
                        AgentLocation = "",
                        CommissionPercentageRate = 0,
                        CoverageRadius = 0,
                        IsSalesAgent = "false",
                        OutOfCoverageLimit = 0,
                        SalesAgentLatitude = "",
                        SalesAgentLongitude = ""
                    };

                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(geolocationparameters, salesAgentParamsForSave).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    response = "success";
                    
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return response;
        }

        public List<SalesAgentListItem> GetSalesAgents()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesagents;

            var salesAgentListItems = new List<SalesAgentListItem>();
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
                            var employeeResponse = new SalesAgentsListResponse();

                            jsonResponse = sr.ReadToEnd();
                            employeeResponse = JsonConvert.DeserializeObject<SalesAgentsListResponse>(jsonResponse);
                            salesAgentListItems = employeeResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesAgentListItems;
        }

        public ShortEmployeeWorker GetEmployeeByRecId(long employeeId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string formattedEndpoint = String.Format(single_employee, employeeId);
            string url = currentEnvironment + formattedEndpoint;

            var singleEmployee = new ShortEmployeeWorker();

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
                            var employeeWorkerResponse = new ShortEmployeeWorkerResponse();

                            jsonResponse = sr.ReadToEnd();
                            employeeWorkerResponse = JsonConvert.DeserializeObject<ShortEmployeeWorkerResponse>(jsonResponse);
                            if (employeeWorkerResponse.value.Count > 0)
                            {
                                singleEmployee = employeeWorkerResponse.value.ElementAt(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return singleEmployee;
        }

        /// <summary>
        /// Accepts the Employee Id and uses it to carry out a count operaton of territories that the Employee is associated with
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>An integer count of territories that the Employee falls under/belongs to</returns>
        public async Task<int> GetEmployeeTerritoriesCount(string employeeId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();

            //string formattedEmployeeId = employeeId.Replace("-", "/");
            //string url = 
            //
            string endpoint = currentEnvironment + employeeterritorycount;
            string formattedEndpoint = String.Format(endpoint, employeeId);

            int employeeTerritoryCountResponse = 0;

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedEndpoint);
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

                    employeeTerritoryCountResponse = JsonConvert.DeserializeObject<int>(jsonResponse);

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

            return employeeTerritoryCountResponse;
        }

        public async Task<long> GetEmployeesCount()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + employeescount;
            

            long employeesCountResponse = 0;

            try
            {
                var webRequest = WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    jsonResponse = reader.ReadToEnd();
                    employeesCountResponse = JsonConvert.DeserializeObject<long>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return employeesCountResponse;
        }

        public async Task<int> GetSalesRepsCount()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesrepselection;

            List<TerritorySalesRep> distinctsalesreps = new List<TerritorySalesRep>();

            try
            {
                var webRequest = WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    jsonResponse = reader.ReadToEnd();
                    var salesRepsListResponse = JsonConvert.DeserializeObject<SalesRepsResponse>(jsonResponse).value;
                    distinctsalesreps = helper.GetDistinctSalesReps(salesRepsListResponse);

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return distinctsalesreps.Count;
        }
    }
}
