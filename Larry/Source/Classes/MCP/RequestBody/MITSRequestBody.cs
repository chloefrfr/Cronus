using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP.RequestBody
{
    // worst name ever, but we ball
    public class MITSRequestBody
    {
        [JsonPropertyName("itemIds")]
        public string[] ItemIds { get; set; }
    }

}
