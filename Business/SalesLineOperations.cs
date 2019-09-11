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
using System.IdentityModel.Tokens.Jwt;
using Serilog;

namespace GeofencingWebApi.Business
{
    public class SalesLineOperations
    {
        readonly IConfiguration _configuration;
        private readonly string saleslinecreate, saleslinecancel, saleslinebyordernumber;
        private string jsonResponse;

        public SalesLineOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            saleslinecreate = _configuration.GetSection("Endpoints").GetSection("saleslinecreate").Value;
            saleslinecancel = _configuration.GetSection("Endpoints").GetSection("saleslinecancel").Value;
            saleslinebyordernumber = _configuration.GetSection("Endpoints").GetSection("saleslinebyordernumber").Value;
        }

        public SalesLineItemResponse Save(SalesLine salesLine)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var salesLineItemResponse = new SalesLineItemResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var salesLineForSave = new SalesLineForSave()
                    {
                        SalesId = salesLine.SalesId,
                        PersonnelNumber = salesLine.PersonnelNumber,
                        Warehouse = salesLine.Warehouse,
                        ItemId = salesLine.ItemId,
                        DateTimeCreated = dateTimeCreated,
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

                    salesLineItemResponse = responseMessage.Content.ReadAsAsync<SalesLineItemResponse>().Result;
                    // return responseMessage.Content.ReadAsAsync<SalesLineItemResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesLineItemResponse;
        }

        public List<SalesLineListItem> GetSalesLinesBySalesOrderNumber(SalesOrderNumber salesOrder)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + saleslinebyordernumber;
            string formattedUrl = String.Format(url, salesOrder.OrdrderNumber);

            
            var saleLineResponseList = new List<SalesLineListItem>();

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
                            var salesLinesResponse = new SalesLineListResponse();

                            jsonResponse = sr.ReadToEnd();
                            salesLinesResponse = JsonConvert.DeserializeObject<SalesLineListResponse>(jsonResponse);
                            saleLineResponseList = salesLinesResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return saleLineResponseList;
        }

        public string CancelSalesLine(long salesLineId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + saleslinecancel;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var salesLineRecId = new SalesLineRecIdForSave()
                {
                    SalesLineRecId = salesLineId
                };

                HttpResponseMessage responseMessage = client.PostAsJsonAsync(url, salesLineRecId).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return "success";
            }
        }
    }
}
