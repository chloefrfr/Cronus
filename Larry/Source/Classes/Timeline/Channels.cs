using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Larry.Source.Classes.Timeline
{
    public class Channels
    {
        [JsonPropertyName("client-matchmaking")]
        public ClientMatchmaking clientMatchmaking { get; set; }
        [JsonPropertyName("community-votes")]
        public CommunityVotes communityVotes { get; set; }
        [JsonPropertyName("client-events")]
        public ClientEvents clientEvents { get; set; }
    }
}
