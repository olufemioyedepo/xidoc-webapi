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
    public class CustomersController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public CustomersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(Customer customerInfo)
        {
            //bool tokenExpired = authOperation.TokenExpired(customerInfo.Token);

            //if (tokenExpired)
            //{
            //    return BadRequest("Authentication token has expired!");
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var customerOperations = new CustomerOperations(_configuration);

            // customerInfo.Token = token;
            var customerResponse = customerOperations.Save(customerInfo);

            if (customerResponse == null)
            {
                return BadRequest();
            }

            return Created("createcustomer", customerResponse);
        }
    }
}