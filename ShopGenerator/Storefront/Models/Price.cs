using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class Price
    {
        [JsonPropertyName("currencyType")]
        public string CurrencyType { get; set; }

        [JsonPropertyName("currencySubType")]
        public string CurrencySubType { get; set; }

        [JsonPropertyName("regularPrice")]
        public decimal RegularPrice { get; set; }

        [JsonPropertyName("dynamicRegularPrice")]
        public decimal? DynamicRegularPrice { get; set; }

        [JsonPropertyName("finalPrice")]
        public decimal FinalPrice { get; set; }

        [JsonPropertyName("saleType")]
        public string? SaleType { get; set; }

        [JsonPropertyName("saleExpiration")]
        public string SaleExpiration { get; set; }

        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }
    }
}
