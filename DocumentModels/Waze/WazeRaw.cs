using System;
using System.Collections.Generic;
using System.Text;
using Database_Access_Object;
using Newtonsoft.Json;

namespace DocumentModels.Waze
{
    public class WazeRaw : IDocumentPOCO
    {
        public string Id { get; set; }

        [JsonProperty("usersOnJams")]
        public string UsersOnJamsString;

        public UsersOnJams[] UsersOnJams => JsonConvert.DeserializeObject<UsersOnJams[]>(UsersOnJamsString);


        [JsonProperty("routes")]
        public string RoutesString { get; set; } //todo: parse to object
        public Route[] Routes => JsonConvert.DeserializeObject<Route[]>(RoutesString);

        [JsonProperty("irregularities")]
        public string IrregularitiesString { get; set; }
        public Irregularities[] Irregularities => JsonConvert.DeserializeObject<Irregularities[]>(IrregularitiesString);


        [JsonProperty("broadcasterId")]
        public int BroadcasterId { get; set; }
        [JsonProperty("areaName")]
        public string AreaName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("isMetric")]
        public bool IsMetric { get; set; }
        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }

        [JsonProperty("lengthOfJams")]
        public string LengthOfJamsString { get; set; }
        public UsersOnJams[] LengthOfJams=> JsonConvert.DeserializeObject<UsersOnJams[]>(LengthOfJamsString);

        public DateTime EventProcessedUtcTime { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Route : SubRoute
    {
        [JsonProperty("subRoutes")]
        public SubRoute[] SubRoutes;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    public class UsersOnJams
    {
        [JsonProperty("wazersCount")]
        public int WazersCount { get; set; }

        [JsonProperty("jamlevel")]
        public int JamLevel { get; set; }

        [JsonProperty("jamLength")]
        public int JamLength { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Irregularities
    {
        [JsonProperty("subRoutes")]
        public SubRoute[] SubRoutes { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class SubRoute
    {
        [JsonProperty("historicTime")]
        public int HistoricTime { get; set; }
        [JsonProperty("line")]
        public Coordinate[] Line { get; set; }
        [JsonProperty("bbox")]
        public Bbox Bbox { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("jamLevel")]
        public int JamLevel { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("subType")]
        public string SubType { get; set; }
        [JsonProperty("reportTime")]
        public string ReportTime { get; set; }
        [JsonProperty("toName")]
        public string ToName { get; set; }
        [JsonProperty("fromName")]
        public string FromName { get; set; }
        [JsonProperty("leadAlert")]
        public LeadAlert LeadAlert { get; set; }
        [JsonProperty("time")]
        public int Time { get; set; } //in s?

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class LeadAlert : IDocumentPOCO
    {

        public string city { get; set; }
        public int numThumbsUp { get; set; }
        public int reportByMood { get; set; }
        public string reportByNickname { get; set; }
        public string type { get; set; }
        public int numNotThereReports { get; set; }
        public int isLeadAlert { get; set; }
        public int numComments { get; set; }
        public string street { get; set; }
        public string subType { get; set; }
        private string _id;
        [JsonProperty("id")]
        public string Id {
            get => (this._id.Replace(@"/", "-") + reportTime);
            set => this._id = value;
        }
        public string position { get; set; }
        public long reportTime { get; set; }


        public DateTime ReportDateTime
        {
            get
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(reportTime/1000).ToLocalTime();
                return dtDateTime;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class Coordinate
    {
        [JsonProperty("x")]
        public double Longitude { get; set; }
        [JsonProperty("y")]
        public double Latitude { get; set; }
    }

    public class Bbox
    {
        [JsonProperty("minX")]
        public double MinX { get; set; }
        [JsonProperty("maxX")]
        public double MaxX { get; set; }
        [JsonProperty("minY")]
        public double MinY { get; set; }
        [JsonProperty("maxY")]
        public double MaxY { get; set; }
    }

}
