using Database_Access_Object;
using DocumentModels.BeMobile;
using DocumentModels.Generic;
using Microsoft.Extensions.Logging;
using RemoteApi.Google;
using System;
using System.Collections.Generic;
using System.Text;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile
{
    public class BeMobileDataNormalization : IDataNormalizer
    {

        private ILogger<BeMobileDataNormalization> _logger;
        private IBlockingQueue<GenericQueueTask<SegmentTaskModel>> _segmentQueue;
        private IBlockingQueue<GenericQueueTask<TrajectTaskModel>> _trajectQueue;

        public BeMobileDataNormalization(ILogger<BeMobileDataNormalization> logger, IBlockingQueue<GenericQueueTask<SegmentTaskModel>> segmentQueue, IBlockingQueue<GenericQueueTask<TrajectTaskModel>> trajectQueue)
        {
            _logger = logger;
            _segmentQueue = segmentQueue;
            _trajectQueue = trajectQueue;
        }                    

        public void StartNormalization(string inputEndpoint, string inputKey, string inputDb, string inputCollStatic, string inputCollDynamic,string[] segments = null,
            string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null)
        {
            if(_segmentQueue?.Queue == null || _trajectQueue?.Queue == null)
            {
                throw new InvalidOperationException("Consumer Queues or Consumers are not running, check if they are added to the startup file");
            }

            _logger.LogInformation("Starting Normalization Task");
            _logger.LogInformation("Step 1: Loading static data");            
            List<TraveltimeStatic> staticData = GetQueryManager<TraveltimeStatic>(inputEndpoint,inputKey,inputDb,inputCollStatic).GetAllResults("SELECT c.Merged FROM c ");
            _logger.LogInformation($"{staticData.Count} staticData found");

            
            HashSet<string> trajects = new HashSet<string>();
            if(segments == null || segments.Length == 0)
            {
                foreach (TraveltimeStatic t in staticData)
                {
                    _logger.LogInformation($" static id -> {t}");

                    trajects.Add(t.Merged.Id);
                }
            }
            else
            {
                foreach(string s in segments)
                {
                    trajects.Add(s);
                }
            }
            _logger.LogInformation("Step 2: Fetching Dynamic data and start merging->");
            QueryManager<TraveltimeSegment> inputQueryManager = GetQueryManager<TraveltimeSegment>(inputEndpoint, inputKey, inputDb, inputCollDynamic);
            QueryManager<GenericTraveltimeSegment> outputQueryManager = GetQueryManager<GenericTraveltimeSegment>(outputEndpoint, outputKey, outputDb, outputColl);
            _logger.LogInformation($"{trajects.Count} trajects found");
            foreach (string s in trajects)
            {
                _logger.LogInformation($"    for traject {s}");

                _trajectQueue.Queue.Add(new GenericQueueTask<TrajectTaskModel>
                {
                    Function = NormalizeTraject,
                    Item = new TrajectTaskModel
                    {
                        InputQueryManager = inputQueryManager,
                        OutputQueryManager = outputQueryManager,
                        StaticData = staticData,
                        Segment = s
                    }

                });

            }
        }

        public void NormalizeTraject(TrajectTaskModel model)
        {
            List<TraveltimeSegment> input = model.InputQueryManager.GetAllResults($"select * from c where c.TrajectID = '{model.Segment}'");
            _logger.LogInformation($"Found {input.Count} documents to process for traject {model.Segment}");

            foreach(TraveltimeSegment t in input)
            {
                SegmentTaskModel segmentModel = new SegmentTaskModel
                {
                    OutputQueryManager = model.OutputQueryManager,
                    TraveltimeStatic = model.StaticData.Find(x => x.Merged.Id == t.Id),
                    TraveltimeSegment = t
                };
                _segmentQueue.Queue.Add(new GenericQueueTask<SegmentTaskModel>
                {
                    Function = ProcessAndUploadSegment,
                    Item = segmentModel
                });

            }
            _trajectQueue.Finished += 1;
        }

        public async void ProcessAndUploadSegment(SegmentTaskModel model)
        {
            GenericTraveltimeSegment merged = NormalizeSegment(model.TraveltimeSegment, model.TraveltimeStatic);
            try
            {
                await model.OutputQueryManager.Create(merged,0,10);
                _segmentQueue.Finished += 1;
            }
            catch (Microsoft.Azure.Documents.DocumentClientException ex)
            {
                //TODO: handle exception

            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                _logger.LogWarning($"A task has been canceled: {ex.Task.Id}");
                _segmentQueue.Failed += 1;
            }
            catch (System.OperationCanceledException ex)
            {
                _logger.LogWarning($"An operation has been canceled: {ex.Message}");
            }
        }



        private QueryManager<T> GetQueryManager<T>(string uri, string key, string database, string collection)
        {
            QueryManager<T> q =  new QueryManager<T>
            {
                CollectionName = collection,
                DatabaseKey = key,
                DatabaseName = database,
                DatabaseUri = uri
            };
            q.Init();
            return q;
        }
        /// <summary>
        /// Merge static and dynamic BE-Mobile data to a single document
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="staticData"></param>
        /// <returns></returns>
        public GenericTraveltimeSegment NormalizeSegment(TraveltimeSegment segment, TraveltimeStatic staticSegment)
        {
            TraveltimeSegmentStatic staticData = staticSegment.Merged;
            if (staticData.Id != segment.Id)
            {
                throw new InvalidOperationException("Documents do not match");
            }

            GenericTraveltimeSegment _new = new GenericTraveltimeSegment
            {
                Source = "Be-Mobile",
                Timestamp = new DocumentDate((DateTime)segment.Timestamp),
                SegmentName = staticData.Id,
                FromLatitude = staticData.beginnodelatitude,
                ToLatitude = staticData.endnodelatitude,
                FromLongitude = staticData.beginnodelongitude,
                ToLongitude = staticData.endnodelongitude,
                Length = staticData.lengthmm / 1000.0,
                Duration = segment.durationInMs / 1000,
                OptimalSpeed = staticData.optimalspeedkph
            };

            _new.SetSpeed();
            _new.SetId();
            _new = FillLocationInfo(staticData, _new);

            return _new;
        }

        /// <summary>
        /// Fills Location info
        /// </summary>
        /// <param name="staticData"></param>
        /// <param name="dynamicData"></param>
        /// <returns></returns>
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

    }
}
