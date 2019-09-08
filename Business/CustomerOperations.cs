using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
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
        private readonly string endpoint;
        //private string url;

        public CustomerOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            endpoint = _configuration.GetSection("Endpoints").GetSection("createcustomer").Value;
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

                HttpResponseMessage responseMessage = client.PostAsJsonAsync(endpoint, customerForSave).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return responseMessage.Content.ReadAsAsync<CustomerResponse>().Result;
            }
        }

        public CustomerGroup[] GetCustomerGroups()
        {
            CustomerGroup[] customerGroups =
            {
                new CustomerGroup { Name = "Employee", Value = "EMP"},
                new CustomerGroup { Name = "Institutions", Value = "INST"},
                new CustomerGroup { Name = "Retail Customers", Value = "RETAIL"},
                new CustomerGroup { Name = "Walkin Customer", Value = "WALKIN"},
            };

            return customerGroups;
        }
    }
}
