using System;
using Database_Access_Object;
using Newtonsoft.Json;

namespace DocumentModels.BeMobile
{
    public class TraveltimeSegment :IDocumentPOCO
    {
        [JsonProperty(PropertyName = "TrajectID")]
        public string Id { get; set; }
        public int segmentID { get; set; }

        public string timestamp { get; set; } //problem with automatic parsing in database

        public DateTime? Timestamp
        {
            get
            {
                if (timestamp == null)
                {
                    return null;
                }
                try
                {
                    return DateTime.ParseExact(timestamp, "dd/MM/yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (System.FormatException ex)
                {
                    return DateTime.ParseExact(timestamp, "dd/MM/yyyy H:mm",
                        System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        } 

        [JsonProperty(PropertyName = "travel time (ms)")]
        public int durationInMs { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
