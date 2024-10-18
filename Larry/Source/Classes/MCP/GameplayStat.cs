using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class GameplayStat
    {
        [JsonProperty("statValue")]
        public int StatValue { get; set; }
        [JsonProperty("statName")]
        public string StatName { get; set; }
    }
}
