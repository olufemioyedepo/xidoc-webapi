using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public WarehousesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(Token authToken)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(authToken.Value);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            var warehouseOperations = new WarehouseOperations(_configuration);
            var warehouseResponse = warehouseOperations.GetWarehouses(authToken);

            return Ok(warehouseResponse);
        }
    }
}