using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class CosmeticType
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("displayValue")]
        public string DisplayValue { get; set; }
        [JsonPropertyName("backendValue")]
        public string BackendValue { get; set; }
    }
}
