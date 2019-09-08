using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GeofencingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        //public IActionResult Get()
        //{
        //    // string result = string.Empty;
        //    var authResponse = new AuthResponse();

        //    using (var wb = new WebClient())
        //    {
        //        var data = new NameValueCollection();

        //        data["grant_type"] = "client_credentials";
        //        data["client_id"] = "6f7ceb6c-fa20-40e5-8013-693ebd84a1a2";
        //        data["client_secret"] = "XM=tMt?V5DY5pRtZwj6V2DP+CVIJ_:j.";
        //        data["resource"] = "https://codix-devdevaos.sandbox.ax.dynamics.com";

        //        string url = "https://login.microsoftonline.com/ba3e3cc6-09b8-455c-a25d-9ec3bc640d7e/oauth2/token";

        //        var response = wb.UploadValues(url, "POST", data);
        //        string responseInString = Encoding.UTF8.GetString(response);

        //        authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);

        //    }

        //    return new JsonResult(authResponse.Access_Token);
        //}

        [HttpGet]
        public IActionResult Get()
        {
            // string result = string.Empty;
            var authResponse = new AuthResponse();

            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();

                data["grant_type"] = "client_credentials";
                data["client_id"] = "c11b33c6-1e65-4e0b-adc1-bd1e5ea0cdb4";
                data["client_secret"] = "6CP/?s6yHlbY=9wNG[PPl3ot=w64drqk";
                data["resource"] = "https://codix-devdevaos.sandbox.ax.dynamics.com";

                string url = "https://login.microsoftonline.com/ba3e3cc6-09b8-455c-a25d-9ec3bc640d7e/oauth2/token";

                var response = wb.UploadValues(url, "POST", data);
                string responseInString = Encoding.UTF8.GetString(response);

                authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);

            }

            return new JsonResult(authResponse.Access_Token);
        }
    }

    public class AuthResponse
    {
        public string Token_Type { get; set; }
        public string Expires_In { get; set; }
        public string Ext_Expires_In { get; set; }
        public string Expires_On { get; set; }
        public string Not_Before { get; set; }
        public string Resource { get; set; }
        public string Access_Token { get; set; }
    }
}