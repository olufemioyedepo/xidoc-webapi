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
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(token.Value);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Authentication token is missing!");
            }

            var productOperations = new ProductOperations(_configuration);
            var productResponse = productOperations.GetProducts(token);

            return Ok(productResponse);
        }


        [HttpPost]
        [Route("count")]
        public IActionResult ProductCount(Token token)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(token.Value);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Authentication token is missing!");
            }

            var productOperations = new ProductOperations(_configuration);
            var productsCount = productOperations.GetProductsCount(token);

            return Ok(productsCount);
        }

        [HttpPost]
        [Route("paged")]
        public IActionResult GetPagedProducts(PagedProduct pagedProduct)
        {
            var authOperation = new AuthOperations(_configuration);

            bool tokenExpired = authOperation.TokenExpired(pagedProduct.Token);

            if (tokenExpired)
            {
                return BadRequest("Authentication token has expired!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Required field are missing");
            }

            var productOperations = new ProductOperations(_configuration);
            var productResponse = productOperations.GetPagedProducts(pagedProduct);

            return Ok(productResponse);
        }

    }
}