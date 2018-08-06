using Database_Access_Object;
using DocumentModels.Generic;

namespace TraveltimesDocumentCreator.DocumentCreator.Aggregation.Location
{
    public class LocationMapper
    {
        public void MapLocationsFromCollection(string inputEndpoint, string inputKey, string inputDb, string inputColl, string outputColl = null, string outputDb = null, string outputEndpoint = null, string outputKey = null)
        {
            //initialize possibly empty variables
            if (outputColl == null)
            {
                outputColl = inputColl;
                if (outputDb == null)
                {
                    outputDb = inputDb;
                    if (outputEndpoint == null)
                    {
                        outputEndpoint = inputEndpoint;
                        if (outputKey == null)
                        {
                            outputKey = inputKey;
                        }
                    }
                }
            }

            var inputManager = new QueryManager<GenericTraveltimeSegment>()
            {
                CollectionName = inputColl,
                DatabaseKey = inputKey,
                DatabaseName = inputDb,
                DatabaseUri = inputEndpoint
            };
            inputManager.Init();

            var outputManager = new QueryManager<MappedLocation>()
            {
                CollectionName = outputColl,
                DatabaseKey = outputKey,
                DatabaseName = outputDb,
                DatabaseUri = outputEndpoint
            };
            outputManager.Init();
            
        }

    }
}