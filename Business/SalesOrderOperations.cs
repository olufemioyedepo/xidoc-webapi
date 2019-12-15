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
        private string salesorderbypersonnelnumber, salesorderbyworkerrecid;
        private string jsonResponse;
        private string salesorderscount, pagedsalesorder;

        public SalesOrderOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            salesordercreate = _configuration.GetSection("Endpoints").GetSection("salesordercreate").Value;
            salesordercancel = _configuration.GetSection("Endpoints").GetSection("salesordercancel").Value;
            salesorderbypersonnelnumber = _configuration.GetSection("Endpoints").GetSection("salesorderbypersonnelnumber").Value;
            salesorderbyworkerrecid = _configuration.GetSection("Endpoints").GetSection("salesorderbyworkerrecid").Value;
            salesorderscount = _configuration.GetSection("Endpoints").GetSection("salesorderscount").Value;
            pagedsalesorder = _configuration.GetSection("Endpoints").GetSection("salesorderspaged").Value;
        }

        public CreateSalesOrderResponse Save(SalesOrder salesOrder)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            // url = currentEnvironment + endpoint;
            var salesOrderResponse = new CreateSalesOrderResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var dateTimeCreated = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

                    var salesOrderForSave = new SalesOrderForSave()
                    {
                        CustAccount = salesOrder.CustAccount,
                        StaffPersonnelNumber = salesOrder.StaffPersonnelNumber,
                        DateTimeCreated = DateTime.Now, //dateTimeCreated,
                        SalesAgentLatitude = salesOrder.SalesAgentLatitude,
                        SalesAgentLongitude = salesOrder.SalesAgentLongitude,
                        InventLocationId = salesOrder.InventLocationId,
                        SalesName = salesOrder.SalesName,
                        TotalDiscountPercentage = Convert.ToDouble(salesOrder.TotalDiscountPercentage),
                        SalesType = "Sales",
                        UniqueId = helper.GenerateUniqueKey(45)
                        //SalesType = salesOrder.SalesType
                    };

                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(salesordercreate, salesOrderForSave).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    salesOrderResponse = responseMessage.Content.ReadAsAsync<CreateSalesOrderResponse>().Result;
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

        public List<SalesOrderItem> GetSalesOrderByPersonnelNumber(PersonnelNumber staffPersonnelNumber)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderbypersonnelnumber;
            string formattedUrl = String.Format(url, staffPersonnelNumber.Value);

            var salesOrderResponseList = new List<SalesOrderItem>();

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

        public List<SalesOrderItem> GetSalesOrderByStaffRecId(long staffRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderbyworkerrecid;
            string formattedUrl = String.Format(url, staffRecId);

            var salesOrderResponseList = new List<SalesOrderItem>();
            var finalSalesOrderResponseList = new List<SalesOrderItem>();

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

            foreach (var salesOrderResponseItem in salesOrderResponseList)
            {
                SalesOrderItem salesOrderItem;

                salesOrderItem = salesOrderResponseItem;
                if (salesOrderResponseItem.SalesOrderStatus == "Backorder")
                {
                    salesOrderItem.SalesOrderStatus = "Open Order";
                }

                var nigerianDateTime = helper.ConvertToNigerianTime(salesOrderItem.CreatedOn);
                salesOrderItem.CreatedOn = nigerianDateTime;

                finalSalesOrderResponseList.Add(salesOrderItem);
            }
            
            return finalSalesOrderResponseList;
        }

        public SalesOrderItem GetLastSalesOrderByStaffRecId(long staffRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderbyworkerrecid;
            string formattedUrl = String.Format(url, staffRecId);

            var salesOrderResponseList = new List<SalesOrderItem>();
            var finalSalesOrderResponseList = new List<SalesOrderItem>();

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

            foreach (var salesOrderResponseItem in salesOrderResponseList)
            {
                SalesOrderItem salesOrderItem;

                salesOrderItem = salesOrderResponseItem;
                if (salesOrderResponseItem.SalesOrderStatus == "Backorder")
                {
                    salesOrderItem.SalesOrderStatus = "Open Order";
                }

                var nigerianDateTime = helper.ConvertToNigerianTime(salesOrderItem.CreatedOn);
                salesOrderItem.CreatedOn = nigerianDateTime;

                finalSalesOrderResponseList.Add(salesOrderItem);
            }

            return finalSalesOrderResponseList.ElementAt(0);
        }

        public long GetSalesOrdersCount(long hcmWorkerRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + salesorderscount;
            string formattedEndpoint = String.Format(url, hcmWorkerRecId);

            long salesOrdersCountResponse = 0;

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
                            salesOrdersCountResponse = JsonConvert.DeserializeObject<long>(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return salesOrdersCountResponse;
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
                        SalesOrderNumber = salesOrder.OrderNumber
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

        public List<SalesOrderItem> GetPagedSalesOrders(int pageNumber, String hcmWorkerRecId)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();

            const int resultperpage = 10;
            int currentPage = pageNumber;
            int skipCount = currentPage * resultperpage;
            long hcmWorkerId = Convert.ToInt64(hcmWorkerRecId);

            // &$skip = 0 &$top = 1
            // first fetchtrip should be skip=0.
            string formattedproducts = string.Format(pagedsalesorder, hcmWorkerId, skipCount, resultperpage);

            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + formattedproducts;

            var salesOrderResponseList = new List<SalesOrderItem>();
            var finalSalesOrderResponseList = new List<SalesOrderItem>();

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
                            var salesOrderResponse = new SalesOrderResponse();

                            jsonResponse = sr.ReadToEnd();
                            salesOrderResponse = JsonConvert.DeserializeObject<SalesOrderResponse>(jsonResponse);
                            salesOrderResponseList = salesOrderResponse.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            foreach (var salesOrderResponseItem in salesOrderResponseList)
            {
                SalesOrderItem salesOrderItem;

                salesOrderItem = this.formatSalesOrderItem(salesOrderResponseItem);

                var nigerianDateTime = helper.ConvertToNigerianTime(salesOrderItem.CreatedOn);
                salesOrderItem.CreatedOn = nigerianDateTime;

                finalSalesOrderResponseList.Add(salesOrderItem);
            }

            return finalSalesOrderResponseList;
        }

        private SalesOrderItem formatSalesOrderItem(SalesOrderItem salesOrderItemToFormat)
        {
            SalesOrderItem salesOrderItem;

            if (salesOrderItemToFormat.SalesOrderStatus == "Backorder")
            {
                salesOrderItemToFormat.SalesOrderStatus = "Open Order";
            }

            if (salesOrderItemToFormat.WorkflowStatus == "NotSubmitted")
            {
                salesOrderItemToFormat.WorkflowStatus = "Draft";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "PendingApproval")
            {
                salesOrderItemToFormat.WorkflowStatus = "In Review";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "ChangeRequested")
            {
                salesOrderItemToFormat.WorkflowStatus = "Change requested";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "Approved")
            {
                salesOrderItemToFormat.WorkflowStatus = "Approved";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "Rejected")
            {
                salesOrderItemToFormat.WorkflowStatus = "Rejected";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "Returned")
            {
                salesOrderItemToFormat.WorkflowStatus = "Returned";
            }
            else if (salesOrderItemToFormat.WorkflowStatus == "Cancelled")
            {
                salesOrderItemToFormat.WorkflowStatus = "Cancelled";
            }

            salesOrderItem = salesOrderItemToFormat;

            return salesOrderItem;
        }

    }
}
