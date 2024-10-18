using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class QuestManager
    {
        [JsonProperty("dailyLoginInterval")]
        public string DailyLoginInterval { get; set; }
        [JsonProperty("dailyQuestRerolls")]
        public int DailyQuestRerolls { get; set; }
        [JsonProperty("questPoolStats")]
        public QuestPoolStats QuestPoolStats { get; set; }
    }
}
