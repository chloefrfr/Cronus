using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class VariantsDef
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("options")]
        public List<VariantsDefOptions> Options { get; set; }
    }
}
