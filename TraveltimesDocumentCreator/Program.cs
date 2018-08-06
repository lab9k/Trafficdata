using System;
using System.Collections.Generic;
using System.IO;
using Database_Access_Object;
using DocumentModels.BeMobile;
using DocumentModels.Generic;
using DocumentModels.Waze;
using Microsoft.Extensions.Configuration;
using RemoteApi;
using RemoteApi.Google;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Waze;

namespace TraveltimesDocumentCreator { 
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Console.ReadKey();
        }
    }
}
