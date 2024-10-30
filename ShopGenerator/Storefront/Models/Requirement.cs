using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class Requirement
    {
        [JsonPropertyName("requirementType")]
        public string RequirementType { get; set; }
        [JsonPropertyName("requiredId")]
        public string RequiredId { get; set; }
        [JsonPropertyName("minQuantity")]
        public int MinQuantity { get; set; }
    }
}
