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

        [HttpPost]
        public IActionResult Post(SalesLine salesLineInfo)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesLineInfo.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

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
        public IActionResult GetSalesLinesBySalesOrderNumber(SalesOrderNumberWithToken salesOrderNumberWithToken)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesOrderNumberWithToken.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesLineOperations = new SalesLineOperations(_configuration);
            var salesLineResponse = salesLineOperations.GetSalesLinesBySalesOrderNumber(salesOrderNumberWithToken);

            return Ok(salesLineResponse);
        }

        [HttpPost]
        [Route("cancel")]
        public IActionResult CancelSalesLine(SalesLineRecIdWithToken salesLineRecIdWithToken)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesLineRecIdWithToken.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }
            var salesLinesOperations = new SalesLineOperations(_configuration);
            string response = salesLinesOperations.CancelSalesLine(salesLineRecIdWithToken);

            return Ok(response);
        }
    }
}