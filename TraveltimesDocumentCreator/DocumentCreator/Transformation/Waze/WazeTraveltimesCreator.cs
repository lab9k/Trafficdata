using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Database_Access_Object;
using DocumentModels.Generic;
using DocumentModels.Waze;
using Microsoft.Azure.Documents;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Waze
{
    public class WazeTraveltimesCreator : GenericTraveltimesCreator<WazeRaw>
    {
        
        public static void CreateMergedDocuments(string inputEndpoint, string inputKey, string inputDb, string inputColl, string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null)
        {
            //initialize possibly empty variables

            outputColl = outputColl ?? inputColl;
            outputDb = outputDb ?? inputDb;
            outputEndpoint = outputEndpoint ?? inputEndpoint;
            outputKey = outputKey ?? inputKey;

            Console.WriteLine("Initializing Input Database");
            var wazeQueryManager = new QueryManager<WazeRaw>
            {
                CollectionName = inputColl,
                DatabaseKey = inputKey,
                DatabaseName = inputDb,
                DatabaseUri = inputEndpoint
            };

            wazeQueryManager.Init();

            Console.WriteLine("Initializing Output Database");
            PersistDocuments= new QueryManager<GenericTraveltimeSegment>()
            {
                CollectionName = outputColl,
                DatabaseKey = outputKey,
                DatabaseName = outputDb,
                DatabaseUri = outputEndpoint
            };
            PersistDocuments.Init();

            Console.WriteLine("Processing Transformations..");

            TransformAndPersist(Verification.CheckExisting, wazeQueryManager);

        }

       
        public static void TransformAndPersist(Verification verifyType, QueryManager<WazeRaw> wazeQueryManager, DateTime? startDate = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Fetching documents to process..");
            List<WazeRaw> toProcess = wazeQueryManager.GetAll();
            Console.WriteLine($" {toProcess.Count} documents found!");


            int added = 0, replaced = 0, ignored = 0;
            Console.WriteLine("Processing document, this may take a while..");
            foreach (WazeRaw input in toProcess)
            {
                foreach (GenericTraveltimeSegment segment in Transform(input))
                {
                    if (!DocExists(segment) || (verifyType != Verification.CheckExisting && verifyType != Verification.ReplaceExisting))
                    {

                        PersistResultDocuments(segment, 0);
                        //Console.WriteLine($"Document added : {segment.Id}");
                        added++;
                    }
                    else if (verifyType == Verification.ReplaceExisting)
                    {
                        ReplaceResultDocuments(segment,5);
                        //Console.WriteLine($"Document Replaced : {segment.Id}");
                        replaced++;
                    }
                    else
                    {
                        //Console.WriteLine($"Document not added; did exist : {segment.Id}");
                        ignored++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Processing finished in {stopwatch.Elapsed}: {added} added, {replaced} replaced and {ignored} ignored");
        }

        private static bool DocExists(IDocumentPOCO doc)
        {
            try
            {
                PersistDocuments.Get(doc.Id).Wait();
                return true;
            }
            catch (Exception ex) //zou moeten DocumentClientExpception zijn bij notfound, maar is AggregateException
            {
                //if (ex.StatusCode == HttpStatusCode.NotFound) //werkt enkel bij DocumentClienException
                {
                    return false;
                }
                return true;
            }
        }

        private static List<GenericTraveltimeSegment> Transform(WazeRaw input)
        {
            List<GenericTraveltimeSegment> _new = new List<GenericTraveltimeSegment>();
            foreach (Route route in input.Routes)
            {
                _new.Add(CreateSegmentFromRoute(route,input.UpdateTime));
                if (route?.SubRoutes != null && route.SubRoutes.Length > 0)
                {
                    foreach (SubRoute subRoute in route.SubRoutes)
                    {
                        _new.Add(CreateSegmentFromRoute(subRoute, input.UpdateTime));
                    }
                }
               
            }
            return _new;
        }

        private static GenericTraveltimeSegment CreateSegmentFromRoute(SubRoute subRoute, long UpdateTime) { 
            GenericTraveltimeSegment segment = new GenericTraveltimeSegment
            {
                Source = "Waze",
                SegmentName = subRoute.FromName + ";" + subRoute.ToName,
                FromPoint = subRoute.FromName,
                ToPoint = subRoute.ToName,
                Timestamp = new DocumentDate(Util.GetDateTimeFromUnixInMillies(UpdateTime)),
                Coordinates = subRoute.Line,
                Length = subRoute.Length,
                Duration = subRoute.Time,
                OptimalSpeed = subRoute.Length / (double)subRoute.HistoricTime * 3.6
            };

            try
            {

                segment.FromLatitude = subRoute.Line[0]?.Latitude ?? 0;
                segment.FromLongitude = subRoute.Line[0]?.Longitude ?? 0;
                segment.ToLatitude = subRoute.Line[subRoute.Line.Length - 1]?.Latitude ?? 0;
                segment.ToLongitude = subRoute.Line[subRoute.Line.Length - 1]?.Longitude ?? 0;
            }
            catch (Exception e)
            {
                //do nothing values stay at 0

               // Console.WriteLine("Index out of range!");
            }

            segment.SetId();
            segment.SetSpeed();
            return segment;
        }

        public WazeTraveltimesCreator(string outputCollection) : base(outputCollection)
        {
        }
    }
}
