using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Database_Access_Object;
using DocumentModels.Generic;
using DocumentModels.Waze;
using Microsoft.Azure.Documents;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Waze
{
    public class WazeAlertCreator 
    {
    
       

        public static void CreateAlertDocuments(string inputEndpoint, string inputKey, string inputDb, string inputColl, string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null)
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
            var PersistDocuments = new QueryManager<LeadAlert>()
            {
                CollectionName = outputColl,
                DatabaseKey = outputKey,
                DatabaseName = outputDb,
                DatabaseUri = outputEndpoint
            };
            PersistDocuments.Init();

            Console.WriteLine("Processing Transformations..");

            TransformAndPersistAlerts(Verification.CheckExisting, wazeQueryManager,PersistDocuments);

        }

        public static void TransformAndPersistAlerts(Verification verifyType, QueryManager<WazeRaw> wazeQueryManager, QueryManager<LeadAlert> Persist,DateTime? startDate = null)
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
                foreach (var route in input.Routes)
                {
                    if (route.LeadAlert != null)
                        PersistResultDocuments(route.LeadAlert, Persist);
                    if (route.SubRoutes != null)
                        {
                            foreach (var subroute in route.SubRoutes)
                            {
                                if (subroute.LeadAlert != null)
                                    PersistResultDocuments(subroute.LeadAlert, Persist);
                            }
                        }

                }
                foreach (var route in input.Irregularities)
                {
                    if (route.SubRoutes != null)
                    {
                        foreach (var subroute in route.SubRoutes)
                        {
                            if (subroute.LeadAlert != null)
                                PersistResultDocuments(subroute.LeadAlert, Persist);
                        }
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Processing finished in {stopwatch.Elapsed}: {added} added, {replaced} replaced and {ignored} ignored");
        }

        protected static void PersistResultDocuments(LeadAlert doc, QueryManager<LeadAlert> PersistDocuments, int tryCount = 0)
        {

            try
            {
                PersistDocuments.Create(doc).Wait();
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine("Error occured:" + ex.Error);
                Console.WriteLine(
                    $"Retrycount: {tryCount}, {(tryCount <= 10 ? String.Format("retrying in 1 second") : String.Format("skipping document"))}");
                if (tryCount < 10)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (tryCount < 10)
                        PersistResultDocuments(doc, PersistDocuments, tryCount++);
                }
            }
            catch (System.AggregateException ex)
            {
                if (ex.InnerExceptions[0] is DocumentClientException && tryCount < 10)
                {
                    Random r = new Random();
                    doc.Id += r.Next(100);
                    PersistResultDocuments(doc, PersistDocuments, ++tryCount);
                }
            }


        }

    }
}
