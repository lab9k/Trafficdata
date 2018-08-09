using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficConsole
{
    public class Cosmos
    {
        public string URL { get; set; }
        public string Key { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
        public string StaticCollection { get; set; }
    }

    public class Blob
    {
        public string AccountName { get; set; }
        public string Key { get; set; }
        public string Container { get; set; }
        public string[] Path { get; set; }
    }
    public class TaskModel
    {
        public string GoogleApiKey { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public Blob BlobInput { get; set; }
        public Cosmos Input { get; set; }
        public Cosmos Output { get; set; }
        public List<string> Segments { get; set; }
    }
}

