using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP.RequestBody
{
    public class MtxPlatformBody
    {
        [JsonPropertyName("newPlatform")]
        public string NewPlatform { get; set; }
    }
}
