using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class MissionAlertClaimData
    {
        [JsonProperty("missionAlertId")]
        public string MissionAlertId { get; set; }
        [JsonProperty("evictClaimDataAfterUtc")]
        public string EvictClaimDataAfterUtc { get; set; }
        [JsonProperty("redemptionDateUtc")]
        public string RedemptionDateUtc { get; set; }
    }
}
