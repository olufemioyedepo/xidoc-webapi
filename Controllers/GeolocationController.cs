using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Models.ODataResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocationController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public GeolocationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("calculateDistance")]
        public IActionResult CalculateDistance(GeolocationParameters geolocationParameters)
        {
            var geolocationOperations = new GeolocationOperations(_configuration);
            var employeeOperations = new EmployeeOperations(_configuration);

            long recId = Convert.ToInt64(geolocationParameters.HcmWorkerRecId);
            var employeeShortInfo = employeeOperations.GetEmployeeByRecId(recId);


            var fullgeolocationdetails = new FullGeolocationParameters();
            fullgeolocationdetails.CurrentGeolocationLatitude = geolocationParameters.CurrentGeolocationLatitude;
            fullgeolocationdetails.CurrentGeolocationLongitude = geolocationParameters.CurrentGeolocationLongitude;
            
            if (employeeShortInfo != null)
            {
                fullgeolocationdetails.AgentLatitude = employeeShortInfo.SalesAgentLatitude;
                fullgeolocationdetails.AgentLongitude = employeeShortInfo.SalesAgentLongitude;
                fullgeolocationdetails.Radius = employeeShortInfo.CoverageRadius;
            }

            var withinRangeResponse = geolocationOperations.IsAgentWithinRange(fullgeolocationdetails);

            return Ok(withinRangeResponse);
        }
    }
}