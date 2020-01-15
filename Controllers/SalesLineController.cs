using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        //[HttpPost]
        //public IActionResult Post(SalesLine salesLineInfo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("One of the required fields is missing!");
        //    }

        //    var salesLineOperations = new SalesLineOperations(_configuration);
        //    var salesLineResponse = salesLineOperations.Save(salesLineInfo);

        //    if (salesLineResponse == null)
        //    {
        //        return BadRequest();
        //    }

        //    return Created("createsalesline", salesLineResponse);
        //}

        [HttpPost]
        public async Task<IActionResult> Post(SalesLine salesLineInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = await salesLineOperations.CreateSalesLine(salesLineInfo);

            if (salesLineResponse == false)
            {
                return BadRequest("Could not create Sales line. An error occurred");
            }

            return Created("createsalesline", salesLineResponse);
        }

        /*
        [HttpPost]
        [Route("salesordernumber")]
        public IActionResult GetSalesLinesBySalesOrderNumber(SalesOrderNumber salesOrderNumber)
        {
            var authOperation = new AuthOperations(_configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = salesLineOperations.GetSalesLinesBySalesOrderNumber(salesOrderNumber);

            return Ok(salesLineResponse);
        }
        */

        [HttpGet]
        [Route("salesordernumber/{orderNumber}")]
        public IActionResult GetSalesLinesBySalesOrderNumber(string orderNumber)
        {
            var authOperation = new AuthOperations(_configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = salesLineOperations.GetSalesLinesBySalesOrderNumber(orderNumber);

            return Ok(salesLineResponse);
        }

        [HttpGet]
        [Route("cancel/{salesLineId}")]
        public IActionResult CancelSalesLine(long salesLineId)
        {
            var salesLinesOperations = new SalesLineOperations(_configuration);
            string response = salesLinesOperations.CancelSalesLine(salesLineId);

            return Ok(response);
        }
    }
}