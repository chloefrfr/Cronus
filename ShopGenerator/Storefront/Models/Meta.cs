using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Models
{
    public class Meta
    {
        public string NewDisplayAssetPath { get; set; }
        public string LayoutId { get; set; }
        public string TileSize { get; set; }
        public string AnalyticOfferGroupId { get; set; }
        public string SectionId { get; set; }
        public string TemplateId { get; set; }
        public string InDate { get; set; }
        public string OutDate { get; set; }
        public string DisplayAssetPath { get; set; }
        public string BUseSharedDisplay { get; set; }
        public string SharedDisplayPriority { get; set; }
        public string IconSize { get; set; }
        public string OriginalOffer { get; set; }
        public int ExtraBonus { get; set; } 
        public int Priority { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string CurrencyAnalyticsName { get; set; }
    }
}
