using Database_Access_Object;
using DocumentModels.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebDownload
{
    public class RecordFetcher
    {
        private static IConfiguration _configuration;
        private static IConfiguration Configuration
        {
            get
            {
                if(_configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json");
                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }
        
        public static List<ParserSegment> GetResults(string from, string to, string segment)
        {
            return GetResults(Configuration["Connection:Url"], Configuration["Connection:Key"], Configuration["Connection:Database"], Configuration["Connection:Collection"], from, to, segment);
        }

        public static List<ParserSegment> GetResults(string url, string key, string database, string collection, string from, string to, string segment)
        {
            QueryManager<ParserSegment> queryManager = new QueryManager<ParserSegment>
            {
                DatabaseUri = url,
                DatabaseKey = key,
                DatabaseName = database,
                CollectionName = collection
            };
            queryManager.Init();
            return queryManager.GetAllResults($"select * from c where c.Timestamp.FullDate > '{from}' and c.Timestamp.FullDate < '{to}' and c.SegmentName ='{segment}'");
        }
    }
}
