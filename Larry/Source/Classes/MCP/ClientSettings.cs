using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class ClientSettings
    {
        [JsonProperty("pinnedQuestInstances")]
        public List<object> PinnedQuestInstances { get; set; }
    }
}
