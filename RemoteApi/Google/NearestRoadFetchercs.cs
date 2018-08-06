using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RemoteApi.Google
{
    public class NearestRoadFetchercs
    {
        public const string NearestRoads = "https://roads.googleapis.com/v1/nearestRoads";
        public const string LocationApi = "https://maps.googleapis.com/maps/api/place/details/json";
        public static NearestRoadResultModel GetNearestRoads(string apiKey,
            List<NearestRoadResultModel.Location> locations)
        {
            Console.WriteLine("Test: " + locations[0].latitude +" " + locations[0].latitude.ToString("G", new CultureInfo("en-US")));
            string s = locations.Aggregate("", (current, loc) => current + $"{loc.latitude.ToString("G",new CultureInfo("en-US"))},{loc.longitude.ToString("G",new CultureInfo("en-US"))}|");
             s = s.Remove(s.Length - 1);
            Console.WriteLine( s );
            Dictionary<string,string> requestArgs = new Dictionary<string, string>();
            requestArgs.Add("points", s);
            requestArgs.Add("key", apiKey);
            string json = RestClient.Get(NearestRoads, requestArgs).Result;
           
            return JsonConvert.DeserializeObject<NearestRoadResultModel>(json);

        }

        private void parseDouble(double d)
        {

        }

        public static LocationInfoModel GetRoadName(string apiKey,string placeId)
        {
           
            Dictionary<string, string> requestArgs = new Dictionary<string, string>();
            requestArgs.Add("placeid", placeId);
            requestArgs.Add("key", apiKey);
            string json = RestClient.Get(LocationApi, requestArgs).Result;

            return JsonConvert.DeserializeObject<LocationInfoModel>(json);

        }
    }
}
