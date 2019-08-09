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
    public class SalesLineOperations
    {
        readonly IConfiguration _configuration;
        private readonly string saleslinecreate, saleslinebyordernumber;
        private string jsonResponse;

        public SalesLineOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            saleslinecreate = _configuration.GetSection("Endpoints").GetSection("saleslinecreate").Value;
            saleslinebyordernumber = _configuration.GetSection("Endpoints").GetSection("saleslinebyordernumber").Value;
        }

        public SalesLineItemResponse Save(SalesLine salesLine)
        {
            var helper = new Helper(_configuration);
            string currentEnvironment = helper.GetEnvironmentUrl();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + salesLine.Token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var salesLineForSave = new SalesLineForSave()
                {
                    SalesId = salesLine.SalesId,
                    CreatorId = salesLine.CreatorId,
                    Warehouse = salesLine.Warehouse,
                    ItemId = salesLine.ItemId,
                    DateTimeCreated = DateTime.Now,
                    LineDisc = salesLine.LineDisc,
                    SalesAgentLatitude = salesLine.SalesAgentLatitude,
                    SalesAgentLongitude = salesLine.SalesAgentLongitude,
                    SalesQty = salesLine.SalesQty
                };

                HttpResponseMessage responseMessage = client.PostAsJsonAsync(saleslinecreate, salesLineForSave).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return responseMessage.Content.ReadAsAsync<SalesLineItemResponse>().Result;
            }
        }

        public List<SalesLineListItem> GetSalesLineBySalesOrderNumber(SalesOrderNumberWithToken salesOrderNumberWithToken)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + saleslinebyordernumber;
            string formattedUrl = String.Format(url, salesOrderNumberWithToken.SalesOrderNumber);

            var salesLinesResponse = new SalesLineListResponse();
            var saleLineResponseList = new List<SalesLineListItem>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + salesOrderNumberWithToken.Token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            salesLinesResponse = JsonConvert.DeserializeObject<SalesLineListResponse>(jsonResponse);
                            saleLineResponseList = salesLinesResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return saleLineResponseList;
        }
    }
}
