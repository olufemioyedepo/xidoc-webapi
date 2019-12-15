using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowStatusController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public WorkflowStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("update")]
        public IActionResult UpdateWorflowStatus(WorkflowUpdate workflowUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var workflowStatusUpdateOperations = new WorkflowStatusUpdateOperations(_configuration);
            var workflowStatusUpdateResponse = workflowStatusUpdateOperations.submitSalesOrderToWorkflow(workflowUpdate);

            return Created("workflowstatusupdated", workflowStatusUpdateResponse);
        }

        [HttpGet]
        [Route("current/{salesOrderNumber}")]
        public IActionResult GetWorkflowStatus(String salesOrderNumber)
        {
            string workflowStatus = String.Empty;

            if (String.IsNullOrEmpty(salesOrderNumber))
            {
                return BadRequest("Sales Order number is empty");
            }

            var workflowStatusOperations = new WorkflowStatusUpdateOperations(_configuration);
            workflowStatus = workflowStatusOperations.getWorkflowStatus(salesOrderNumber);

            return Ok(workflowStatus);
        }
    }
}