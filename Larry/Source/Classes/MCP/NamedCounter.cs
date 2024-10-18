using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class NamedCounter
    {
        [JsonProperty("current_count")]
        public int CurrentCount { get; set; }
        [JsonProperty("last_incremented_time")]
        public string LastIncrementedTime { get; set; }
    }
}
