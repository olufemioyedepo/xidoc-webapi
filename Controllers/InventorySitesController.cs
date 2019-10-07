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
    public class InventorySitesController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public InventorySitesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var inventorySiteOperations = new InventorySitesOperations(_configuration);
            var warehouseResponse = inventorySiteOperations.GetInventorySites();

            return Ok(warehouseResponse);
        }
    }
}