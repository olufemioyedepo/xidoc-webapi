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
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class CurrencyOperations
    {
        readonly IConfiguration _configuration;
        private string currenciesendpoint;
        private string jsonResponse;

        public CurrencyOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            currenciesendpoint = _configuration.GetSection("Endpoints").GetSection("currencies").Value;
        }

        public List<CurrencyItem> GetCurrencies()
        {
            var authOperations = new AuthOperations(_configuration);
            var helper = new Helper(_configuration);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + currenciesendpoint;
            string token = authOperations.GetAuthToken();

            var currenciesResponseList = new List<Currency>();
            var currencyNameCodeList = new List<CurrencyItem>();

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
                            var currenciesResponse = new CurrencyResponse();

                            jsonResponse = sr.ReadToEnd();
                            currenciesResponse = JsonConvert.DeserializeObject<CurrencyResponse>(jsonResponse);
                            currenciesResponseList = currenciesResponse.value;
                        }
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            var firstCurrencyItem = new CurrencyItem()
            {
                CurrencyCode = "NGN",
                NameCurrencyCode = "Naira"
            };

            currencyNameCodeList.Add(firstCurrencyItem);

            foreach (var item in currenciesResponseList)
            {
                var obj = new CurrencyItem()
                {
                    CurrencyCode = item.CurrencyCode,
                    NameCurrencyCode = item.Name + " (" + item.CurrencyCode + ")"
                };

                if (item.CurrencyCode != "NGN")
                {
                    currencyNameCodeList.Add(obj);
                }
            }

            return currencyNameCodeList;
        }
    }
}
