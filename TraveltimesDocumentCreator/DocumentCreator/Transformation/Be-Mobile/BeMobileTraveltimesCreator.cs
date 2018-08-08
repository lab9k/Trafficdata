using Database_Access_Object;
using DocumentModels.BeMobile;
using DocumentModels.Generic;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using RemoteApi.Google;
using System.Threading.Tasks;

namespace TraveltimesDocumentCreator
{
    public class BeMobileTraveltimesCreator : GenericTraveltimesCreator<TraveltimeSegment>
    {

        private static int THREADS = 12;
        
        public static void CreateMergedDocuments(string[] segments, string inputEndpoint, string inputKey, string inputDb, string inputCollStatic,string inputCollDynamic,
            string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null, int threads = 12)
        {
            THREADS = threads;
            //initialize possibly empty variables

            outputColl = outputColl ?? inputCollStatic;
            outputDb = outputDb ?? inputDb;
            outputEndpoint = outputEndpoint ?? inputEndpoint;
            outputKey = outputKey ?? inputKey;

            Console.WriteLine("Initializing Input Database");
            var staticInfoQuerier = new QueryManager<TraveltimeStatic>
            {
                CollectionName = inputCollStatic,
                DatabaseKey = inputKey,
                DatabaseName = inputDb,
                DatabaseUri = inputEndpoint
            };

            staticInfoQuerier.Init();

            var dynamicInfoQuerier = new QueryManager<TraveltimeSegment>
            {
                CollectionName = inputCollDynamic,
                DatabaseKey = inputKey,
                DatabaseName = inputDb,
                DatabaseUri = inputEndpoint
            };

            dynamicInfoQuerier.Init();

            Console.WriteLine("Initializing Output Database");
            PersistDocuments = new QueryManager<GenericTraveltimeSegment>()
            {
                CollectionName = outputColl,
                DatabaseKey = outputKey,
                DatabaseName = outputDb,
                DatabaseUri = outputEndpoint
            };
            PersistDocuments.Init();
            Console.WriteLine("Starting transformations");
            TransformAndPersist(dynamicInfoQuerier,staticInfoQuerier,Verification.CheckExisting, segments);

        }

        public static  List<GenericTraveltimeSegment> Transform(QueryManager<TraveltimeStatic> staticInfoQuerier, List<TraveltimeSegment> input)
        {
            List<GenericTraveltimeSegment> _new = new List<GenericTraveltimeSegment>();
            foreach (TraveltimeSegment segment in input)
            {
                TraveltimeSegmentStatic merged = staticInfoQuerier.GetAllResults($"SELECT c.Merged FROM c where c.Merged.TrajectID = '{segment.Id}'")[0].Merged;

                _new.Add(MergeDocuments(merged, segment));

            }
            Console.WriteLine($"Transformed {_new.Count} Be-Mobile documents to generic traveltimes");
            return _new;
        }

        public static  GenericTraveltimeSegment MergeDocuments(TraveltimeSegmentStatic staticData, TraveltimeSegment dynamicData)
        {
            if (staticData.Id != dynamicData.Id)
            {
                throw new InvalidOperationException("Documents do not match");
            }

            GenericTraveltimeSegment _new = new GenericTraveltimeSegment
            {
                Source = "Be-Mobile",
                Timestamp = new DocumentDate((DateTime) dynamicData.Timestamp),
                SegmentName = staticData.Id,
                FromLatitude = staticData.beginnodelatitude,
                ToLatitude = staticData.endnodelatitude,
                FromLongitude = staticData.beginnodelongitude,
                ToLongitude = staticData.endnodelongitude,
                Length = staticData.lengthmm / 1000.0,
                Duration = dynamicData.durationInMs / 1000 ,
                OptimalSpeed = staticData.optimalspeedkph
            };

            _new.SetSpeed();
            _new.SetId();
            _new = FillLocationInfo(staticData, _new);

            return _new;
        }

        private static GenericTraveltimeSegment FillLocationInfo(TraveltimeSegmentStatic staticData, GenericTraveltimeSegment dynamicData)
        {
            List<LocationInfoModel.AddressComponent> components =
                staticData.googleLocationInfo[0].result.address_components;
            LocationInfoModel.AddressComponent routeComponent = components[1];
            int index = 0;
            while (!routeComponent.types.Contains("route") && index < components.Count)
            {
                routeComponent = components[index];
            }
            dynamicData.FromPoint = routeComponent.long_name;
            dynamicData.ToPoint = routeComponent.long_name;
            return dynamicData;
        }

        public static void TransformAndPersist(QueryManager<TraveltimeSegment> queryManager, QueryManager<TraveltimeStatic> staticInfoQuerier, Verification verifyType, string[] segmentIds,DateTime? startDate = null)
        {
           foreach (string segment in segmentIds)
            {
                Console.WriteLine("Processing documents for segment: " + segment);
                TransformationTask(queryManager, staticInfoQuerier, verifyType, segment, startDate);
            }


        }

