using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class Gifts
    {
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
        [JsonProperty("attributes")]
        public GiftsAttributes Attributes { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
