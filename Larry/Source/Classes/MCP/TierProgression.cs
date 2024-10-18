using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class TierProgression
    {
        [JsonProperty("progressionInfo")]
        public List<ProgressionInfo> ProgressionInfo { get; set; }
    }
}
