using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Util
{
    public class Helper
    {
        private string environment;
        private readonly IConfiguration Configuration;
        private static Random random = new Random();

        public Helper(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public string GetEnvironment()
        {
            string currentenvironment = Configuration.GetSection("Environments").GetSection("Current").Value;
            return currentenvironment;
        }

        public string GetEnvironmentUrl()
        {
            environment = this.GetEnvironment();
            string environmentUrl = Configuration.GetSection("Environments").GetSection(environment).Value;
            return environmentUrl;
        }

        public static string GenerateUniqueKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrtuvyxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
