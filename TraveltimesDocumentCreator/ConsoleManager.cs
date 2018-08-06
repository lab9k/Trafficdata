using Database_Access_Object;
using DocumentModels.BeMobile;
using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator
{
    public class ConsoleManager
    {

        public static void InitProgram()
        {
            Console.WriteLine("Select one of the options: \n 1: Aggregate static Be-Mobile data\n 2: Exit");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.KeyChar == '2')
            {
                Environment.Exit(0);

            }
            else if (key.KeyChar == '1')
            {
                Option1();
            }
        }

        private static void Option1()
        {
            Console.WriteLine("Specify input collection? y/n(default)");
            ConsoleKeyInfo key2 = Console.ReadKey();
            string Idburi = null, Idbkey = null, IdbName = null, Icollection = null;
            string Odburi = null, Odbkey = null, OdbName = null, Ocollection = null;
            if (key2.KeyChar == 'y')
            {
                Console.WriteLine("");
                Console.WriteLine("Database URI: ");
                Idburi = Console.ReadLine();
                Console.WriteLine("Database Key: ");
                Idbkey = Console.ReadLine();
                Console.WriteLine("Database Name: ");
                IdbName = Console.ReadLine();
                Console.WriteLine("Collection: ");
                Icollection = Console.ReadLine();
            }
            Console.WriteLine("Specify output collection? y/n(default)");
            key2 = Console.ReadKey();
            if (key2.KeyChar == 'y')
            {
                Console.WriteLine("");
                Console.WriteLine("Database URI: ");
                Odburi = Console.ReadLine();
                Console.WriteLine("Database Key: ");
                Odbkey = Console.ReadLine();
                Console.WriteLine("Database Name: ");
                OdbName = Console.ReadLine();
                Console.WriteLine("Collection: ");
                Ocollection = Console.ReadLine();
            }

            BemStaticAggregator aggregator;
            if (Idburi == null)
            {
                aggregator = new BemStaticAggregator();
            }
            else
            {
                aggregator = new BemStaticAggregator(new QueryManager<TraveltimeSegmentStatic>
                {
                    CollectionName = Icollection,
                    DatabaseName = IdbName,
                    DatabaseUri = Idburi,
                    DatabaseKey = Idbkey
                });
            }
            List<TraveltimeStatic> aggregatedSegments = aggregator.GeTraveltimeStatic(aggregator.GetAllSegmentIds());
            if (Odburi == null)
            {
                Console.WriteLine("Not Supported Yet");
            }
            else
            {
                QueryManager<TraveltimeStatic> outputManager = new QueryManager<TraveltimeStatic>
                {
                    CollectionName = Ocollection,
                    DatabaseName = OdbName,
                    DatabaseUri = Odburi,
                    DatabaseKey = Odbkey
                };
                outputManager.Init();
                foreach (TraveltimeStatic segment in aggregatedSegments)
                {
                    outputManager.Create(segment).Wait();
                }
            }
        }
    }
}
