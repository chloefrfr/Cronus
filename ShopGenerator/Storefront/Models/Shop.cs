using System.Text.Json.Serialization;

namespace ShopGenerator.Storefront.Models
{
    public class Shop
    {
        [JsonPropertyName("expiration")]
        public string Expiration => DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");
        [JsonPropertyName("refreshIntervalHrs")]
        public int RefreshIntervalHrs { get; set; } = 24;
        [JsonPropertyName("dailyPurchaseHrs")]
        public int DailyPurchaseHrs { get; set; } = 24;
        [JsonPropertyName("storefronts")]
        public List<Storefronts> Storefronts { get; set; } = new();
    }
}
