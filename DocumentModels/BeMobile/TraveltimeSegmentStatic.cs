using System;
using System.Collections.Generic;
using Database_Access_Object;
using Newtonsoft.Json;
using RemoteApi.Google;

namespace DocumentModels.BeMobile
{
    public class TraveltimeSegmentStatic : IDocumentPOCO, ICloneable

    {
        [JsonProperty("TrajectID")]
        public string Id { get; set; }
        public int SegmentID { get; set; }
        public int lengthmm { get; set; }
        public int optimalspeedkph { get; set; }
        public List<LocationInfoModel> googleLocationInfo { get; set; }
        public List<string> googleLocationsId { get; set; }
        public double beginnodelatitude { get; set; }
        public double beginnodelongitude { get; set; }
        public double endnodelatitude { get; set; }
        public double endnodelongitude { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public object Clone()
        {
            TraveltimeSegmentStatic _new = new TraveltimeSegmentStatic
            {
                Id = this.Id,
                SegmentID = this.SegmentID,
                lengthmm = this.lengthmm,
                beginnodelatitude = this.beginnodelatitude,
                beginnodelongitude = this.beginnodelongitude,
                endnodelatitude = this.endnodelatitude,
                endnodelongitude = this.endnodelongitude,
                optimalspeedkph = this.optimalspeedkph
            };
            return _new;
        }
    }
}
