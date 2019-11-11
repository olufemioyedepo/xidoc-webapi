using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public SalesOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(SalesOrder salesOrderInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.Save(salesOrderInfo);

            if (salesOrderResponse == null)
            {
                return BadRequest();
            }

            return Created("saledordercreated", salesOrderResponse);
        }

        [HttpPost]
        [Route("personnelnumber")]
        public IActionResult GetSalesOrderByPersonnelNumber(PersonnelNumber personnelNumber)
        {
            var authOperation = new AuthOperations(_configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest("One of the required fields is missing!");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.GetSalesOrderByPersonnelNumber(personnelNumber);

            return Ok(salesOrderResponse);
        }

        [HttpGet]
        [Route("employeeRecId/{staffRecId}")]
        public IActionResult GetSalesOrderByEmployeeRecId(string staffRecId)
        {
            long staffrecid;

            if (staffRecId == null)
            {
                return BadRequest("Staff Rec ID is missing!");
            }

            try
            {
                staffrecid = Convert.ToInt64(staffRecId);
            }
            catch (Exception)
            {
                return BadRequest("Couldn't cast Staff Rec ID");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.GetSalesOrderByStaffRecId(staffrecid);

            return Ok(salesOrderResponse);
        }

        [HttpGet]
        [Route("lastSalesOrder/{staffRecId}")]
        public IActionResult GetLastSalesOrderByStaffRecId(string staffRecId)
        {
            long staffrecid;

            if (staffRecId == null)
            {
                return BadRequest("Staff Rec ID is missing!");
            }

            try
            {
                staffrecid = Convert.ToInt64(staffRecId);
            }
            catch (Exception)
            {
                return BadRequest("Couldn't cast Staff Rec ID");
            }

            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderResponse = salesOrderOperations.GetLastSalesOrderByStaffRecId(staffrecid);

            return Ok(salesOrderResponse);
        }

        [HttpGet]
        [Route("types")]
        public IActionResult GetSalesOrderTypes()
        {
            var salesOrderOperations = new SalesOrderOperations(_configuration);
            var salesOrderTypes = salesOrderOperations.GetSalesOrderTypes();

            return Ok(salesOrderTypes);
        }

        [HttpPost]
        [Route("cancel")]
        public IActionResult CancelSalesOrder(SalesOrderNumber salesOrderNumber)
        {
            var salesOrderOperations = new SalesOrderOperations(_configuration);
            string response = salesOrderOperations.CancelSalesOrder(salesOrderNumber);

            return Ok(response);
        }

        [HttpGet]
        [Route("count/{hcmWorkerRecId}")]
        public IActionResult SalesOrdersCount(long hcmWorkerRecId)
        {
            var salesOrderOperations = new SalesOrderOperations(_configuration);
            long salesOrdersCount = salesOrderOperations.GetSalesOrdersCount(hcmWorkerRecId);

            return Ok(salesOrdersCount);
        }

        [HttpGet]
        [Route("paged/{pageNumber}/{hcmWorkerRecId}")]
        public IActionResult PagedSalesOrders(int pageNumber, String hcmWorkerRecId)
        {
            var salesOrderOperations = new SalesOrderOperations(_configuration);
           
            var pagedSalesOrdersResponse = salesOrderOperations.GetPagedSalesOrders(pageNumber, hcmWorkerRecId);

            return Ok(pagedSalesOrdersResponse);
        }
    }
}