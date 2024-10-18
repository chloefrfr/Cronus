using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class Variants
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("active")]
        public string Active { get; set; }
        [JsonProperty("owned")]
        public List<string> Owned { get; set; }
    }
}
