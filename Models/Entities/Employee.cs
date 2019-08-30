using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class Employee
    {
        public string Name { get; set; }
        public string PersonnelNumber { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string SalesAgentLongitude { get; set; }
        public string SalesAgentLatitude { get; set; }
        public float CoverageRadius { get; set; }
        public float OutOfCoverargeLimit { get; set; }
        public float CommissionPercentageRate { get; set; }
        public string AgentLocation { get; set; }
    }
}
