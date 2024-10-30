using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class CosmeticSeries
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
        [JsonPropertyName("colors")]
        public List<string> Colors { get; set; }
        [JsonPropertyName("backendValue")]
        public string BackendValue { get; set; }
    }
}
