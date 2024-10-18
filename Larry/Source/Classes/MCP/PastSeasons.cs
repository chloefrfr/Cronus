using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class PastSeasons
    {
        [JsonProperty("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonProperty("numWins")]
        public int NumWins { get; set; }

        [JsonProperty("numHighBracket")]
        public int NumHighBracket { get; set; }

        [JsonProperty("numLowBracket")]
        public int NumLowBracket { get; set; }

        [JsonProperty("seasonXp")]
        public int SeasonXp { get; set; }

        [JsonProperty("seasonLevel")]
        public int SeasonLevel { get; set; }

        [JsonProperty("bookXp")]
        public int BookXp { get; set; }

        [JsonProperty("bookLevel")]
        public int BookLevel { get; set; }

        [JsonProperty("purchasedVIP")]
        public bool PurchasedVIP { get; set; }

        [JsonProperty("numRoyalRoyales")]
        public int NumRoyalRoyales { get; set; }

        [JsonProperty("survivorTier")]
        public int SurvivorTier { get; set; }

        [JsonProperty("survivorPrestige")]
        public int SurvivorPrestige { get; set; }
    }
}
