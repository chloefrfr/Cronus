using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class StoreSet
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("definition")]
        public List<JSONResponse> Definition { get; set; }
    }
}
