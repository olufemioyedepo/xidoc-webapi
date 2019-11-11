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

        [HttpGet]
        public IActionResult GetProducts()
        {
            var productOperations = new ProductOperations(_configuration);
            var productResponse = productOperations.GetProducts();

            return Ok(productResponse);
        }

        [HttpPost]
        [Route("count")]
        public IActionResult ProductCount()
        {
            var productOperations = new ProductOperations(_configuration);
            var productsCount = productOperations.GetProductsCount();

            return Ok(productsCount);
        }

        [HttpGet]
        [Route("page/{pageNumber}")]
        public IActionResult GetPagedProducts(int pageNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Required field are missing");
            }

            var productOperations = new ProductOperations(_configuration);
            var productResponse = productOperations.GetPagedProducts(pageNumber);

            return Ok(productResponse);
        }

        [HttpGet]
        [Route("released")]
        public IActionResult GetReleaseProducts()
        {
            var productOperations = new ProductOperations(_configuration);
            var releasedProductResponse = productOperations.GetReleasedProducts();

            return Ok(releasedProductResponse);
        }

    }
}