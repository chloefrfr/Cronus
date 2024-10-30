using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class CosmeticIntroduction
    {
        [JsonPropertyName("chapter")]
        public string Chapter { get; set; }
        [JsonPropertyName("season")]
        public string Season { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("backendValue")]
        public int BackendValue { get; set; }
    }
}
