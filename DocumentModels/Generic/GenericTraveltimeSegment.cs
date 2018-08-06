using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Database_Access_Object;
using DocumentModels.Waze;

namespace DocumentModels.Generic
{
    public class GenericTraveltimeSegment : IDocumentPOCO
    {
        [JsonProperty("id")]
        public string Id { get; set; } //id = Source + segment + timestamp
        public string Source { get; set; }
        public DocumentDate Timestamp { get; set; }
        public string SegmentName { get; set; }
        public string FromPoint { get; set; }
        public string ToPoint { get; set; }

        public Coordinate[] Coordinates; 
        public double FromLatitude { get; set; }
        public double ToLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLongitude { get; set; }

        public double Length { get; set; } //in meters
        public double Speed { get; set; }//in km/h
        public double OptimalSpeed { get; set; }//in km/h
        public int Duration { get; set; }//in seconds


        public void SetId()
        {
            if(Source == null || Timestamp == null || SegmentName == null)
            {
                throw new InvalidOperationException("Source, Timestamp or SegmentName cannot be empty");
            }

            Id = Source +"_" + Timestamp.FullDate.ToString("O") + "_" + SegmentName;
            Id = Id.Replace('/', ' ');
        }

        public void SetSpeed()
        {
            if (Length == 0 || Duration == 0)
            {
                //throw new InvalidOperationException("Length or Duration cannot be empty");
                Console.WriteLine($"Id: {Id}  Setting speed failed, Length or Duration is empty");
            }
            Speed = Length / Duration * 3.6;
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
