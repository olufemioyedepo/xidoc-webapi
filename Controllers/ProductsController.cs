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
    public class ProductsController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Authentication token is missing!");
            }

            var productOperations = new ProductOperations(_configuration);
            var productResponse = productOperations.GetProducts(token);

            return Ok(productResponse);
        }
    }
}