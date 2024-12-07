using ShopGenerator.Storefront.Enums;
using ShopGenerator.Storefront.Extensions;
using ShopGenerator.Storefront.Generation.Prices;
using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ShopGenerator.Storefront.Generation
{
    public class ShopEntry
    {
        /// <summary>
        /// Creates a new shop entry from a given JSON response and section.
        /// </summary>
        /// <param name="item">The JSON response containing item details.</param>
        /// <param name="section">The section to which the entry belongs.</param>
        /// <returns>A new <see cref="Entry"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the item or required fields are null.</exception>
        public static Entry New(JSONResponse item, Sections section)
        {
            if (item == null || string.IsNullOrEmpty(item?.Type.BackendValue) || string.IsNullOrEmpty(item?.Id))
                throw new ArgumentNullException(nameof(item));

            var itemPrice = ItemPrices.GetPrice(item);
            if (itemPrice == null)
                throw new ArgumentNullException(nameof(itemPrice));

            var entry = CreateBaseEntry(item, section, itemPrice ?? 0);

            void AddMetaInfo(string key, string value)
            {
                if (!entry.MetaInfo.Any(meta => meta.Key == key))
                {
                    entry.MetaInfo.Add(new() { Key = key, Value = value });
                }
            }

            AddMetaInfo("DisplayAssetPath", entry.DisplayAssetPath);
            AddMetaInfo("NewDisplayAssetPath", entry.NewDisplayAssetPath);
            AddMetaInfo("TileSize", section == Sections.Featured ? "Normal" : "Small");
            AddMetaInfo("SectionId", section.GetDescription());

            if (item.Backpack != null)
            {
                ItemUtils.AddItemGrant(entry, item.Backpack.Type.BackendValue, item.Backpack.Id);
                ItemUtils.AddRequirement(entry, item.Backpack.Type.BackendValue, item.Backpack.Id);
            }

            entry.GiftInfo.PurchaseRequirements = entry.Requirements;

            return entry;
        }


        /// <summary>
        /// Creates a base entry with default properties.
        /// </summary>
        /// <param name="item">The JSON response containing item details.</param>
        /// <param name="section">The section to which the entry belongs.</param>
        /// <param name="price">The price of the entry in MtxCurrency.</param>
        /// <returns>A base <see cref="Entry"/> object.</returns>
        private static Entry CreateBaseEntry(JSONResponse item, Sections section, int price)
        {
            return new Entry
            {
                OfferId = $"v2:/{Guid.NewGuid()}",
                OfferType = "StaticPrice",
                DevName = $"[VIRTUAL] 1x {item.Type.BackendValue}:{item.Id} for {price} MtxCurrency",
                ItemGrants = new List<ItemGrant> { new() { TemplateId = $"{item.Type.BackendValue}:{item.Id}", Quantity = 1 } },
                Requirements = new List<Requirement> { Constants.DefaultRequirements(item.Type.BackendValue, item.Id) },
                Categories = item.Set != null ? new List<string> { item.Set.BackendValue } : new List<string>(),
                MetaInfo = Constants.DefaultMetaInfo,
                Prices = new List<Price>
                {
                    new()
                    {
                        CurrencyType = "MtxCurrency",
                        CurrencySubType = "Currency",
                        RegularPrice = price,
                        DynamicRegularPrice = -1,
                        FinalPrice = price
                    }
                },
                GiftInfo = new GiftInfo
                {
                    BIsEnabled = true,
                    ForcedGiftBoxTemplateId = string.Empty,
                    PurchaseRequirements = new List<Requirement>(),
                    GiftRecordIds = new List<string>() 
                },
                DisplayAssetPath = string.IsNullOrWhiteSpace(item.DisplayAssetPath) ? ItemUtils.SetNewDisplayAssetPath($"DA_Daily_{item.Id}") : item.DisplayAssetPath,
                NewDisplayAssetPath = string.IsNullOrWhiteSpace(item.NewDisplayAssetPath) ? string.Empty : item.NewDisplayAssetPath,
                Refundable = true,
                BannerOverride = "",
                Title = "",
                Description = "",
                ShortDescription = "",
                AppStoreId = new List<string>(),
                FulfillmentIds = new List<object>(),
                DailyLimit = -1,
                WeeklyLimit = -1,
                SortPriority = 0,
                CatalogGroupPriority = 0,
                FilterWeight = 0
            };
        }
    }
}
