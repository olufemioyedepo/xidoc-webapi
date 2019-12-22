using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace GeofencingWebApi.Business
{
    public class CustomerDepositOperations
    {
        readonly IConfiguration _configuration;
        private string customerdepositcreate, customerdepositbyemployeeid, customerdepositcount;
        private string jsonResponse;
        private string pagedcustomerdeposits;
        private string deletecustomerdposit;
        private string credittable;
        private readonly string t;
        private int resultsperpage;

        public CustomerDepositOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            customerdepositcreate = _configuration.GetSection("Endpoints").GetSection("customerdepositcreate").Value;
            customerdepositbyemployeeid = _configuration.GetSection("Endpoints").GetSection("customerdepositbyemployeeid").Value;
            customerdepositcount = _configuration.GetSection("Endpoints").GetSection("customerdepositbyemployeeidcount").Value;
            pagedcustomerdeposits = _configuration.GetSection("Endpoints").GetSection("pagedcustomerdeposit").Value;
            resultsperpage = Convert.ToInt32(_configuration.GetSection("Variables").GetSection("resultsperpage").Value);
            deletecustomerdposit = _configuration.GetSection("Endpoints").GetSection("depositdelete").Value;
            credittable = _configuration.GetSection("Endpoints").GetSection("credittable").Value;
            t = _configuration.GetSection("Endpoints").GetSection("territories").Value;
        }

        public async Task<bool> SaveTerritoryAsync(Territory customerDeposit)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool customerDepositResponse = true;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var customerDepositForSave = new TerritoryInfoForSave()
                    {
                        LocalGovernment = customerDeposit.LocalGovernment,
                        Location = customerDeposit.Location,
                        Name = customerDeposit.Name,
                        Population = customerDeposit.Population,
                        Region = customerDeposit.Region,
                        State = customerDeposit.State
                    };

                    try
                    {
                        var response = await client.PostAsJsonAsync(t, customerDepositForSave);
                        // var d = response.EnsureSuccessStatusCode().Content;

                        bool returnValue = await response.Content.ReadAsAsync<bool>();

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    //HttpResponseMessage responseMessage = client.PostAsJsonAsync(t, customerDepositForSave).Result;
                    //var resp = client.PostAsJsonAsync(t, customerDepositForSave);

                    //if (!responseMessage.IsSuccessStatusCode)
                    //{
                    //    return false;
                    //}

                    //salesOrderResponse = responseMessage.Content.ReadAsAsync<CreateSalesOrderResponse>().Result;
                    // return responseMessage.Content.ReadAsAsync<SalesOrderResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customerDepositResponse;
        }

        public bool Save(CustomerDeposit customerDeposit)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);
            bool customerDepositResponse = true;

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));
                    
                    var customerDepositForSave = new CustomerDepositForSave()
                    {
                        AmountPaid = customerDeposit.AmountPaid,
                        BankName = customerDeposit.BankName,
                        Currency = customerDeposit.Currency,
                        CustId = customerDeposit.CustId,
                        CustName = customerDeposit.CustName,
                        DepositorName = customerDeposit.DepositorName,
                        EmployeeId = customerDeposit.EmployeeId,
                        EmployeeName = customerDeposit.EmployeeName,
                        FiscalYear = customerDeposit.FiscalYear,
                        Month = customerDeposit.Month,
                        PaymentDate = customerDeposit.PaymentDate,
                        PmtMethod = customerDeposit.PmtMethod,
                        ProcessingStatus = customerDeposit.ProcessingStatus,
                        WHTDeducted = customerDeposit.WHTDeducted,
                        UniqueId = helper.GenerateUniqueKey(45)
                    };
                    
                    HttpResponseMessage responseMessage =  client.PostAsJsonAsync(customerdepositcreate, customerDepositForSave).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    //salesOrderResponse = responseMessage.Content.ReadAsAsync<CreateSalesOrderResponse>().Result;
                    // return responseMessage.Content.ReadAsAsync<SalesOrderResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customerDepositResponse;
        }

        public List<CustomerDepositDTO> GetCustomerDepositsbyEmployeeId(string employeeId)
        {
            var authOperations = new AuthOperations(_configuration);
            var helper = new Helper(_configuration);

            string formattedEmployeeId = employeeId.Replace("-", "/");
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + customerdepositbyemployeeid;
            string formattedUrl = String.Format(url, formattedEmployeeId);
            string token = authOperations.GetAuthToken();

            var customerDepositResponseList = new List<CustomerDepositDTO>();
            var finalcustomerDepositResponseList = new List<CustomerDepositDTO>();

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
                            var customerDepositsResponse = new CustomerDepositResponse();

                            jsonResponse = sr.ReadToEnd();
                            customerDepositsResponse = JsonConvert.DeserializeObject<CustomerDepositResponse>(jsonResponse);
                            customerDepositResponseList = customerDepositsResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            foreach (var item in customerDepositResponseList)
            {
                CustomerDepositDTO customerDepositItem;

                customerDepositItem = this.formatCustomerDepositItem(item);
                var nigerianDateTime = helper.ConvertToNigerianTime(customerDepositItem.DateTimeCreated);
                customerDepositItem.DateTimeCreated = nigerianDateTime;

                finalcustomerDepositResponseList.Add(customerDepositItem);
            }

            return finalcustomerDepositResponseList;
        }

        private CustomerDepositDTO formatCustomerDepositItem(CustomerDepositDTO customerDepositItemToFormat)
        {
            CustomerDepositDTO customerDepositItem;

            if (customerDepositItemToFormat.ProcessingStatus == "InReview")
            {
                customerDepositItemToFormat.ProcessingStatus = "In Review";
            }
            else if (customerDepositItemToFormat.ProcessingStatus == "PmtReceived")
            {
                customerDepositItemToFormat.ProcessingStatus = "Payment Received";
            }
            else if (customerDepositItemToFormat.ProcessingStatus == "PmtCaptured")
            {
                customerDepositItemToFormat.ProcessingStatus = "Payment Captured";
            }

            customerDepositItem = customerDepositItemToFormat;

            return customerDepositItem;
        }

        public long GetCustomerDepositCount(string employeeId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + customerdepositcount;
            string formattedEndpoint = String.Format(url, employeeId);

            long customerDespositCountResponse = 0;

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
                            customerDespositCountResponse = JsonConvert.DeserializeObject<long>(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return customerDespositCountResponse;
        }

        public async Task<List<CustomerDepositDTO>> GetPagedCustomerDeposits(int pageNumber, string employeeId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string formattedEmployeeId = employeeId.Replace("-", "/");
            int currentPage = pageNumber;
            int skipCount = currentPage * resultsperpage;

            string formattedendpoint = string.Format(pagedcustomerdeposits, formattedEmployeeId, skipCount, resultsperpage);
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + formattedendpoint;

            var customerDepositList = new List<CustomerDepositDTO>();
            var finalCustomerDepositList = new List<CustomerDepositDTO>();

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
                            var customerDepositResponse = new CustomerDepositResponse();

                            jsonResponse = await sr.ReadToEndAsync();
                            customerDepositResponse =  JsonConvert.DeserializeObject<CustomerDepositResponse>(jsonResponse);
                            customerDepositList = customerDepositResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            foreach (var item in customerDepositList)
            {
                CustomerDepositDTO customerDepositItem;

                customerDepositItem = this.formatCustomerDepositItem(item);
                var nigerianDateTime = helper.ConvertToNigerianTime(customerDepositItem.DateTimeCreated);
                customerDepositItem.DateTimeCreated = nigerianDateTime;

                if (String.IsNullOrEmpty(item.BankName))
                {
                    customerDepositItem.BankName = "N/A";
                }

                if (String.IsNullOrEmpty(item.JournalNum))
                {
                    customerDepositItem.JournalNum = "Awaiting Approval";
                }

                if (String.IsNullOrEmpty(item.PostedWithJournalNum))
                {
                    customerDepositItem.PostedWithJournalNum = "Awaiting Approval";
                }

                if (String.IsNullOrEmpty(item.SysBankAccount))
                {
                    customerDepositItem.SysBankAccount = "Awaiting Approval";
                }

                finalCustomerDepositList.Add(customerDepositItem);
            }

            return finalCustomerDepositList;
        }

        public bool RemoveCustomerDeposit(long recordId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string endpoint = currentEnvironment + deletecustomerdposit;

            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(currentEnvironment);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var customerDepositForDelete = new CustomerDepositRec()
                {
                    RecordId = recordId
                };

                HttpResponseMessage responseMessage = client.PostAsJsonAsync(endpoint, customerDepositForDelete).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return false;
                }

                result = true;
            }

            return result;
        }

        public int GetCreditTableCount(string employeeId, string month, string fiscalYear)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();

            string formattedEmployeeId = employeeId.Replace("-", "/");
            string formattedEndpoint = String.Format(credittable, formattedEmployeeId, month, fiscalYear);
            string endpoint = currentEnvironment + formattedEndpoint;

            int creditTableCountResponse = 0;

            try
            {
                var webRequest = System.Net.WebRequest.Create(endpoint);
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
                            creditTableCountResponse = JsonConvert.DeserializeObject<int>(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return creditTableCountResponse;
        }
    }
}
