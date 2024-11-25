using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class MetaInfo
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
