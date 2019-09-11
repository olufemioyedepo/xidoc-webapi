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
    public class SalesOrderOperations
    {
        readonly IConfiguration _configuration;
        private string salesordercreate, salesordercancel;
        private string salesorderbypersonnelnumber;
        private string jsonResponse;

        public SalesOrderOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            salesordercreate = _configuration.GetSection("Endpoints").GetSection("salesordercreate").Value;
            salesordercancel = _configuration.GetSection("Endpoints").GetSection("salesordercancel").Value;
            salesorderbypersonnelnumber = _configuration.GetSection("Endpoints").GetSection("salesorderbypersonnelnumber").Value;
        }

        public SalesOrderResponse Save(SalesOrder salesOrder)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            // url = currentEnvironment + endpoint;
            var salesOrderResponse = new SalesOrderResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + salesOrder.Token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var salesOrderForSave = new SalesOrderForSave()
                    {
                        CustAccount = salesOrder.CustAccount,
                        StaffPersonnelNumber = salesOrder.StaffPersonnelNumber,
                        DateTimeCreated = dateTimeCreated,
                        SalesAgentLatitude = salesOrder.SalesAgentLatitude,
                        SalesAgentLongitude = salesOrder.SalesAgentLongitude,
                        SalesName = salesOrder.SalesName,
                        SalesType = salesOrder.SalesType
                    };

                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(salesordercreate, salesOrderForSave).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    salesOrderResponse = responseMessage.Content.ReadAsAsync<SalesOrderResponse>().Result;
                    // return responseMessage.Content.ReadAsAsync<SalesOrderResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesOrderResponse;
        }

        public SalesOrderTypes[] GetSalesOrderTypes()
        {
            SalesOrderTypes[] salesOrderTypes =
            {
                new SalesOrderTypes { Value = "Journal"},
                new SalesOrderTypes { Value = "Quotation"},
                new SalesOrderTypes { Value = "Subscription"},
                new SalesOrderTypes { Value = "Sales order"},
                new SalesOrderTypes { Value = "Returned order"},
                new SalesOrderTypes { Value = "Item requirements"},
            };

            return salesOrderTypes;
        }

        public List<SalesOrderItem> GetSalesOrderByPersonnelNumber(StaffPersonnelWithToken agentIdWithToken)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderbypersonnelnumber;
            string formattedUrl = String.Format(url, agentIdWithToken.PersonnelNumber);

            
            var salesOrderResponseList = new List<SalesOrderItem>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + agentIdWithToken.Token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var salesOrdersResponse = new SalesOrderResponse();

                            jsonResponse = sr.ReadToEnd();
                            salesOrdersResponse = JsonConvert.DeserializeObject<SalesOrderResponse>(jsonResponse);
                            salesOrderResponseList = salesOrdersResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesOrderResponseList;
        }

        public string CancelSalesOrder(SalesOrderNumber salesOrder)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesordercancel;
            string response = String.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var salesOrderNumberForSave = new SalesOrderNumbeForSave()
                    {
                        SalesOrderNumber = salesOrder.OrdrderNumber
                    };

                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(url, salesOrderNumberForSave).Result;

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
    }
}
