using System.Text.Json.Serialization;
using Newtonsoft.Json;
 

namespace Larry.Source.Classes.MCP
{
    public class VoteData
    {
        [JsonProperty("electionId")]
        public string ElectionId { get; set; }
        [JsonProperty("voteHistory")]
        public Dictionary<string, object> VoteHistory { get; set; }
        [JsonProperty("votesRemaining")]
        public int VotesRemaining { get; set; }
        [JsonProperty("lastVoteGranted")]
        public string LastVoteGranted { get; set; }
    }
}
