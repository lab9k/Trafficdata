using System;
using System.Collections.Generic;
using System.Text;
using DocumentModels.Waze;
using Newtonsoft.Json;

namespace DocumentModels.Generic
{
    public class MappedLocation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string OriginalName { get; set; }
        public string NormalizedName { get; set; }
        public List<Coordinate> Coordinates { get; set; }
        public string GoogleLocationId { get; set; }
    }
}
