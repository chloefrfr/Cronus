using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class BanStatus
    {
        [JsonProperty("bRequiresUserAck")]
        public bool BRequiresUserAck { get; set; }
        [JsonProperty("bBanHasStarted")]
        public bool BBanHasStarted { get; set; }
        [JsonProperty("banReasons")]
        public List<object> BanReasons { get; set; }
        [JsonProperty("banStartTimeUtc")]
        public string BanStartTimeUtc { get; set; }
        [JsonProperty("banDurationDays")]
        public int? BanDurationDays { get; set; }
        [JsonProperty("additionalInfo")]
        public string AdditionalInfo { get; set; }
        [JsonProperty("exploitProgramName")]
        public string ExploitProgramName { get; set; }
        [JsonProperty("competitiveBanReason")]
        public string CompetitiveBanReason { get; set; }
    }
}
