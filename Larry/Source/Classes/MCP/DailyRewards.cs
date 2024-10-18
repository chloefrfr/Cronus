using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class DailyRewards
    {
        [JsonProperty("nextDefaultReward")]
        public int NextDefaultReward { get; set; }
        [JsonProperty("totalDaysLoggedIn")]
        public int TotalDaysLoggedIn { get; set; }
        [JsonProperty("lastClaimDate")]
        public string LastClaimDate { get; set; }
        [JsonProperty("additionalSchedules")]
        public Dictionary<string, AdditionalSchedule> AdditionalSchedules { get; set; }
    }
}
