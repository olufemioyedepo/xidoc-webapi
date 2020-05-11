using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.ODataResponse;
using GeofencingWebApi.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GeofencingWebApi.Business
{
    public class AuthOperations
    {
        IConfiguration _configuration;
        private readonly string employeelogin;
        private string employeeresponse;

        public AuthOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            employeelogin = _configuration.GetSection("Endpoints").GetSection("employeelogin").Value;
        }

        public string GetAuthToken()
        {
            var authResponse = new AuthResponse();

            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();

                    data["grant_type"] = "client_credentials";
                    data["client_id"] = "c11b33c6-1e65-4e0b-adc1-bd1e5ea0cdb4";
                    data["client_secret"] = "6CP/?s6yHlbY=9wNG[PPl3ot=w64drqk";
                    // data["resource"] = "https://codix-sat.sandbox.operations.dynamics.com";
                    //data["resource"] = "https://codix-devdevaos.sandbox.ax.dynamics.com";
                    data["resource"] = "https://codix.operations.dynamics.com";

                    string url = _configuration.GetSection("AuthConfig").GetSection("url").Value;

                    var response = wb.UploadValues(url, "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);

                    authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseInString);

                    return authResponse.Access_Token.Trim();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return authResponse.Access_Token.Trim();
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

        public Worker Login(EmployeeEmail employeeEmail)
        {
            string formattedEndpoint = String.Format(employeelogin, employeeEmail.Email);

            var helper = new Helper(_configuration);
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + formattedEndpoint;

            var employeeWorker = new Worker();
            var workerResponse = new WorkerResponse();

            try
            {
                var webRequest = System.Net.WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + employeeEmail.Token);

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            employeeresponse = sr.ReadToEnd();
                            workerResponse = JsonConvert.DeserializeObject<WorkerResponse>(employeeresponse);
                            employeeWorker = workerResponse.value.ElementAt(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            if (workerResponse.value == null)
            {
                return null;
            }
            if (workerResponse.value.Count == 0)
            {
                return null;
            }

            return employeeWorker;
        }


        public async Task<Worker> DoLogin(EmployeeEmail employeeEmail)
        {
            string formattedEndpoint = String.Format(employeelogin, employeeEmail.Email);

            var helper = new Helper(_configuration);
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + formattedEndpoint;

            var employeeWorker = new Worker();
            var workerResponse = new WorkerResponse();

            try
            {
                var webRequest = WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + employeeEmail.Token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    employeeresponse = reader.ReadToEnd();

                    //var employeesInTerritoryResponse = new EmployeeInTerritoryListResponse();
                    workerResponse = JsonConvert.DeserializeObject<WorkerResponse>(employeeresponse);

                    employeeWorker = workerResponse.value.ElementAt(0);

                    //using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    //{
                    //    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    //    {
                    //        employeeresponse = sr.ReadToEnd();
                    //        workerResponse = JsonConvert.DeserializeObject<WorkerResponse>(employeeresponse);
                    //        employeeWorker = workerResponse.value.ElementAt(0);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            if (workerResponse.value == null)
            {
                return null;
            }
            if (workerResponse.value.Count == 0)
            {
                return null;
            }

            return employeeWorker;
        }
        public string HttpPostRequest(string url)
        {
            string postData = "";
            var postParameters = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", "c11b33c6-1e65-4e0b-adc1-bd1e5ea0cdb4" },
                { "client_secret", "6CP/?s6yHlbY=9wNG[PPl3ot=w64drqk" },
                { "tenant_id", "ba3e3cc6-09b8-455c-a25d-9ec3bc640d7e" }
            };

            foreach (string key in postParameters.Keys)
            {
                postData += HttpUtility.UrlEncode(key) + "="
                      + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string pageContent = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();

            return pageContent;
        }
    }
}
