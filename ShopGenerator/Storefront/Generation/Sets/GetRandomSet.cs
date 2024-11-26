using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Generation.Sets
{
    public class GetRandomSet
    {
        /// <summary>
        /// Gets a random <see cref="StoreSet"/> from the available sets.
        /// </summary>
        /// <returns>A randomly selected <see cref="StoreSet"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no sets are defined in <see cref="Constants._sets"/>.
        /// </exception>
        public static StoreSet Get()
        {
            var keys = Constants._sets.Keys.ToList();

            if (keys.Count == 0)
            {
                throw new InvalidOperationException("No sets available.");
            }

            var randomKey = keys[new Random().Next(keys.Count)];
            return Constants._sets[randomKey];
        }
    }
}
