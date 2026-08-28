using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TihieZori.Code
{
    public class JsonCusrtomFiltr
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("params")]
        public string[] Params { get; set; }
    }
}