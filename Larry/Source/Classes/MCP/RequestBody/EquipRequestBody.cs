using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP.RequestBody
{
    public class EquipRequestBody
    {
        [JsonPropertyName("itemToSlot")]
        public string ItemToSlot { get; set; }
        [JsonPropertyName("indexWithinSlot")]
        public int IndexWithinSlot { get; set; }
        [JsonPropertyName("slotName")]
        public string SlotName { get; set; }
        [JsonPropertyName("variantUpdates")]
        public List<Variants> VariantUpdates { get; set; }
    }
}
