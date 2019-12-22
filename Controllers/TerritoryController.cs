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
    public class TerritoryController : ControllerBase
    {
        readonly IConfiguration _configuration;

        public TerritoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task <IActionResult> Post([FromBody] Territory territoryInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing");
            }

            var territoryOperations = new TerritoryOperations(_configuration);
            var territoryCreateResponse = await territoryOperations.Save(territoryInfo);

            if (territoryCreateResponse == false)
            {
                return BadRequest();
            }

            return Created("territorycreated", territoryCreateResponse);
        }
    }
}