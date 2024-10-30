using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class Meta
    {
        // No need for JsonPropertyName here, these are the default property names.
        public string SectionId { get; set; }
        public string TileSize { get; set; }
    }
}
