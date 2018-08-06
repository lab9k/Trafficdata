using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DocumentModels.Generic
{
    public class Segment
    {
        public string FromPoint { get; set; }
    }
    public class TraveltimeSegments
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("FromPoints")]
        public Segment[] FromPoints { get; set; }

        public string[] Segments
        {
            get
            {
                string[] strings = new string[FromPoints.Length];
                for (var i = 0; i < FromPoints.Length; i++)
                {
                    strings[i] = FromPoints[i].FromPoint;
                }
                return strings;

            }
        }
    }
}
