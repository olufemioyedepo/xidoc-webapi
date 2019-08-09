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
    public class SalesLineController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public SalesLineController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(SalesLine salesLineInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = salesLineOperations.Save(salesLineInfo);

            if (salesLineResponse == null)
            {
                return BadRequest();
            }

            return Created("createsalesline", salesLineResponse);
        }

        [HttpPost]
        [Route("salesordernumber")]
        public IActionResult GetSalesOrderByAgentId(SalesOrderNumberWithToken salesOrderNumberWithToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = salesLineOperations.GetSalesLineBySalesOrderNumber(salesOrderNumberWithToken);

            return Ok(salesLineResponse);
        }
    }
}