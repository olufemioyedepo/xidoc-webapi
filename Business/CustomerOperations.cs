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
using System.Net.Http;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class CustomerOperations
    {
        readonly IConfiguration _configuration;
        private readonly string createcustomerendpoint, getcustomersendpoint, customersbypersonnelnumber, customergroups;
        private string jsonResponse;
        //private string url;

        public CustomerOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            createcustomerendpoint = _configuration.GetSection("Endpoints").GetSection("createcustomer").Value;
            getcustomersendpoint = _configuration.GetSection("Endpoints").GetSection("customers").Value;
            customersbypersonnelnumber = _configuration.GetSection("Endpoints").GetSection("customersbystaffpersonnelnumber").Value;
            customergroups = _configuration.GetSection("Endpoints").GetSection("customergroups").Value;
        }

        public List<SingleCustomerResponse> GetCustomers()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + getcustomersendpoint;

            var customersList = new List<SingleCustomerResponse>();

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
                            var fetchCustomersResponse = new FetchCustomersResponse();

                            jsonResponse = sr.ReadToEnd();
                            fetchCustomersResponse = JsonConvert.DeserializeObject<FetchCustomersResponse>(jsonResponse);
                            customersList = fetchCustomersResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customersList;
        }

        public CustomerResponse Save(Customer customerInfo)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            // url = currentEnvironment + endpoint;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var customerForSave = new CustomerForSave() {
                    //AccountNum = customerInfo.AccountNum,
                    City = customerInfo.City,
                    Name = customerInfo.Name,
                    //CreatorId = customerInfo.CreatorId,
                    PersonnelNumber = customerInfo.PersonnelNumber,
                    Currency = customerInfo.Currency, //"NGN",
                    CustGroup = customerInfo.CustGroup,
                    Location = customerInfo.Location,
                    Phone = customerInfo.Phone,
                    State = customerInfo.State
                };

                HttpResponseMessage responseMessage = client.PostAsJsonAsync(createcustomerendpoint, customerForSave).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return responseMessage.Content.ReadAsAsync<CustomerResponse>().Result;
            }
        }

        public List<CustomerGroup> GetCustomerGroups()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + customergroups;

            var customerGroupResponseList = new List<CustomerGroup>();

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
                            var customerGroupsResponse = new CustomerGroupResponse();

                            jsonResponse = sr.ReadToEnd();
                            customerGroupsResponse = JsonConvert.DeserializeObject<CustomerGroupResponse>(jsonResponse);
                            customerGroupResponseList = customerGroupsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customerGroupResponseList;
        }

        public List<V3CustomerResponse> GetCustomersByPersonnelNumber(PersonnelNumber staffPersonnelNumber)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string formatcustomersendpoint = String.Format(customersbypersonnelnumber, staffPersonnelNumber.Value);
            string url = currentEnvironment + formatcustomersendpoint;

            var customersList = new List<V3CustomerResponse>();

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
                            var fetchCustomersResponse = new FetchV3CustomersResponse();

                            jsonResponse = sr.ReadToEnd();
                            fetchCustomersResponse = JsonConvert.DeserializeObject<FetchV3CustomersResponse>(jsonResponse);
                            customersList = fetchCustomersResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customersList;
        }
    }
}
