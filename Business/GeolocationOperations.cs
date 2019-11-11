using GeofencingWebApi.Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Business
{
    public class GeolocationOperations
    {
        readonly IConfiguration _configuration;

        public GeolocationOperations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private double CalculateDistance(double currentPostionLatitude, double currentPostionLongitude, double agentLatitude, double agentLongitude, double radius)
        {
            if ((currentPostionLatitude == agentLatitude) && (currentPostionLongitude == agentLongitude))
            {
                return 0;
            }
            else
            {
                double theta = currentPostionLongitude - agentLongitude;
                double dist = Math.Sin(Deg2rad(currentPostionLatitude)) * Math.Sin(Deg2rad(agentLatitude)) + Math.Cos(Deg2rad(currentPostionLatitude)) * Math.Cos(Deg2rad(agentLatitude)) * Math.Cos(Deg2rad(theta));
                double allowabledistance;

                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515;

                dist = dist * 1.609344; // kilometers
                allowabledistance = dist * 1000; // convert to meters
                //allowabledistance += 10; // add 10 meters 

                return allowabledistance;
            }
        }

        public static double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        private  double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private  double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public bool IsAgentWithinRange(FullGeolocationParameters geolocationParameters)
        {
            double _radius = geolocationParameters.Radius;
            double _currPosLat = Convert.ToDouble(geolocationParameters.CurrentGeolocationLatitude);
            double _currPosLong = Convert.ToDouble(geolocationParameters.CurrentGeolocationLongitude);
            double _agentLat = Convert.ToDouble(geolocationParameters.AgentLatitude);
            double _agentLong = Convert.ToDouble(geolocationParameters.AgentLongitude);

            double distance = GetDistance(_currPosLat, _currPosLong, _agentLat, _agentLong);
            //distance += 40; // cut them a slack of extra 10 meters. Winks!!!

            if (distance <= _radius)
            {
                return true;
            }

            return false;
        }
    }
}
