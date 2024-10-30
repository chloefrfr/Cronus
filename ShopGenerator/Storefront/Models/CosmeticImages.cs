using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class CosmeticImages
    {
        [JsonPropertyName("smallIcon")]
        public string SmallIcon { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("featured")]
        public string Featured { get; set; }
    }
}
