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
    public class SalesOrderOperations
    {
        readonly IConfiguration _configuration;
        private string salesordercreate;
        private string salesorderbyagentid;
        private string jsonResponse;

        public SalesOrderOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            salesordercreate = _configuration.GetSection("Endpoints").GetSection("salesordercreate").Value;
            salesorderbyagentid = _configuration.GetSection("Endpoints").GetSection("salesorderbyagent").Value;
        }

        public SalesOrderResponse Save(SalesOrder salesOrder)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            // url = currentEnvironment + endpoint;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + salesOrder.Token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var salesOrderForSave = new SalesOrderForSave()
                {
                    CustAccount = salesOrder.CustAccount,
                    CreatorId = salesOrder.CreatorId,
                    DateTimeCreated = DateTime.Now,
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

                return responseMessage.Content.ReadAsAsync<SalesOrderResponse>().Result;
            }
        }

        public List<SalesOrderItem> GetSalesOrderByCreatorId(AgentIdWithToken agentIdWithToken)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderbyagentid;
            string formattedUrl = String.Format(url, agentIdWithToken.CreatorId);

            var salesOrdersResponse = new SalesOrderResponse();
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
                            jsonResponse = sr.ReadToEnd();
                            salesOrdersResponse = JsonConvert.DeserializeObject<SalesOrderResponse>(jsonResponse);
                            salesOrderResponseList = salesOrdersResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return salesOrderResponseList;
        }
    }
}
