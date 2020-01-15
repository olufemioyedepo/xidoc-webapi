using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class GeolocationParameters
    {
        public string CurrentGeolocationLatitude { get; set; }
        public string CurrentGeolocationLongitude { get; set; }
        public string HcmWorkerRecId { get; set; }
        //public string AgentLatitude { get; set; }
        //public string AgentLongitude { get; set; }
        //public double Radius { get; set; }
    }

    public class FullGeolocationParameters
    {
        public string CurrentGeolocationLatitude { get; set; }
        public string CurrentGeolocationLongitude { get; set; }
        public string HcmWorkerRecId { get; set; }
        public string AgentLatitude { get; set; }
        public string AgentLongitude { get; set; }
        public double Radius { get; set; }
    }

    public class RangeCheckParam
    {
        public string EmployeeId { get; set; }
        public string AgentLatitude { get; set; }
        public string AgentLongitude { get; set; }

    }
}
