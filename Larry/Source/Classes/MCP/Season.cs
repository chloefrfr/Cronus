using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class Season
    {
        [JsonProperty("numWins")]
        public int NumWins { get; set; }
        [JsonProperty("numHighBracket")]
        public int NumHighBracket { get; set; }
        [JsonProperty("numLowBracket")]
        public int NumLowBracket { get; set; }
    }
}
