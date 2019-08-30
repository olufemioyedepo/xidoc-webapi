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
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesOrderInfo.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

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
        [Route("personnelnumber")]
        public IActionResult GetSalesOrderByAgentId(StaffPersonnelWithToken agentIdWithToken)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(agentIdWithToken.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.GetSalesOrderByPersonnelNumber(agentIdWithToken);

            return Ok(salesOrderResponse);
        }

        [HttpGet]
        [Route("types")]
        public IActionResult GetSalesOrderTypes()
        {
            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderTypes = salesOrderOperations.GetSalesOrderTypes();

            return Ok(salesOrderTypes);
        }

        [HttpPost]
        [Route("cancel")]
        public IActionResult CancelSalesOrder(SalesOrderNumberWithToken salesOrderNumberWithToken)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesOrderNumberWithToken.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }
            var salesOrderOperations = new SalesOrderOperations(_configuration);
            string response = salesOrderOperations.CancelSalesOrder(salesOrderNumberWithToken);

            return Ok(response);
        }
    }
}