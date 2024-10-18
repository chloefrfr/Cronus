using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class GiftsAttributes
    {
        [JsonProperty("lootList")]
        public List<Lootlist> LootList { get; set; }
    }
}
