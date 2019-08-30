using GeofencingWebApi.Models.ODataResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class AuthOperations
    {
       IConfiguration _configuration;

        public AuthOperations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAuthToken()
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

            return authResponse.Access_Token;
        }

        public Boolean TokenExpired(string authToken)
        {
            var token = new JwtSecurityToken(jwtEncodedString: authToken);
            var exp = Convert.ToInt64(token.Claims.ElementAt(4).Value);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime.AddHours(1);

            // var dateTimeNow = DateTime.Now;
            var dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));

            if (dateTimeNow < expirationTime)
            {
                return false;
            }

            return true;
        }
    }
}
