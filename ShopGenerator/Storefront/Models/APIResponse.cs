using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class APIResponse
    {
        [JsonPropertyName("data")]
        public List<JSONResponse> Data { get; set; }
    }
}
