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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class CustomerOperations
    {
        readonly IConfiguration _configuration;
        private readonly string createcustomerendpoint, getcustomersendpoint, customersbypersonnelnumber, customersbystaffrecid, customergroups;
        private readonly string removecustomerendpoint, customerscount, pagedcustomers;
        private string jsonResponse;
        //private string url;

        public CustomerOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            createcustomerendpoint = _configuration.GetSection("Endpoints").GetSection("createcustomer").Value;
            removecustomerendpoint = _configuration.GetSection("Endpoints").GetSection("removecustomer").Value;
            getcustomersendpoint = _configuration.GetSection("Endpoints").GetSection("customers").Value;
            customersbypersonnelnumber = _configuration.GetSection("Endpoints").GetSection("customersbystaffpersonnelnumber").Value;
            customersbystaffrecid = _configuration.GetSection("Endpoints").GetSection("customersbystaffrecid").Value;
            customergroups = _configuration.GetSection("Endpoints").GetSection("customergroups").Value;
            customerscount = _configuration.GetSection("Endpoints").GetSection("customerscount").Value;
            pagedcustomers = _configuration.GetSection("Endpoints").GetSection("pagedcustomers").Value;
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
                    CreatorId = Convert.ToDouble(customerInfo.HcmWorkerRecId),
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

            return customerGroupResponseList.OrderBy(s => s.Description).ToList();
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

            return customersList.OrderByDescending(s => s.CustomerAccount).ToList();
        }

        public bool DeleteCustomer(string customerAccount)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string formattedEndpoint = String.Format(removecustomerendpoint, customerAccount);

            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var response = client.DeleteAsync(formattedEndpoint).Result;
                if ((int)response.StatusCode == 204)
                {
                    result = true;
                }
            }

            return result;
        }

        public List<V3CustomerResponse> GetCustomersByEmployeeRecId(Int64 employeeRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string formatcustomersendpoint = String.Format(customersbystaffrecid, employeeRecId);
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

            return customersList.OrderByDescending(s => s.CustomerAccount).ToList();
        }

        public List<V3CustomerResponse> GetPagedCustomersByEmployeeRecId(PagedCustomers pagedCustomers)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            const int resultperpage = 10;
            long hcmWorkerRecId = Convert.ToInt64(pagedCustomers.HcmWorkerRecId);

            int currentPage = pagedCustomers.Skip;
            int skipCount = currentPage * resultperpage;

            // first fetching trip should be skip=0.
            //string formattedproducts = string.Format(pagedsalesorder, pagedSalesOrder.PersonnelNumber, skipCount, resultperpage);
            string foramattedcustomersendpoint = string.Format(pagedcustomers, pagedCustomers.HcmWorkerRecId, skipCount, resultperpage);
            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            //string formatcustomersendpoint = String.Format(pagedcustomers, hcmWorkerRecId, );
            string url = currentEnvironment + foramattedcustomersendpoint;

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

            return customersList.OrderByDescending(s => s.CustomerAccount).ToList();
        }

        public long GetCustomersCount(long employeeRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            long customers = 0;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string formatcustomersendpoint = String.Format(customerscount, employeeRecId);
            string url = currentEnvironment + formatcustomersendpoint;
            
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
                            customers = Convert.ToInt64(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customers;
        }


    }
}
