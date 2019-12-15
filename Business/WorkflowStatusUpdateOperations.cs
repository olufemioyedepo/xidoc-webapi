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
    public class WorkflowStatusUpdateOperations
    {
        readonly IConfiguration _configuration;
        private string salesorderworkflowsubmit, workflowstatus, jsonResponse;

        public WorkflowStatusUpdateOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            salesorderworkflowsubmit = _configuration.GetSection("Endpoints").GetSection("salesorderworkflowsubmit").Value;
            workflowstatus = _configuration.GetSection("Endpoints").GetSection("workflowstatus").Value;
        }

        public bool submitSalesOrderToWorkflow(WorkflowUpdate salesOrderWorkflowUpdate)
        {
            bool response = false;
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var workflowStatusUpdateResponse = new WorkflowUpdateResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(currentEnvironment);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    HttpResponseMessage responseMessage = client.PostAsJsonAsync(salesorderworkflowsubmit, salesOrderWorkflowUpdate).Result;

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        response = false;
                    }

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        response = true;
                    }

                    workflowStatusUpdateResponse = responseMessage.Content.ReadAsAsync<WorkflowUpdateResponse>().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return response;
        }

        public string getWorkflowStatus(String salesOrderNumber)
        {
            string workflowStatus = String.Empty;

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + workflowstatus;
            string formattedUrl = String.Format(url, salesOrderNumber);

            var currentStatusList = new List<CurrentStatusItem>();

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
                            var workflowStatusResponse = new WorkflowStatusResponse();

                            jsonResponse = sr.ReadToEnd();
                            workflowStatusResponse = JsonConvert.DeserializeObject<WorkflowStatusResponse>(jsonResponse);
                            currentStatusList = workflowStatusResponse.value;
                            if (currentStatusList.Count > 0)
                            {
                                workflowStatus = this.formatWorkflowStatus(currentStatusList.ElementAt(0).WorkflowStatus);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return workflowStatus;
        }

        public String formatWorkflowStatus(String workflowStatus)
        {
            String formattedWorkflowStatus = String.Empty;
            if (workflowStatus == "Submitted")
            {
                formattedWorkflowStatus = "Submitted";
            }
            else if (workflowStatus == "NotSubmitted")
            {
                formattedWorkflowStatus = "Draft";
            }
            else if (workflowStatus == "PendingApproval")
            {
                formattedWorkflowStatus = "In Review";
            }
            else if (workflowStatus == "ChangeRequested")
            {
                formattedWorkflowStatus = "Change requested";
            }
            else {
                formattedWorkflowStatus = workflowStatus;
            }

            return formattedWorkflowStatus;
        }
    }
}
