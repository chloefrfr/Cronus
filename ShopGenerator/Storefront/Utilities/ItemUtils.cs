using ShopGenerator.Storefront.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Utilities
{
    public class ItemUtils
    {
        /// <summary>
        /// Gets the base item ID by removing predefined prefixes from the full item ID.
        /// </summary>
        /// <param name="fullItemId">The full item ID containing a prefix.</param>
        /// <returns>The base item ID without the prefix.</returns>
        public static string GetBaseItemId(string fullItemId)
        {
            List<string> prefixesToRemove = new List<string>
            {
                "AthenaCharacter",
                "AthenaGlider",
                "AthenaPickaxe",
                "AthenaItemWrap",
                "AthenaDance"
            };

            foreach (var prefix in prefixesToRemove)
            {
                if (fullItemId.Contains(prefix))
                {
                    return fullItemId.Replace(prefix + ":", "");
                }
            }

            return fullItemId;
        }

        /// <summary>
        /// Generates the display asset path for an item based on its base item ID.
        /// </summary>
        /// <param name="item">The full item ID.</param>
        /// <returns>The display asset path for the item.</returns>
        public static string SetDisplayAsset(string item)
        {
            string baseItemId = GetBaseItemId(item);
            return $"/Game/Catalog/DisplayAssets/{baseItemId}.{baseItemId}";
        }

        /// <summary>
        /// Generates the new display asset path for an item.
        /// </summary>
        /// <param name="item">The full item ID.</param>
        /// <returns>The new display asset path for the item.</returns>
        public static string SetNewDisplayAssetPath(string item)
        {
            string baseItemId = GetBaseItemId(item);
            string newDisplayAsset = $"DAv2_Featured_{baseItemId}";
            return $"/Game/Catalog/NewDisplayAssets/{newDisplayAsset}.{newDisplayAsset}";
        }

        /// <summary>
        /// Gets the display asset path for an item based on its base item ID.
        /// </summary>
        /// <param name="item">The full item ID.</param>
        /// <returns>The display asset path for the item.</returns>
        public static string GetDisplayAsset(string item)
        {
            string baseItemId = GetBaseItemId(item);
            return $"/Game/Catalog/DisplayAssets/{baseItemId}.{baseItemId}";
        }

        /// <summary>
        /// Adds an item grant to the entry with the specified type and ID.
        /// </summary>
        /// <param name="entry">The entry to which the item grant will be added.</param>
        /// <param name="type">The type of the item.</param>
        /// <param name="id">The ID of the item.</param>
        public static void AddItemGrant(Entry entry, string type, string id)
        {
            entry.ItemGrants.Add(new ItemGrant { TemplateId = $"{type}:{id}", Quantity = 1 });
        }

        /// <summary>
        /// Adds a requirement to the entry with the specified type and ID.
        /// </summary>
        /// <param name="entry">The entry to which the requirement will be added.</param>
        /// <param name="type">The type of the requirement.</param>
        /// <param name="id">The ID of the requirement.</param>
        public static void AddRequirement(Entry entry, string type, string id)
        {
            entry.Requirements.Add(Constants.DefaultRequirements(type, id));
        }
    }
}
