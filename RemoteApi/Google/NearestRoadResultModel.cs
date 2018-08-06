using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RemoteApi.Google
{
    public class NearestRoadResultModel
    {
        public SnappedPoint[] snappedPoints;
        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class SnappedPoint
        {
            public Location location { get; set; }
            public int originalIndex { get; set; }
            public string placeId { get; set; }

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
