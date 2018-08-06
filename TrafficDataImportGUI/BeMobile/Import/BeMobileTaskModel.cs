using BlobToCosmos;
using Database_Access_Object;
using DocumentModels.BeMobile;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficDataImportGUI.BeMobile.Import
{
    public class BeMobileTaskModel
    {
        public string BlobName { get; set; }
        public string BlobKey { get; set; }
        public string BlobContainer { get; set; }
        public string Month { get; set; }
        public string Traject { get; set; }
        public string CosmosKey { get; set; }
        public string CosmosUrl { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }

        public void Execute(ILogger logger, IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> queue)
        {
            if(BlobName == null && BlobKey == null)
            {
                logger.LogWarning("Execute called upon empty task");
                return;
            }
            logger.LogInformation("Starting BE-Mobile import task");
            BlobManager manager = new BlobManager(BlobName, BlobKey);
            List<IListBlobItem> items = manager.ListAllBlobsAsync(BlobContainer, $"BE-Mobile/{Month}/Reistijden/{Traject}/").Result;
            CrudManager<TraveltimeSegment> crudManager = new CrudManager<TraveltimeSegment>
            {
                DatabaseKey = CosmosKey,
                DatabaseUri = CosmosUrl,
                DatabaseName = DatabaseName,
                CollectionName = CollectionName,
            };
            crudManager.Init();

            List<TraveltimeSegment> segments = new List<TraveltimeSegment>();
            foreach (IListBlobItem item in items)
            {
                segments.AddRange(CSVReader.GetSegmentsFromCSVString(manager.DownloadBlob(item.Container.Name, BlobManager.getBlobIdFromURI(item.Uri)).Result));                                
            }

            foreach(TraveltimeSegment segment in segments)
            {
                queue.DocumentQueue.Add(new BEMobileSegmentTaskModel
                {
                    CreationMethod = crudManager.Create,
                    SegmentToCreate = segment
                });
            }
            queue.DocumentsFinished += 1;
        }
    }
}
