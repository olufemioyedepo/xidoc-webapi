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
        private readonly string saleslinecreate, saleslinecancel, saleslinebyordernumber, saleslinecount;
        private string jsonResponse, salesorderlines;

        public SalesLineOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            saleslinecreate = _configuration.GetSection("Endpoints").GetSection("saleslinecreate").Value;
            saleslinecancel = _configuration.GetSection("Endpoints").GetSection("saleslinecancel").Value;
            salesorderlines = _configuration.GetSection("Endpoints").GetSection("salesorderlines").Value;
            saleslinebyordernumber = _configuration.GetSection("Endpoints").GetSection("saleslinebyordernumber").Value;
            saleslinecount = _configuration.GetSection("Endpoints").GetSection("saleslinebyordernumber").Value;
        }

        public async Task<bool> CreateSalesLine(SalesLine salesLine)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            //var salesLineItemResponse = new SalesLineItemResponse();
            bool salesLineCreateResponse = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var salesLineForSave = new SalesLineCreate()
                    {
                        ItemNumber = salesLine.ItemNumber,
                        LineDiscountPercentage = String.IsNullOrEmpty(salesLine.LineDiscountPercentage) ? 0.0 : Convert.ToDouble(salesLine.LineDiscountPercentage),
                        LineDiscountAmount = String.IsNullOrEmpty(salesLine.LineDiscountAmount) ? 0.0 : Convert.ToDouble(salesLine.LineDiscountAmount),
                        OrderedSalesQuantity = salesLine.OrderedSalesQuantity,
                        SalesOrderNumber = salesLine.SalesOrderNumber,
                        ShippingWarehouseId = salesLine.ShippingWarehouseId
                    };

                    var responseMessage = await client.PostAsJsonAsync(salesorderlines, salesLineForSave);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        salesLineCreateResponse = true;
                    }

                    // var salesLineCreateResponse = responseMessage.Content.ReadAsAsync<SalesLineCreateResponse>().Result;
                    // return responseMessage.Content.ReadAsAsync<SalesLineItemResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesLineCreateResponse;
        }
        public List<SalesLineListItem> GetSalesLinesBySalesOrderNumber(string salesOrderNumber)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            var productOperation = new ProductOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + saleslinebyordernumber;
            string formattedUrl = String.Format(url, salesOrderNumber);

            var products = productOperation.GetProductsWithoutToken(token);

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

            foreach (var item in saleLineResponseList)
            {
                string productName = ProductOperations.GetProductName(products, item.ItemNumber);
                item.ProductName = productName;
                var nigerianDateTime = helper.ConvertToNigerianTime(item.CreatedOn);
                item.CreatedOn = nigerianDateTime;
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

        public long GetSalesLinesCount(string salesOrderNumber)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + saleslinecount;
            string formattedEndpoint = String.Format(url, salesOrderNumber);

            long salesLineCountResponse = 0;

            try
            {
                var webRequest = System.Net.WebRequest.Create(formattedEndpoint);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            salesLineCountResponse = JsonConvert.DeserializeObject<long>(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesLineCountResponse;
        }
    }
}
