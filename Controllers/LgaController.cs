using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LgaController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public LgaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("stateCode/{stateCode}")]
        public IActionResult GetLgasByStateCode(string stateCode)
        {
            var lgasOperations = new StatesOperations(_configuration);
            var lgasResponse = lgasOperations.GetLgas(stateCode);

            return Ok(lgasResponse);
        }
    }
}