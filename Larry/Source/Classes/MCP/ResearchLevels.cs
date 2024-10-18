using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class ResearchLevels
    {
        [JsonProperty("offense")]
        public int Offense { get; set; }
        [JsonProperty("technology")]
        public int Technology { get; set; }
        [JsonProperty("fortitude")]
        public int Fortitude { get; set; }
        [JsonProperty("resistance")]
        public int Resistance { get; set; }
    }
}
