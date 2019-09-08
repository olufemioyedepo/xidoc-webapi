using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.ODataResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public EmployeesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var employeeOperations = new EmployeeOperations(_configuration);
            var employeeResponse = employeeOperations.GetEmployees();

            return Ok(employeeResponse);
        }

        [HttpPost]
        [Route("addasagent")]
        public IActionResult SetGeolocationParameters(SalesAgentPayload salesAgentPayload)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(salesAgentPayload.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required field(s) is missing!");
            }

            var employeeOperations = new EmployeeOperations(_configuration);
            var geolocationParameterOdataResponse = employeeOperations.CreateEmployeeAsSalesAgent(salesAgentPayload);
            var geolocationResponse = employeeOperations.FormatOdataResponse(geolocationParameterOdataResponse);

            return Ok(geolocationResponse);
        }

        [HttpPost]
        [Route("removeasagent")]
        public IActionResult RemoveEmployeeAsAgent(RemoveSalesAgentPayload removeSalesAgentPayload)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(removeSalesAgentPayload.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }
            var employeeOperations = new EmployeeOperations(_configuration);
            string response = employeeOperations.RemoveEmployeeAsSalesAgent(removeSalesAgentPayload);

            return Ok(response);
        }

        [HttpPost]
        [Route("agents")]
        public IActionResult GetSalesAgents(Token token)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(token.Value);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            var employeeOperations = new EmployeeOperations(_configuration);
            var employeeResponse = employeeOperations.GetSalesAgents(token);

            return Ok(employeeResponse);
        }
    }
}