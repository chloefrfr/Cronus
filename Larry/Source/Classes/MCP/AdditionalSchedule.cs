using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP
{
    public class AdditionalSchedule
    {
        [JsonProperty("rewardsClaimed")]
        public int RewardsClaimed { get; set; }
        [JsonProperty("claimedToday")]
        public bool ClaimedToday { get; set; }
    }
}
