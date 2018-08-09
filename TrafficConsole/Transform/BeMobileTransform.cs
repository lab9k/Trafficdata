using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraveltimesDocumentCreator;

namespace TrafficConsole.Transform
{
    public class BeMobileTransform
    {
        public static void TransformStaticData(TaskModel model)
        {
            if(model.GoogleApiKey == null)
            {
                //TODO: Uncomment when Locations API is implemented correctly
                //Console.WriteLine($"No google APIKey found for task {model.Source}:{model.Type}, skipping task");
                //return;
            }

            Console.WriteLine($"Task {model.Source}:{model.Type} started");

            BemStaticAggregator.AggregateStaticData(model.GoogleApiKey, model.Input.URL, model.Input.Key, model.Input.Database, model.Input.Collection,
                model.Output.Collection, model.Output.Database, model.Output.URL, model.Output.Key);
        }

        public async static Task Transform(TaskModel model)
        {
            List<Task> runningTasks = new List<Task>();
            foreach (string segment in model.Segments)
            {
                Console.WriteLine($"Task {model.Source}:{model.Type} for segment {segment} started");

                Task t = BeMobileTraveltimesCreator.CreateMergedDocuments(new string[] { segment }, model.Input.URL, model.Input.Key,
                    model.Input.Database, model.Input.StaticCollection, model.Input.Collection, model.Output.Collection, model.Output.Database, model.Output.URL, model.Output.Key);
                Task s = t.ContinueWith((x) => NotifyFinished(model, segment));

                runningTasks.Add(t);
                runningTasks.Add(s);
            }
            await Task.WhenAll(runningTasks);
        }

        public static void NotifyFinished(TaskModel m, string segment)
        {
            Console.WriteLine($"Task {m.Source}:{m.Type} for segment {segment} finished");
        }

    }
}
