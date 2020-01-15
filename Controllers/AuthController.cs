using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GeofencingWebApi.Business;
using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.ODataResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public IActionResult Get()
        //{
        //    var authResponse = new AuthResponse();

        //    using (var wb = new WebClient())
        //    {
        //        var data = new NameValueCollection();

        //        data["grant_type"] = _configuration.GetSection("AuthConfig").GetSection("grant_type").Value;
        //        data["client_id"] = _configuration.GetSection("AuthConfig").GetSection("client_id").Value;
        //        data["client_secret"] = _configuration.GetSection("AuthConfig").GetSection("client_secret").Value;
        //        //data["tenant_id"] = _configuration.GetSection("AuthConfig").GetSection("tenant_id").Value;
        //        data["resource"] = _configuration.GetSection("AuthConfig").GetSection("resource").Value;

        //        string url = _configuration.GetSection("AuthConfig").GetSection("url").Value;

        //        var response = wb.UploadValues(url, "POST", data);
        //        string responseInString = Encoding.UTF8.GetString(response);

        //        authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);

        //    }

        //    return new JsonResult(authResponse.Access_Token);
        //}

        //[HttpGet]
        //[Route("token")]
        //public IActionResult Authenticate()
        //{
        //    var authResponse = new AuthResponse();
        //    string token = String.Empty;

        //    using (var wb = new WebClient())
        //    {
        //        var data = new NameValueCollection();

        //        data["grant_type"] = "client_credentials";
        //        data["client_id"] = "c11b33c6-1e65-4e0b-adc1-bd1e5ea0cdb4";
        //        data["client_secret"] = "6CP/?s6yHlbY=9wNG[PPl3ot=w64drqk";
        //        data["resource"] = "https://codix-devdevaos.sandbox.ax.dynamics.com";

        //        string url = "https://login.microsoftonline.com/ba3e3cc6-09b8-455c-a25d-9ec3bc640d7e/oauth2/token";

        //        // var response = wb.UploadValues(url, "POST", data);
        //        //string responseInString = Encoding.UTF8.GetString(response);

        //        //authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);

        //        var authOperations = new AuthOperations(_configuration);
        //        token = authOperations.GetAuthToken();
        //        // return Ok(token);

        //    }

        //    return Ok(token);
        //}

        //[HttpGet]
        //[Route("gentoken")]
        //public IActionResult GenerateToken()
        //{
        //    var authOperations = new AuthOperations(_configuration);
        //    string token = authOperations.GetAuthToken();

        //    return Ok(token);
        //}

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var authOperation = new AuthOperations(_configuration);

            // bool tokenExpired = authOperation.TokenExpired(loginModel.Token);

            //if (tokenExpired)
            //{
            //    return BadRequest("Authentication token has expired!");
            //}

            var authOperations = new AuthOperations(_configuration);
            string token = authOperations.GetAuthToken();

            var employeeEmail = new EmployeeEmail()
            {
                Email = loginModel.Email,
                Token = token
            };


            var employeeWorkerResponse = await authOperation.DoLogin(employeeEmail);
            // var employeeWorkerResponse = authOperation.Login(employeeEmail);

            if (employeeWorkerResponse == null)
            {
                return NotFound("Invalid user credentials");
            }

            return Ok(employeeWorkerResponse);
        }
    }
}
