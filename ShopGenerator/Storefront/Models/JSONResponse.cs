using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class JSONResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("type")]
        public CosmeticType Type { get; set; }
        [JsonPropertyName("rarity")]
        public CosmeticRarity Rarity { get; set; }
        [JsonPropertyName("series")]
        public CosmeticSeries Series { get; set; }
        [JsonPropertyName("set")]
        public CosmeticSet Set { get; set; }
        [JsonPropertyName("backpack")]
        public JSONResponse Backpack { get; set; }
        [JsonPropertyName("introduction")]
        public CosmeticIntroduction Introduction { get; set; }
        [JsonPropertyName("images")]
        public CosmeticImages Images { get; set; }
        [JsonPropertyName("itemPreviewHero")]
        public string ItemPreviewHeroPath { get; set; }
        [JsonPropertyName("displayAssetPath")]
        public string DisplayAssetPath { get; set; }
        [JsonPropertyName("newDisplayAssetPath")]
        public string NewDisplayAssetPath { get; set; }
        [JsonPropertyName("defintionPath")]
        public string DefinitionPath { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("added")]
        public DateTime Added { get; set; }
        [JsonPropertyName("shopHistory")]
        public List<string> ShopHistory { get; set; }
        [JsonPropertyName("variants")]
        public List<VariantsDef> Variants { get; set; }
    }
}
