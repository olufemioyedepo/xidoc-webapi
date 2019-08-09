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
    public class ProductOperations
    {
        readonly IConfiguration _configuration;
        private string products;
        private string jsonResponse;

        public ProductOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            products = _configuration.GetSection("Endpoints").GetSection("products").Value;
        }


        public List<Item> GetProducts(Token token)
        {
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + products;
            

            var productsResponse = new ProductsResponse();
            var productsResponseList = new List<Item>();

            try
            {
                var webRequest = System.Net.WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token.Value);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            productsResponse = JsonConvert.DeserializeObject<ProductsResponse>(jsonResponse);
                            productsResponseList = productsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return productsResponseList;
        }
    }
}