        private static void TransformationTask(QueryManager<TraveltimeSegment> queryManager, QueryManager<TraveltimeStatic> staticInfoQuerier, Verification verifyType,string segment, DateTime? startDate = null)
        {
            Console.WriteLine($"Fetching documents for segment {segment}, this will take a while...");
            List<TraveltimeSegment> input = queryManager.GetAllResults($"select * from c where c.TrajectID = '{segment}'");
            Console.WriteLine($"Processing segment: {segment}, fetched {input.Count} documents");

            // Console.WriteLine($"Fetched {input.Count} documents");

            //Console.WriteLine("Processing transformations...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int[] res = ProcessMerges(input, verifyType, staticInfoQuerier);
            stopwatch.Stop();

            Console.WriteLine($"Processing finished for segment {segment} (thread: {Thread.CurrentThread.ManagedThreadId}) in {stopwatch.Elapsed}: {res[0]} succeeded, {res[1]} failed, {res[2]} skipped");

        }


        private static Dictionary<string, TraveltimeSegmentStatic> staticData;
        private static Object lockObject = new Object();

        private static TraveltimeSegmentStatic GetStaticData(string trajectId, QueryManager<TraveltimeStatic> staticInfoQuerier)
        {
            lock (lockObject)
            {
                if (staticData == null)
                {
                    staticData = new Dictionary<string, TraveltimeSegmentStatic>();
                    Console.WriteLine("Static data not found, populating static data...");
                    List<TraveltimeStatic> staticDataList = staticInfoQuerier.GetAllResults("SELECT c.Merged FROM c ");

                    foreach (TraveltimeStatic data in staticDataList)
                    {
                        staticData.Add(data.Merged.Id, data.Merged);
                    }
                    Console.WriteLine(" Done");
                }
            }
            
            return staticData[trajectId];
        }




        private static async Task<int[]> ProcessMergesAsync(List<TraveltimeSegment> input, Verification verifyType, QueryManager<TraveltimeStatic> staticInfoQuerier)
        {
            Console.WriteLine("Processing merges");

            int itemsPerThread = input.Count / THREADS;
            int currentIndex = 0;
            List<Task> tasks = new List<Task>();
            List<int[]> resultCounts = new List<int[]>();
            while(currentIndex < input.Count)
            {
                int itemCount = currentIndex + itemsPerThread < input.Count ? itemsPerThread : input.Count - currentIndex;
                List<TraveltimeSegment> slicedInput = input.GetRange(currentIndex, itemsPerThread);
                int[] results = await ProcessMergesAsync(input, verifyType, staticInfoQuerier);
                resultCounts.Add(results);
                currentIndex += itemCount;
            }
            //while(tasks.Count > 0)
            //{
            //    await Task.WhenAny(tasks);
                //for (int i = 0; i < tasks.Count;i++)
                //{
                //    if (tasks[i].IsCompleted)
                //    {
                //        Console.WriteLine($"Task {i}: {tasks[i].Status}");
                //        tasks.RemoveAt(i);
                //    }

               // }
            //}

            int[] totalResultCounts = { 0, 0, 0 };
            foreach(int[] i in resultCounts)
            {
                for(int j = 0; j < totalResultCounts.Length; j++)
                {
                    if(j < i.Length)
                        totalResultCounts[j] += i[j];
                }
            }
            return totalResultCounts;

        }

        public static int[] ProcessMerges(List<TraveltimeSegment> input, Verification verifyType, QueryManager<TraveltimeStatic> staticInfoQuerier)
        {
            int[] resultCount = { 0, 0, 0 }; //Success, Failed, Skipped

            foreach (TraveltimeSegment segment in input)
            {

                TraveltimeSegmentStatic merged = GetStaticData(segment.Id, staticInfoQuerier);
                GenericTraveltimeSegment result = MergeDocuments(merged, segment);

                if (verifyType == Verification.CheckExisting)
                {
                    try
                    {
                        PersistDocuments.Get(result.Id).Wait();
                        resultCount[2]++;
                        // Console.WriteLine($"Merged document {result.Id} not persisted in the database, conflict found.");
                    }
                    catch (Exception ex)
                    {
                        resultCount[0]++;
                        PersistResultDocuments(result);
                        //Console.WriteLine($"Merged document {result.Id} persisted in the database");


                    }
                }
                else
                {
                    resultCount[0]++;
                    PersistResultDocuments(result);
                    //Console.WriteLine($"Merged document {result.Id} persisted in the database");
                }
            }
            return resultCount;
        }
        

        public BeMobileTraveltimesCreator(string outputCollection) : base(outputCollection)
        {
        }
    }
}