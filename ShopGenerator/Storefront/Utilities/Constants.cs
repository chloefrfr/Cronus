using ShopGenerator.Storefront.Enums;
using ShopGenerator.Storefront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Utilities
{
    public class Constants
    {
        public static Dictionary<string, JSONResponse> _items = new();
        public static Dictionary<string, StoreSet> _sets = new();
        public static Dictionary<string, CosmeticTypes> _cosmeticTypes = new();

        public static readonly HashSet<string> ExcludedBackendValues = new HashSet<string>
        {
            "AthenaBackpack",
            "AthenaSkyDiveContrail",
            "AthenaMusicPack",
            "AthenaToy"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Storefronts"/> class with the specified section name.
        /// </summary>
        /// <param name="section">The name of the section for the storefront.</param>
        /// <returns>A <see cref="Storefronts"/> object with the specified name and an empty catalog.</returns>
        public static Storefronts InitializeStorefront(string section)
        {
            return new Storefronts
            {
                Name = section,
                CatalogEntries = new List<Entry>()
            };
        }

        /// <summary>
        /// Pushes changes from the specified <see cref="Storefronts"/> object to the given <see cref="Shop"/>.
        /// </summary>
        /// <param name="shop">The shop to which changes will be applied.</param>
        /// <param name="changes">The storefront changes to be added.</param>
        public static void PushChanges(Shop shop, Storefronts changes)
        {
            shop.Storefronts.Add(changes);
        }
    }
}
