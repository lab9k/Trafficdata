using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Database_Access_Object
{
    public class CrudManager<T> : ICRUDManager<T>
    {
        public string DatabaseUri { get; set; }
        public string DatabaseKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public DocumentClient Client;

        public void Init()
        {
            Client = new DocumentClient(new Uri(DatabaseUri), DatabaseKey);
            CreateDatabaseIfNotExists().Wait();
            CreateCollectionIfNotExists().Wait();
        }

        public void Init(IncludedPath indexPolicy)
        {
            Client = new DocumentClient(new Uri(DatabaseUri), DatabaseKey);
            CreateDatabaseIfNotExists().Wait();
            CreateCollectionIfNotExists(indexPolicy).Wait();
        }

        private async Task CreateDatabaseIfNotExists()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Client.CreateDatabaseAsync(new Database {Id = DatabaseName});
                }
                else
                {
                    throw ex;
                }
            }


        }

        private async Task CreateCollectionIfNotExists(IncludedPath indexPolicy = null)
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName,CollectionName));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    DocumentCollection coll = new DocumentCollection { Id = CollectionName };

                    if(indexPolicy != null)
                    {
                        coll.IndexingPolicy.IncludedPaths.Add(indexPolicy);
                    }
                    await Client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(DatabaseName),
                        coll, new RequestOptions {OfferThroughput = 400});

                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task Create(T obj, int tryCount = 0, int retryCount = 0)
        {
            try
            {
                if(tryCount <= retryCount)
                    await Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), obj);
            }catch(Exception ex)
            {
                Console.WriteLine($"Error inserting document, {(tryCount <= retryCount ? "retrying action" : "skipping action")}");
                await Create(obj, ++tryCount, retryCount);
            }
        }


        public async Task CreateAll(List<T> list)
        {
            foreach(T obj in list)
            {
                await Create(obj);
            }
        }
        public async Task<T> Get(string id)
        {
                var docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
                Document doc = await Client.ReadDocumentAsync(docUri);
                
                T res =  (T) (dynamic) doc;
                return res;           
        }

        public async Task<T> Update(string id, T obj)
        {
            var docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
            Document doc = await Client.ReplaceDocumentAsync(docUri,obj);
            return (T)(dynamic)doc;
        }

        public async Task Delete(string id)
        {
            var docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
            await Client.DeleteDocumentAsync(docUri);
        }
    }
}