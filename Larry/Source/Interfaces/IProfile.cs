using Larry.Source.Classes.Profile;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Larry.Source.Classes.MCP;


namespace Larry.Source.Interfaces
{
    public interface IProfile
    {
        [JsonProperty("created")]
        public string Created { get; set; }
        [JsonProperty("updated")]
        public string Updated { get; set; }
        [JsonProperty("rvn")]
        public int Rvn { get; set; }
        [JsonProperty("wipeNumber")]
        public int WipeNumber { get; set; }
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
        [JsonProperty("profileId")]
        public string ProfileId { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("stats")]
        public StatsAttributes Stats { get; set; }
        [JsonProperty("items")]
        public Dictionary<Guid, ItemDefinition> Items { get; set; }
        [JsonProperty("commandRevision")]
        public int CommandRevision { get; set; }
    }
}
