using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public SalesOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(SalesOrder salesOrderInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.Save(salesOrderInfo);

            if (salesOrderResponse == null)
            {
                return BadRequest();
            }

            return Created("saledordercreated", salesOrderResponse);
        }

        [HttpPost]
        [Route("agentid")]
        public IActionResult GetSalesOrderByAgentId(AgentIdWithToken agentIdWithToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.GetSalesOrderByCreatorId(agentIdWithToken);

            return Ok(salesOrderResponse);
        }

    }
}