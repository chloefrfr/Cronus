using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ShopGenerator.Storefront.Models
{
    public class Entry
    {
        [JsonPropertyName("offerId")]
        public string OfferType { get; set; }
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
        [JsonPropertyName("requirements")]
        public List<Requirement> Requirements { get; set; }
        [JsonPropertyName("prices")]
        public List<Price> Prices { get; set; }
        [JsonPropertyName("itemGrants")]
        public List<ItemGrant> ItemGrants { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
