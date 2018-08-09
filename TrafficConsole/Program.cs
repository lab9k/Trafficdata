using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TrafficConsole.Import;
using TrafficConsole.Transform;
using TraveltimesDocumentCreator;

namespace TrafficConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "config.json";
            if (args.Length >= 1) file = args[0];
            string path = Path.Combine(
                           Directory.GetCurrentDirectory(), file);
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                List<TaskModel> tasks = JsonConvert.DeserializeObject<List<TaskModel>>(json);
                foreach(TaskModel t in tasks)
                {
                    if(t.Source == "Be-Mobile")
                    {
                        if (t.Type == "Transform")
                        {
                            BeMobileTransform.Tranform(t).Wait();
                        }
                        else if (t.Type == "Import")
                        {
                            BeMobileImport import = new BeMobileImport();
                            import.Import(t).Wait();
                        }
                    }                   
                }

            }
            else
            {
                Console.WriteLine($"File {path} does not exist");
            }
            Console.ReadKey();
            //BeMobileTraveltimesCreator.CreateMergedDocuments()
        }
    }
}
