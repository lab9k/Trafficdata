using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Database_Access_Object;
using DocumentModels.BeMobile;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class ConfigManager
    {
        public static IConfiguration Configuration { get; set; }

        private static void InitIfNull()
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
            }
        }

        public static object GetProperty(string id)
        {
            InitIfNull();
            return Configuration[id];
        }

        public static QueryManager<T> GetDefaultQueryManager<T>(string collectionId)
        {
            QueryManager<T> _new = new QueryManager<T>
            {
                CollectionName = collectionId,
                DatabaseKey = ConfigManager.GetProperty("database:key") as string,
                DatabaseName = ConfigManager.GetProperty("database:name") as string,
                DatabaseUri = ConfigManager.GetProperty("database:endpoint") as string
            };

            _new.Init();
            return _new;
        }


    }
}
