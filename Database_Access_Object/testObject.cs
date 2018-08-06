using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Database_Access_Object
{
    public class TestObject : IDocumentPOCO
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("age")]
        public int Age { get; set; }

    }
}
