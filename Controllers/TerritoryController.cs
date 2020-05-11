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
    public class TerritoryController : ControllerBase
    {
        readonly IConfiguration _configuration;

        public TerritoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task <IActionResult> Post([FromBody] Territory territoryInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing");
            }

            var territoryOperations = new TerritoryOperations(_configuration);
            var territoryCreateResponse = await territoryOperations.Save(territoryInfo);

            if (territoryCreateResponse.TerritoryInfoRecId == 0)
            {
                return BadRequest();
            }

            return Created("territorycreated", territoryCreateResponse);
        }

        [HttpPost]
        [Route("savecoordinates")]
        public async Task<IActionResult> SaveTerritoryCoordinates(TerritoryCoordinate territoryCoordinates)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var territoryCoordinatesCreateResponse = await territoryOperations.SaveCoordinates(territoryCoordinates);

            return Created("territorycoordinatescreated", territoryCoordinatesCreateResponse);
        }

        [HttpGet]
        [Route("bystatelocalgovernment/{state}/{localgovernment}")]
        public IActionResult GetTerritories(string state, string localgovernment)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var lgasInTerritory = territoryOperations.GetTerritoriesByLocalGovernment(state, localgovernment);

            return Ok(lgasInTerritory);
        }

        [HttpGet]
        [Route("recId/{recId}")]
        public async Task<IActionResult> GetTerritoryByRecId(long recId)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var territory = await territoryOperations.GetTerritoryByRecId(recId);

            return Ok(territory);
        }

        [HttpGet]
        [Route("coordinatesByRecId/{recId}")]
        public async Task<IActionResult> GetTerritoryCoorindatesByRecId(long recId)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var territoryCoordinates = await territoryOperations.GetTerritoryCoordinatesByRecId(recId);

            return Ok(territoryCoordinates);
        }

        [HttpGet]
        [Route("allterritories")]
        public async Task<IActionResult> GetAllTerritories()
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var allTerritories = await territoryOperations.GetAllTerritories();

            return Ok(allTerritories);
        }

        [HttpPost]
        [Route("getmappedterritories")]
        public async Task<IActionResult> GetMappedTerritories(PersonnelNumber employeeId)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var mappedTerritories = await territoryOperations.GetMappedTerritories(employeeId);

            return Ok(mappedTerritories);
        }

        [HttpPost]
        [Route("mapsalesrep")]
        public async Task<IActionResult> MapSalesRep(EmployeeTerritoryMap employeeTerritoryMap)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            bool mapRepToTerritoryResponse = await territoryOperations.MapSalesRep(employeeTerritoryMap);

            return Created("mapreptoterritory", mapRepToTerritoryResponse);
        }

        [HttpPost]
        [Route("unmapsalesrep")]
        public async Task<IActionResult> UnMapSalesRep(EmployeeTerritoryMap employeeTerritoryMap)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            bool unmapRepToTerritoryResponse = await territoryOperations.UnMapSalesRep(employeeTerritoryMap);

            return Created("unmapreptoterritory", unmapRepToTerritoryResponse);
        }

        [HttpGet]
        [Route("salesreps/{territoryRecId}")]
        public async Task<IActionResult> GetSalesRepsByTerritoryRecId(long territoryRecId)
        {
            var territoryOperations = new TerritoryOperations(_configuration);
            var salesReps = await territoryOperations.GetSalesRepsByTerritoryRecId(territoryRecId);

            return Ok(salesReps);
        }

        //[HttpPost]
        //[Route("territoriesbyrecid")]
        //public async Task<IActionResult> GetTerritoryDetailsByRecIds(List<TerritoryId> territoryRecIds)
        //{
        //    var territoryOperations = new TerritoryOperations(_configuration);
        //    var territoriesResponse = await territoryOperations.GetTerritoriesByRecIds(territoryRecIds);

        //    return Ok(territoriesResponse);
        //}
    }
}