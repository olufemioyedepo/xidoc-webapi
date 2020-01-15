using GeofencingWebApi.Models.Entities;
using GeofencingWebApi.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Serilog;
using GeofencingWebApi.Models.DTOs;

namespace GeofencingWebApi.Business
{
    public class GeolocationOperations
    {
        readonly IConfiguration _configuration;
        private string jsonResponse, territoryidbyemployeeid, territorycoordinates, coordinatesbyrecid;

        public GeolocationOperations(IConfiguration configuration)
        {
            _configuration = configuration;
            territoryidbyemployeeid = _configuration.GetSection("Endpoints").GetSection("territoryidbyemployeeid").Value;
            territorycoordinates = _configuration.GetSection("Endpoints").GetSection("territorycoordinates").Value;
            coordinatesbyrecid = _configuration.GetSection("Endpoints").GetSection("coordinatesbyrecid").Value;
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

        

        static bool IsPointInPolygon(PolygonPoint p, PolygonPoint[] polygon)
        {
            double minLongitude = polygon[0].Longitude;
            double maxLongitude = polygon[0].Longitude;
            double minLatitude = polygon[0].Latitude;
            double maxLatitude = polygon[0].Latitude;
            for (int i = 1; i < polygon.Length; i++)
            {
                PolygonPoint q = polygon[i];
                minLongitude = Math.Min(q.Longitude, minLongitude);
                maxLongitude = Math.Max(q.Longitude, maxLongitude);
                minLatitude = Math.Min(q.Latitude, minLatitude);
                maxLatitude = Math.Max(q.Latitude, maxLatitude);
            }

            if (p.Longitude < minLongitude || p.Longitude > maxLongitude || p.Latitude < minLatitude || p.Latitude > maxLatitude)
            {
                return false;
            }

            
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Latitude > p.Latitude) != (polygon[j].Latitude > p.Latitude) &&
                     p.Longitude < (polygon[j].Longitude - polygon[i].Longitude) * (p.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude)
                {
                    inside = !inside;
                }
            }

            return inside;
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

        public async Task<bool> IsAgentWithinSalesPolygon(RangeCheckParam rangeCheckParams)
        {
            bool isAgentWithinRange = false;

            var rangeGeoCoordinates = new PolygonPoint() {
                Latitude = Convert.ToDouble(rangeCheckParams.AgentLatitude),
                Longitude = Convert.ToDouble(rangeCheckParams.AgentLongitude)
            };

            // Get a list of all Territory Rec Ids that matches rangeCheckParams.EmployeeId
            var territoryCoordinates = await this.GetTerritoryCoordinates(rangeCheckParams.EmployeeId);

            foreach (var territoryCoordinate in territoryCoordinates.CoordinatesInfoItems)
            {
                var polygonPoints = new List<PolygonPoint>();
                foreach (var item in territoryCoordinate.CoordinatesInfoList)
                {
                    var point = new PolygonPoint();
                    
                    point.Latitude = Convert.ToDouble(item.Latitude);
                    point.Longitude = Convert.ToDouble(item.Longitude);
                    polygonPoints.Add(point);
                }

                var polygonPointArray = polygonPoints.ToArray();
                isAgentWithinRange = GeolocationOperations.IsPointInPolygon(rangeGeoCoordinates, polygonPointArray);
                
                // if polygonpoint check returns true for a territory/set of cooridinate point, return true and exit
                if (isAgentWithinRange == true)
                {
                    return isAgentWithinRange;
                }
            }

            return isAgentWithinRange;
        }

        public async Task<TerritoryCoordinatesList> GetTerritoryCoordinates(string employeeId)
        {
            var territoryIdResponse = new List<TerritoryId>();
            var coordinatesDetailsResponse = new List<CoordinateInfoForSave>();
            var coordinatesListResponse = new TerritoryCoordinatesList();

            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            string url = currentEnvironment + territoryidbyemployeeid;
            string formattedUrl = String.Format(url, employeeId);

            try
            {
                var webRequest = WebRequest.Create(formattedUrl);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 120000;
                    webRequest.Headers.Add("Authorization", "Bearer " + token);

                    WebResponse response = await webRequest.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    jsonResponse = reader.ReadToEnd();

                    var territoryIdListResponse = new TerritoryIdListResponse();

                    territoryIdListResponse = JsonConvert.DeserializeObject<TerritoryIdListResponse>(jsonResponse);
                    territoryIdResponse = territoryIdListResponse.value;

                    coordinatesListResponse = await GetTerritoriesCoordinatesByRecIds(territoryIdResponse);

                    response.Dispose();
                    dataStream.Close();
                    dataStream.Dispose();
                    reader.Close();
                    reader.Dispose();

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return coordinatesListResponse;
        }

        public async Task<TerritoryCoordinatesList> GetTerritoriesCoordinatesByRecIds(List<TerritoryId> territoryRecIds)
        {
            var helper = new Helper(_configuration);
            var authOperation = new AuthOperations(_configuration);

            string token = authOperation.GetAuthToken();
            string currentEnvironment = helper.GetEnvironmentUrl();
            var coordinatesList = new List<CoordinateInfoForSave>();
            var territoryCoordinatesList = new TerritoryCoordinatesList();
            territoryCoordinatesList.CoordinatesInfoItems = new List<CoordinateInfoItem>();

            foreach (var territory in territoryRecIds)
            {
                string url = currentEnvironment + coordinatesbyrecid;
                string formattedUrl = String.Format(url, territory.TerritoryRecId);

                try
                {
                    var webRequest = WebRequest.Create(formattedUrl);
                    if (webRequest != null)
                    {
                        webRequest.Method = "GET";
                        webRequest.Timeout = 120000;
                        webRequest.Headers.Add("Authorization", "Bearer " + token);

                        WebResponse response = await webRequest.GetResponseAsync();
                        // Get the stream containing all content returned by the requested server.
                        Stream dataStream = response.GetResponseStream();

                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);

                        // Read the content fully up to the end.
                        jsonResponse = reader.ReadToEnd();

                        var coordinateInfoList = new CoordinateInfoListResponse();
                        coordinateInfoList = JsonConvert.DeserializeObject<CoordinateInfoListResponse>(jsonResponse);

                        // coordinates for a single territory
                        coordinatesList = coordinateInfoList.value;
                        
                        var coordinateInfoItem = new CoordinateInfoItem();
                        coordinateInfoItem.CoordinatesInfoList = coordinatesList;

                        // adds coordinates for a single territory to the list holding all territoriies coordinates
                        territoryCoordinatesList.CoordinatesInfoItems.Add(coordinateInfoItem);

                        response.Close();
                        response.Dispose();
                        dataStream.Close();
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }


            return territoryCoordinatesList;
        }
    }
}
