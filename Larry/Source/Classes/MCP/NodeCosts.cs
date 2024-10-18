using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class NodeCosts
    {
        [JsonProperty("homebase_node_default_page")]
        public Dictionary<string, int> HomebaseNodeDefaultPage { get; set; }
        [JsonProperty("research_node_default_page")]
        public Dictionary<string, int> ResearchNodeDefaultPage { get; set; }
    }
}
