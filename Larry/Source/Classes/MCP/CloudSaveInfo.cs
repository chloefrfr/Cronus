using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class CloudSaveInfo
    {
        [JsonProperty("saveCount")]
        public int SaveCount { get; set; }
        [JsonProperty("savedRecords")]
        public List<SavedRecords> SavedRecords { get; set; }
    }
}
