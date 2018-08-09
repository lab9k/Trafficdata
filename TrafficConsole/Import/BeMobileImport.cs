using BlobToCosmos;
using Database_Access_Object;
using DocumentModels.BeMobile;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficConsole.Import
{
    public class BeMobileImport
    {
        public int TotalToProcess { get; set; }
        public int Processed { get; set; }

        public async Task Import(TaskModel model)
        {
            if(model.BlobInput == null)
            {
                Console.WriteLine("No Storage account connection info found, skipping task");
                return;
            }
            Console.WriteLine($"Starting blob import task for {model.BlobInput.AccountName},{model.BlobInput.Container},{model.BlobInput.Key} path {model.BlobInput.Path}");

            BlobManager manager = new BlobManager(model.BlobInput.AccountName, model.BlobInput.Key);

            List<Task> runningUploadTasks = new List<Task>();
            foreach(var path in model.BlobInput.Path)
            {
                Console.WriteLine($"Fetching al blobs at path {path}");
                List<IListBlobItem> items = manager.ListAllBlobsAsync(model.BlobInput.Container, path).Result;
                Console.WriteLine($"Found {items.Count} items at path {path}, parsing...");

                List<TraveltimeSegment> segments = new List<TraveltimeSegment>();
                foreach (IListBlobItem item in items)
                {
                    segments.AddRange(CSVReader.GetSegmentsFromCSVString(manager.DownloadBlob(item.Container.Name, BlobManager.getBlobIdFromURI(item.Uri)).Result));
                }

                CrudManager<TraveltimeSegment> crudManager = new CrudManager<TraveltimeSegment>
                {
                    DatabaseKey = model.Output.Key,
                    DatabaseUri = model.Output.URL,
                    DatabaseName = model.Output.Database,
                    CollectionName = model.Output.Collection,
                };
                crudManager.Init();
                TotalToProcess += segments.Count;
                runningUploadTasks.Add(UploadToCosmos(crudManager, segments));
            }
            await Task.WhenAll();
            
        }

        private const int maxConcurrency = 100;
        private SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(maxConcurrency);

        public async Task UploadToCosmos(CrudManager<TraveltimeSegment> crudManager, List<TraveltimeSegment> segments)
        {
            {
                List<Task> tasks = new List<Task>();
                foreach (var segment in segments)
                {
                    concurrencySemaphore.Wait();

                    var t = Task.Factory.StartNew(() =>
                    {

                        try
                        {
                            crudManager.Create(segment).Wait();
                        }
                        finally
                        {
                            concurrencySemaphore.Release();
                            UpdateProgress();
                        }
                    });

                    tasks.Add(t);
                }

                await Task.WhenAll(tasks.ToArray());
            }
        }

        public void UpdateProgress()
        {
            Processed += 1;
            if(Processed %500 == 0)
            {
                Console.WriteLine($"BeMobile import progress: {Processed}/{TotalToProcess} ({String.Format("{0:0}", (1.0 * Processed / TotalToProcess)*100)}%)");
            }
            if (Processed == TotalToProcess)
            {
                Console.WriteLine($"BeMobile import task finished");
            }
        }

    }
}
