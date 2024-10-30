using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class ItemGrant
    {
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
