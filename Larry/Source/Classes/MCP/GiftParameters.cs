using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class GiftParameters
    {
        [JsonProperty("userMessage")]
        public string UserMessage { get; set; }
    }
}
