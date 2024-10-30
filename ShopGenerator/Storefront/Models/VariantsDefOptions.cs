using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class VariantsDefOptions
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}
