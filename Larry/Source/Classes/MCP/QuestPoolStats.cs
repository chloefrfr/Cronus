using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class QuestPoolStats
    {
        [JsonProperty("dailyLoginInterval")]
        public string DailyLoginInterval { get; set; }
        [JsonProperty("poolLockouts")]
        public PoolLockouts PoolLockouts { get; set; }
        [JsonProperty("poolStats")]
        public List<PoolStat> PoolStats { get; set; }
    }
}
