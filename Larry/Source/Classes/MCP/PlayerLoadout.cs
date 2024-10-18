using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class PlayerLoadout
    {
        [JsonProperty("primaryQuickBarRecord")]
        public QuickBarRecord PrimaryQuickBarRecord { get; set; }
        [JsonProperty("pinnedSchematicInstances")]
        public List<object> PinnedSchematicInstances { get; set; }
        [JsonProperty("secondaryQuickBarRecord")]
        public QuickBarRecord SecondaryQuickBarRecord { get; set; }
        [JsonProperty("zonesCompleted")]
        public int ZonesCompleted { get; set; }
        [JsonProperty("bPlayerIsNew")]
        public bool BPlayerIsNew { get; set; }
    }
}
