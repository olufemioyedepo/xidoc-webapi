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
    public class CustomerDepositController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public CustomerDepositController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(CustomerDeposit customerDeposit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            var customerDepositOperations = new CustomerDepositOperations(_configuration);
            var response =  customerDepositOperations.Save(customerDeposit);

            if (response == false)
            {
                return BadRequest();
            }

            return Created("customerdepositupdated", response);
        }

        [HttpGet]
        [Route("employeeId")]
        public IActionResult GetCustomerDepositsByEmployeeId(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId))
            {
                return BadRequest("Employee ID does not exist!");
            }

            var customerDepositOperations = new CustomerDepositOperations(_configuration);
            var response = customerDepositOperations.GetCustomerDepositsbyEmployeeId(employeeId);

            return Ok(response);
        }

        [HttpGet]
        [Route("count")]
        public IActionResult GetCustomerDepositsCount(string employeeId)
        {
            var customerDepositsOperations = new CustomerDepositOperations(_configuration);
            long customerDepositsCount = customerDepositsOperations.GetCustomerDepositCount(employeeId);

            return Ok(customerDepositsCount);
        }

        [HttpGet]
        [Route("paged/{pageNumber}/{employeeId}")]
        public async Task<IActionResult> PagedSalesOrders(int pageNumber, string employeeId)
        {
            var customerDepositsOperations = new CustomerDepositOperations(_configuration);
            var pagedCustomerDepositsResponse = await customerDepositsOperations.GetPagedCustomerDeposits(pageNumber, employeeId);

            return Ok(pagedCustomerDepositsResponse);
        }

        [HttpDelete]
        [Route("delete/{custDepositId}")]
        public IActionResult DeleteCustomerDeposit(long custDepositId)
        {
            var customerDepositsOperations = new CustomerDepositOperations(_configuration);
            bool customerDepositRemove = customerDepositsOperations.RemoveCustomerDeposit(custDepositId);

            return Ok(customerDepositRemove);
        }

        [HttpGet]
        [Route("creditcount/{employeeId}/{month}/{fiscalYear}")]
        public IActionResult GetCreditTableCount(string employeeId, string month, string fiscalYear)
        {
            var customerDepositsOperations = new CustomerDepositOperations(_configuration);
            int creditCount = customerDepositsOperations.GetCreditTableCount(employeeId, month, fiscalYear);

            return Ok(creditCount);
        }
    }
}