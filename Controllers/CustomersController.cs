using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using Microsoft.AspNetCore.Cors;
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

        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Get()
        {
            var customerOperations = new CustomerOperations(_configuration);

            var customersResponse = await customerOperations.GetCustomers();

            return Ok(customersResponse);
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

        //[HttpPost]
        //[Route("staffpersonnelnumber")]
        //public IActionResult GetCustomersByStaffPersonnelNumber(PersonnelNumber staffPersonnelNumber)
        //{
        //    var customerOperations = new CustomerOperations(_configuration);

        //    var customersResponse = customerOperations.GetCustomersByPersonnelNumber(staffPersonnelNumber);

        //    return Ok(customersResponse);
        //}

        [HttpDelete]
        [Route("delete/{customerAccount}")]
        public IActionResult DeleteCustomer(String customerAccount)
        {
            var customerOperations = new CustomerOperations(_configuration);
            var response = customerOperations.DeleteCustomer(customerAccount);

            return Ok(response);
        }

        [HttpGet]
        [Route("staffid/{employeeRecId}")]
        public async Task<IActionResult> GetCustomersByStaffRecId(String employeeRecId)
        {
            long id = Convert.ToInt64(employeeRecId);

            var customerOperations = new CustomerOperations(_configuration);

            var customersResponse = await customerOperations.GetCustomersByEmployeeRecId(id);

            return Ok(customersResponse);
        }

        [HttpGet]
        [Route("pagedstaffid/{employeeRecId}/{skipCount}")]
        public async Task<IActionResult> GetPagedCustomersByStaffRecId(String employeeRecId, int skipCount)
        {
            long id = Convert.ToInt64(employeeRecId);
            var pagedCustomer = new PagedCustomers()
            {
                HcmWorkerRecId = employeeRecId,
                Skip = skipCount
            };

            var customerOperations = new CustomerOperations(_configuration);

            var customersResponse = await customerOperations.GetPagedCustomersByEmployeeRecId(pagedCustomer);

            return Ok(customersResponse);
        }

        [HttpGet]
        [Route("count/{employeeRecId}")]
        public IActionResult GetCustomersCountByStaffRecId(String employeeRecId)
        {
            long id = Convert.ToInt64(employeeRecId);

            var customerOperations = new CustomerOperations(_configuration);

            var customersCountResponse = customerOperations.GetCustomersCount(id);

            return Ok(customersCountResponse);
        }
    }
}