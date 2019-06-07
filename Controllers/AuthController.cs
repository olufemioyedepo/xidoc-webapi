using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GeofencingWebApi.Models.WebResponse;
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
        IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("gettoken")]
        public IActionResult Authenticate()
        {
            var authResponse = new AuthResponse();

            using (var wb = new WebClient())
            {
                var data = new NameValueCollection
                {
                    ["grant_type"] = _configuration.GetSection("AuthConfig").GetSection("grant_type").Value,
                    ["client_id"] = _configuration.GetSection("AuthConfig").GetSection("client_id").Value,
                    ["client_secret"] = _configuration.GetSection("AuthConfig").GetSection("client_secret").Value,
                    ["tenant_id"] = _configuration.GetSection("AuthConfig").GetSection("tenant_id").Value,
                    ["resource"] = _configuration.GetSection("AuthConfig").GetSection("resource").Value
                };

                string url = _configuration.GetSection("AuthConfig").GetSection("url").Value;

                var response = wb.UploadValues(url, "POST", data);
                string responseInString = Encoding.UTF8.GetString(response);

                authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);
            }

            return new JsonResult(authResponse);
        }
    }
}
