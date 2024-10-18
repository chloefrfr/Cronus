using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class ProgressionInfo
    {
        [JsonProperty("progressionLayoutGuid")]
        public string ProgressionLayoutGuid { get; set; }
        [JsonProperty("highestDefeatedTier")]
        public int HighestDefeatedTier { get; set; }
    }
}
