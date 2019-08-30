using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class EmployeeOperations
    {
        readonly IConfiguration _configuration;
        private readonly string employees, geolocationparameters, salesagents;
        private string jsonResponse;

        public EmployeeOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            employees = _configuration.GetSection("Endpoints").GetSection("employees").Value;
            geolocationparameters = _configuration.GetSection("Endpoints").GetSection("geolocationparameters").Value;
            salesagents = _configuration.GetSection("Endpoints").GetSection("salesagents").Value;
        }

        public List<EmployeeWorker> GetEmployees(Token authToken)
        {
            var helper = new Helper(_configuration);
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
                    webRequest.Headers.Add("Authorization", "Bearer " + authToken.Value);

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
                Console.WriteLine(ex.ToString());
            }

            return employeesList;
        }

        public SalesAgentResponse CreateEmployeeAsSalesAgent(SalesAgentPayload salesAgentPayload)
        {
            var helper = new Helper(_configuration);
            string currentEnvironment = helper.GetEnvironmentUrl();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + salesAgentPayload.Token);
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

                return responseMessage.Content.ReadAsAsync<SalesAgentResponse>().Result;
            }
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
            string currentEnvironment = helper.GetEnvironmentUrl();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + removeSalesAgentPayload.Token);
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

                return "success";
            }
        }

        public List<SalesAgentListItem> GetSalesAgents(Token authToken)
        {
            var helper = new Helper(_configuration);
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
                    webRequest.Headers.Add("Authorization", "Bearer " + authToken.Value);

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
                Console.WriteLine(ex.ToString());
            }

            return salesAgentListItems;
        }
    }
}
