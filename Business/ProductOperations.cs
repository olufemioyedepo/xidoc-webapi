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
    public class ProductOperations
    {
        readonly IConfiguration _configuration;
        private string jsonResponse, products, productscount, pagedproducts;

        public ProductOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            products = _configuration.GetSection("Endpoints").GetSection("products").Value;
            pagedproducts = _configuration.GetSection("Endpoints").GetSection("pagedproducts").Value;
            productscount = _configuration.GetSection("Endpoints").GetSection("productscount").Value;
        }

        public List<ProductItem> GetProducts()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + products;
            
            var productsResponseList = new List<ProductItem>();

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
                            var productsResponse = new ProductsResponse();

                            jsonResponse = sr.ReadToEnd();
                            productsResponse = JsonConvert.DeserializeObject<ProductsResponse>(jsonResponse);
                            productsResponseList = productsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return productsResponseList;
        }

        public List<ProductItem> GetProductsWithoutToken(string token)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            //string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + products;

            var productsResponseList = new List<ProductItem>();

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
                            var productsResponse = new ProductsResponse();

                            jsonResponse = sr.ReadToEnd();
                            productsResponse = JsonConvert.DeserializeObject<ProductsResponse>(jsonResponse);
                            productsResponseList = productsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return productsResponseList;
        }

        public static String GetProductName(List<ProductItem> products, string itemNumber)
        {
            string productName = String.Empty;

            foreach (var product in products)
            {
                if (product.ProductNumber == itemNumber)
                {
                    productName = product.ProductName;
                    break;
                }
            }

            return productName;
        }

        public long GetProductsCount()
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + productscount;

            long productCountResponse = 0;

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
                            jsonResponse = sr.ReadToEnd();
                            productCountResponse = JsonConvert.DeserializeObject<long>(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return productCountResponse;
        }

        public List<ProductItem> GetPagedProducts(int pageNumber)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();

            const int resultperpage = 20;
            int currentPage = pageNumber; // pagedProduct.Page;
            int skipCount = currentPage * resultperpage;

            //$skip=20&$top=20
            string formattedproducts = string.Format(pagedproducts, skipCount, resultperpage); 

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + formattedproducts;

            var productsResponseList = new List<ProductItem>();

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
                            var productsResponse = new ProductsResponse();

                            jsonResponse = sr.ReadToEnd();
                            productsResponse = JsonConvert.DeserializeObject<ProductsResponse>(jsonResponse);
                            productsResponseList = productsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return productsResponseList;
        }

        
    }
}
