﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                    if (t.Type == "Transform" && t.Source == "Be-Mobile")
                    {
                        BeMobileTransform.Tranform(t).Wait();
                    }
                }

            }
            else
            {
                Console.WriteLine($"File {path} does not exist");
            }
            //BeMobileTraveltimesCreator.CreateMergedDocuments()
        }
    }
}
