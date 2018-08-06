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
        
        public static void CreateMergedDocuments(string inputEndpoint, string inputKey, string inputDb, string inputCollStatic,string inputCollDynamic,
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
            TransformAndPersist(dynamicInfoQuerier,staticInfoQuerier,Verification.CheckExisting);

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

        public static void TransformAndPersist(QueryManager<TraveltimeSegment> queryManager, QueryManager<TraveltimeStatic> staticInfoQuerier, Verification verifyType, DateTime? startDate = null)
        {

            string[] segmentIds = { "B_OW", "B_WO", "C_NZ", "C_ZN", "D_NZ", "D_ZN", "E_NZ", "E_ZN", "F_NZ", "F_ZN", "G_NZ", "G_ZN", "H_NZ", "H_ZN", "I_OW", "I_WO", "Invalsweg1", "Invalsweg10", "Invalsweg11", "Invalsweg12", "Invalsweg13", "Invalsweg14", "Invalsweg15", "Invalsweg16", "Invalsweg17", "Invalsweg18", "Invalsweg19", "Invalsweg2", "Invalsweg20_stadinwaarts", "Invalsweg3", "Invalsweg4", "Invalsweg5", "Invalsweg6", "Invalsweg7", "Invalsweg8", "Invalsweg9", "J_OW", "J_WO", "K_NZ", "K_ZN", "L_NZ", "L_ZN", "M_NZ", "M_ZN", "N_NZ", "N_ZN", "O_OW", "O_WO", "P_OW", "P_WO", "R40a_OW", "R40a_WO", "R40wijzerzin", "Uitvalsweg20_staduitwaarts" }; //Todo: add to config file


            foreach (string segment in segmentIds)
            {
                TransformationTask(queryManager, staticInfoQuerier, verifyType, segment, startDate);
            }


        }

        private static void TransformationTask(QueryManager<TraveltimeSegment> queryManager, QueryManager<TraveltimeStatic> staticInfoQuerier, Verification verifyType,string segment, DateTime? startDate = null)
        {
            //Console.WriteLine("Fetching documents for segment, this will take a while...");
            List<TraveltimeSegment> input = queryManager.GetAllResults($"select * from c where c.TrajectID = '{segment}'");
            Console.WriteLine($"Processing segment: {segment}, fetched {input.Count} documents");

            // Console.WriteLine($"Fetched {input.Count} documents");

            //Console.WriteLine("Processing transformations...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int[] res = ProcessMergesAsync(input, verifyType, staticInfoQuerier).Result;
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
                int[] resultCount= { };                
                tasks.Add(Task.Run(() => ProcessMerges(slicedInput, verifyType, staticInfoQuerier,ref resultCount)));
                resultCounts.Add(resultCount);
                currentIndex += itemCount;
            }
            await Task.WhenAll(tasks);
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

        public static void ProcessMerges(List<TraveltimeSegment> input, Verification verifyType, QueryManager<TraveltimeStatic> staticInfoQuerier, ref int[] resultCount)
        {
            Console.WriteLine("Started processing in thread: " + Thread.CurrentThread.ManagedThreadId);
            resultCount = new int[]{ 0, 0, 0 }; //Success, Failed, Skipped

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
            Console.WriteLine($"Processing in thread {Thread.CurrentThread.ManagedThreadId} finished.");
        }
        

        public BeMobileTraveltimesCreator(string outputCollection) : base(outputCollection)
        {
        }
    }
}