using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP
{
    public class CollectionBook
    {
        [JsonProperty("maxBookXpLevelAchieved")]
        public int MaxBookXpLevelAchieved { get; set; }
        [JsonProperty("pages")]
        public List<string> Pages { get; set; }
    }
}
