using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class OutpostCoreInfo
    {
        [JsonProperty("placedBuildings")]
        public List<PlacedBuildings> PlacedBuildings { get; set; }
        [JsonProperty("accountsWithEditPermission")]
        public List<string> AccountsWithEditPermission { get; set; }
        [JsonProperty("highestEnduranceWaveReached")]
        public string HighestEnduranceWaveReached { get; set; }
    }
}
