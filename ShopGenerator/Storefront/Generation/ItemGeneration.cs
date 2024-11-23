using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Generation
{
    public class ItemGeneration
    {
        /// <summary>
        /// Gets a random item that hasn't been added yet and is not excluded based on its backend value.
        /// </summary>
        /// <param name="addedItemIds">A set of item IDs that have already been added, to avoid duplicates.</param>
        /// <returns>A random valid item from the list of stored items.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no items are available in the store.</exception>
        public JSONResponse GetRandomItem(HashSet<string> addedItemIds)
        {
            Random random = new Random();
            JSONResponse randomItem = null;

            if (Constants._items.Count == 0)
            {
                throw new InvalidOperationException("No stored items available.");
            }

            while (true)
            {
                var keys = Constants._items.Keys.ToList();

                if (keys.Count == 0) continue;

                var keyList = new List<string>(keys);
                var randomKey = keyList[random.Next(keyList.Count)];

                randomItem = Constants._items[randomKey];

                if (!Constants.ExcludedBackendValues.Contains(randomItem.Type.BackendValue) && !addedItemIds.Contains(randomItem.Id))
                {
                    // Break the loop if a valid item is found.
                    break;
                }
            }

            return randomItem;
        }

        /// <summary>
        /// Fills the daily storefront with a selection of random items.
        /// </summary>
        /// <param name="dailySection">The storefront section to be filled with items.</param>
        public async Task FillDailyStorefront(Storefronts dailySection)
        {
            var characters = 0;
            var dances = 0;

            var addedItemIds = new HashSet<string>();
            
            while (dailySection.CatalogEntries.Count < 6)
            {
                JSONResponse randomItem = GetRandomItem(addedItemIds);

                if (randomItem.Type.BackendValue == "AthenaCharacter" && characters < 2)
                {
                    characters++;
                }
                else if (randomItem.Type.BackendValue == "AthenaDance" && dances < 2)
                {
                    dances++;
                }
                else if (randomItem.Type.BackendValue == "AthenaCharacter" && characters >= 2)
                {
                    continue; 
                }


            }
        }

        public async Task FillWeeklyStorefront(Storefronts weeklySection)
        {

        }
    }
}
