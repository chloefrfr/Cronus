using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class CampaignStat
    {
        [JsonProperty("season")]
        public int Season { get; set; }
    }
}
