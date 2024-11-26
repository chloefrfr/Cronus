using ShopGenerator.Storefront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Generation.Sets
{
    public class GetRandomFullSetLength
    {
        /// <summary>
        /// Calculates the count of unique first categories from the given list of entries.
        /// Entries are grouped internally by their first category for potential additional use.
        /// </summary>
        /// <param name="entries">A list of <see cref="Entry"/> objects containing category information.</param>
        /// <returns>
        /// The count of unique first categories across all entries.
        /// Categories are identified by the first element in the <see cref="Entry.Categories"/> list.
        /// </returns>
        public static int Get(List<Entry> entries)
        {
            var categoryEntriesMap = new Dictionary<string, List<Entry>>();
            var uniqueCategories = new HashSet<string>();

            foreach (var entry in entries)
            {
                if (entry.Categories == null || entry.Categories.Count == 0) continue;

                string category = entry.Categories[0];
                uniqueCategories.Add(category);

                if (!categoryEntriesMap.TryGetValue(category, out var categoryList))
                {
                    categoryList = new List<Entry>();
                    categoryEntriesMap[category] = categoryList;
                }

                categoryList.Add(entry);
            }

            return uniqueCategories.Count;
        }
    }
}
