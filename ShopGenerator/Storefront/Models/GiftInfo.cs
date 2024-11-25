using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class GiftInfo
    {
        [JsonPropertyName("bIsEnabled")]
        public bool BIsEnabled { get; set; }
        [JsonPropertyName("forcedGiftBoxTemplateId")]
        public string ForcedGiftBoxTemplateId { get; set; }
        [JsonPropertyName("purchaseRequirements")]
        public List<Requirement> PurchaseRequirements { get; set; }
        [JsonPropertyName("giftRecordIds")]
        public List<string> GiftRecordIds { get; set; }
    }
}
