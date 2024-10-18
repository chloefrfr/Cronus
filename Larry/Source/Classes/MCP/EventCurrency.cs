using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class EventCurrency
    {
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
        [JsonProperty("cf")]
        public int Cf { get; set; }
    }
}
