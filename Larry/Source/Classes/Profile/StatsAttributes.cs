using System.Collections.Generic;
using System.Text.Json.Serialization;
using Larry.Source.Classes.MCP;
using Newtonsoft.Json;

namespace Larry.Source.Classes.Profile
{
    public class StatsAttributes
    {
        [JsonProperty("attributes")]
        public StatsData Attributes { get; set; }
    }
}
