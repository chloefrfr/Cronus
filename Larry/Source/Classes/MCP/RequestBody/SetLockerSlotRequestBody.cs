using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP.RequestBody
{
    public class SetLockerSlotRequestBody
    {
        [JsonPropertyName("itemToSlot")]
        public string ItemToSlot { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("lockerItem")]
        public string LockerItem { get; set; }
        [JsonPropertyName("slotIndex")]
        public int SlotIndex { get; set; }
        [JsonPropertyName("variantUpdates")]
        public List<Variants> VariantUpdates { get; set; }
    }
}
