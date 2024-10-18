using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP
{
    public class MissionAlertRedemptionRecord
    {
        [JsonProperty("lastClaimTimesMap")]
        public object LastClaimTimesMap { get; set; }
        [JsonProperty("lastClaimedGuidPerTheater")]
        public object LastClaimedGuidPerTheater { get; set; }
        [JsonProperty("claimData")]
        public List<MissionAlertClaimData> ClaimData { get; set; }
    }
}
