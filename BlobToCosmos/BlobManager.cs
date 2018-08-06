using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlobToCosmos
{
    public class BlobManager
    {
        private CloudStorageAccount _cloudStorageAccount;

        public BlobManager(string myAccountName, string myAccountKey)
        {
            StorageCredentials storageCredentials = new StorageCredentials(myAccountName, myAccountKey);
            _cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);

        }

        public async Task DownloadBlob(string containerName, string filePath,string outputFolder)
        {
            CloudBlobClient _blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer _cloudBlobContainer = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob _blockBlob = _cloudBlobContainer.GetBlockBlobReference(filePath);
            //download    
            await _blockBlob.DownloadToFileAsync(outputFolder,FileMode.Create);
        }

        public async Task<string> DownloadBlob(string containerName, string blobId)
        {
            CloudBlobClient _blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer _cloudBlobContainer = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob _blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobId);
            //download    
            return await _blockBlob.DownloadTextAsync();
        }

        public async Task<List<IListBlobItem>> ListAllBlobsAsync(string containerName, string prefix = null)
        {
            // Create the blob client.
            CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Loop over items within the container and output the length and URI.
            BlobContinuationToken blobContinuationToken = null;
            List<IListBlobItem> blobItems = new List<IListBlobItem>();

            do
            {
                var results = await container.ListBlobsSegmentedAsync(prefix, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    blobItems.Add(item);
                    //Console.WriteLine(item.Uri);
                }

            } while (blobContinuationToken != null); // Loop while the continuation token is not null. 
            return blobItems;
        }

        public static string getBlobIdFromURI(Uri uri)
        {
            string s = "";
            int segments = uri.Segments.Length;
            while(segments-- > 2)
            {
                s = uri.Segments[segments] + s;
            }
            return s;
        }
    }
}
