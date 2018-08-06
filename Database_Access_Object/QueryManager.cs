using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace Database_Access_Object
{
    public class QueryManager<T> : CrudManager<T>,IQueryManager<T>
    {
       

        public List<T> GetAllResults(string Query, FeedOptions options = null)
        {
            //Console.WriteLine("Executing SQL-query, this may take a while...");
            if (this.Client == null)
            {
                throw new InvalidOperationException(
                    "Client object is not initialized, did you execute the 'init()' function?");
            }
            IQueryable<T> queryInSql = this.Client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                Query,
                options);
            
            List < T > result = queryInSql.ToList<T>();
           // Console.WriteLine($"{result.Count} results");
            return result;
        }

        public async Task<List<T>> GetAllResultsAsync(string Query, FeedOptions options = null)
        {
            return await Task.Run(() => GetAllResults(Query, options));
        }



        public IOrderedQueryable<T> GetLinqQuery()
        {
            return this.Client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName));
        }

        public List<T> GetAll(int? limit = null)
        {
            List <T> result = GetAllResults(String.Format("select {0} * from c", limit == null ? " " : $"TOP {limit}"));
            return result;
        }

    }
}
