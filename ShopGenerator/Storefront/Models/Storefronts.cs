using System.Text.Json.Serialization;

namespace ShopGenerator.Storefront.Models
{
    public class Storefronts
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("catalogEntries")]
        public List<Entry> CatalogEntries { get; set; } = new();
    }
}
