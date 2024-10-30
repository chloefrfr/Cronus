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
        [JsonPropertyName("regularPrice")]
        public int RegularPrice { get; set; }
        [JsonPropertyName("finalPrice")]
        public int FinalPrice { get; set; }
    }
}
