using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Database_Access_Object;
using DocumentModels.BeMobile;
using RemoteApi.Google;

namespace TraveltimesDocumentCreator
{
    public class BemStaticAggregator
    {

       
        public static void AggregateStaticData(string GoogleApiKey, string inputEndpoint, string inputKey, string inputDb, string inputColl, string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null)
        {
            //initialize possibly empty variables

            outputColl = outputColl ?? inputColl;
            outputDb = outputDb ?? inputDb;
            outputEndpoint = outputEndpoint ?? inputEndpoint;
            outputKey = outputKey ?? inputKey;

            Console.WriteLine("Initializing Input Database");
            var staticInfoQuerier = new QueryManager<TraveltimeSegmentStatic>
            {
                CollectionName = inputColl,
                DatabaseKey = inputKey,
                DatabaseName = inputDb,
                DatabaseUri = inputEndpoint
            };

            staticInfoQuerier.Init();

            Console.WriteLine("Initializing Output Database");
            var outputManager = new QueryManager<TraveltimeStatic>()
            {
                CollectionName = outputColl,
                DatabaseKey = outputKey,
                DatabaseName = outputDb,
                DatabaseUri = outputEndpoint
            };
            outputManager.Init();

            Console.WriteLine("Processing Transformations..");
            List<TraveltimeStatic> toProcess = GeTraveltimeStatic(GetAllSegmentIds(staticInfoQuerier), staticInfoQuerier);
            Console.WriteLine("Google recently changed its places API model, loading locationsinfo currently disabled");
            //Google location info disabled
           // List<TraveltimeStatic> toProces = GetGeoLocation(GeTraveltimeStatic(GetAllSegmentIds(staticInfoQuerier), staticInfoQuerier),
             //       GoogleApiKey);

            foreach (var r in toProcess)//FillLocationInfoFromLocationId(toProcess, GoogleApiKey))
            {
                outputManager.Create(r).Wait();
            }
        }

        public static List<TraveltimeStatic> GetGeoLocation(List<TraveltimeStatic> toProcess, string apiKey)
        {
            int c = 0;
            while (c < toProcess.Count)
            {
                List<TraveltimeStatic> slice =
                    toProcess.GetRange(c, (c + 100 < toProcess.Count) ? 100 : (toProcess.Count - c));

                List<NearestRoadResultModel.Location> locations = new List<NearestRoadResultModel.Location>();
                foreach (var travelTime in slice)
                {
                    locations.Add( new NearestRoadResultModel.Location
                    {
                        latitude = travelTime.Merged.beginnodelatitude,
                        longitude = travelTime.Merged.beginnodelongitude
                    });
                }
                NearestRoadResultModel results = NearestRoadFetchercs.GetNearestRoads(apiKey, locations);
                for (int i = 0; i < results.snappedPoints.Length; i++)
                {
                    int originalIndex = results.snappedPoints[i].originalIndex;
                    if (toProcess[c + originalIndex].Merged.googleLocationsId == null)
                    {
                        toProcess[c + originalIndex].Merged.googleLocationsId = new List<string>();
                    }
                    toProcess[c + originalIndex].Merged.googleLocationsId.Add(results.snappedPoints[i].placeId);
                }
                c += slice.Count;
            }
            return toProcess;
        }
        
        public static List<TraveltimeStatic> FillLocationInfoFromLocationId(List<TraveltimeStatic> toProcess, string apiKey)
        {
            foreach(TraveltimeStatic ts in toProcess)
            {
                ts.Merged.googleLocationInfo = new List<LocationInfoModel>();
                foreach (string locId in ts.Merged.googleLocationsId)
                {
                    ts.Merged.googleLocationInfo.Add(NearestRoadFetchercs.GetRoadName(apiKey,locId));
                }
            }
            return toProcess;
        }

        public static List<string> GetAllSegmentIds(QueryManager<TraveltimeSegmentStatic> staticInfoQuerier)
        {
            List<TraveltimeSegmentStatic> temp = staticInfoQuerier.GetAllResults("select c.TrajectID FROM c"); //distinct keywork does not work
            List<string> uniqueSegments = new List<string>();

            foreach (var segment in temp)
            {
                if (!uniqueSegments.Contains(segment.Id))
                {
                    uniqueSegments.Add(segment.Id);
                }
            }
            return uniqueSegments;
        }

        public static  List<TraveltimeStatic> GeTraveltimeStatic(List<string> segmentIds, QueryManager<TraveltimeSegmentStatic> staticInfoQuerier) 
        {
            List<TraveltimeStatic> routes = new List<TraveltimeStatic>();

            foreach (string s in segmentIds)
            {
                List < TraveltimeSegmentStatic > segments=
                    staticInfoQuerier.GetAllResults($"select * from c where c.TrajectID = '{s}'");
                routes.Add(new TraveltimeStatic(segments));
            }
            return routes;
        }


    }
}
