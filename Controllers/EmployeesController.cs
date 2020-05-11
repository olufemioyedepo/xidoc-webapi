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

        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> SalesOrdersCount()
        {
            var employeeOperations = new EmployeeOperations(_configuration);
            long employeesCount = await employeeOperations.GetEmployeesCount();

            return Ok(employeesCount);
        }
        //[HttpPost]
        //[Route("addasagent")]
        //public IActionResult SetGeolocationParameters(SalesAgentPayload salesAgentPayload)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("One of the required field(s) is missing!");
        //    }

        //    var employeeOperations = new EmployeeOperations(_configuration);
        //    var geolocationParameterOdataResponse = employeeOperations.CreateEmployeeAsSalesAgent(salesAgentPayload);
        //    var geolocationResponse = employeeOperations.FormatOdataResponse(geolocationParameterOdataResponse);

        //    return Ok(geolocationResponse);
        //}

        //[HttpPost]
        //[Route("removeasagent")]
        //public IActionResult RemoveEmployeeAsAgent(RemoveSalesAgentPayload removeSalesAgentPayload)
        //{
        //    var employeeOperations = new EmployeeOperations(_configuration);
        //    string response = employeeOperations.RemoveEmployeeAsSalesAgent(removeSalesAgentPayload);

        //    return Ok(response);
        //}

        //[HttpGet]
        //[Route("salesagents")]
        //public IActionResult GetSalesAgents()
        //{
        //    var employeeOperations = new EmployeeOperations(_configuration);
        //    var employeeResponse = employeeOperations.GetSalesAgents();

        //    return Ok(employeeResponse);
        //}

        [HttpGet]
        [Route("recId/{recId}")]
        public IActionResult GetEmployeeByRecId(long recId)
        {
            var employeeOperations = new EmployeeOperations(_configuration);
            var employeeResponse = employeeOperations.GetEmployeeByRecId(recId);

            return Ok(employeeResponse);
        }

        [HttpPost]
        [Route("issalesagent")]
        public async Task<IActionResult> GetEmployeeTerritoriesCount(PersonnelNumber personnelNumber)
        {
            var employeeOperations = new EmployeeOperations(_configuration);
            var employeeTerritoryCount = await employeeOperations.GetEmployeeTerritoriesCount(personnelNumber.Value);

            if (employeeTerritoryCount == 0)
            {
                return Ok(false);
            }
            
            return Ok(true);
        }

        [HttpGet]
        [Route("salesrepscount")]
        public async Task<IActionResult> GetSalesReps()
        {
            var employeeOperations = new EmployeeOperations(_configuration);
            var employeeTerritoryCount = await employeeOperations.GetSalesRepsCount();

            return Ok(employeeTerritoryCount);
        }
    }
}