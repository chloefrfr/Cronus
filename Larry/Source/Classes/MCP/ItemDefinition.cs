using Larry.Source.Classes.Profile;
using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class ItemDefinition
    {
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("attributes")]
        // ACTUALLY KYS ICBA ITS SO LATE
        public dynamic Attributes { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

