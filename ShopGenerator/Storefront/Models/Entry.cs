using System.Diagnostics;
using System.Runtime;
using System.Text.Json.Serialization;

namespace ShopGenerator.Storefront.Models
{
    public class Entry
    {
        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }

        [JsonPropertyName("offerType")]
        public string OfferType { get; set; }

        [JsonPropertyName("devName")]
        public string DevName { get; set; }

        [JsonPropertyName("itemGrants")]
        public List<ItemGrant> ItemGrants { get; set; }

        [JsonPropertyName("requirements")]
        public List<Requirement> Requirements { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }

        [JsonPropertyName("metaInfo")]
        public List<MetaInfo> MetaInfo { get; set; }

        [JsonPropertyName("giftInfo")]
        public GiftInfo GiftInfo { get; set; }

        [JsonPropertyName("prices")]
        public List<Price> Prices { get; set; }

        [JsonPropertyName("bannerOverride")]
        public string BannerOverride { get; set; }

        [JsonPropertyName("displayAssetPath")]
        public string DisplayAssetPath { get; set; }

        [JsonPropertyName("newDisplayAssetPath")]
        public string NewDisplayAssetPath { get; set; }

        [JsonPropertyName("refundable")]
        public bool Refundable { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("appStoreId")]
        public List<string> AppStoreId { get; set; }

        [JsonPropertyName("fulfillmentIds")]
        public List<object> FulfillmentIds { get; set; }

        [JsonPropertyName("dailyLimit")]
        public int DailyLimit { get; set; }

        [JsonPropertyName("weeklyLimit")]
        public int WeeklyLimit { get; set; }

        [JsonPropertyName("monthlyLimit")]
        public int MonthlyLimit { get; set; }

        [JsonPropertyName("sortPriority")]
        public int SortPriority { get; set; }

        [JsonPropertyName("catalogGroupPriority")]
        public int CatalogGroupPriority { get; set; }

        [JsonPropertyName("filterWeight")]
        public int FilterWeight { get; set; }
    }
}
