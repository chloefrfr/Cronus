using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;

namespace ShopGenerator.Storefront.Generation.Prices
{
    public class ItemPrices
    {
        /// <summary>
        /// Gets the price of a item based on its type and rarity.
        /// </summary>
        /// <param name="item">The item to evaluate, containing the type and rarity info.</param>
        /// <returns>
        /// The price of the item as an integer, or <c>null</c> if the item's type or rarity is unknown.
        /// </returns>
        public static int? GetPrice(JSONResponse item)
        {
            string rarity = item.Rarity.BackendValue.Split("::")[1];

            switch (item.Type.BackendValue)
            {
                case "AthenaCharacter":
                    // Uncommon, Rare, Epic, Legendary
                    int[] characterPrices = { 800, 1200, 1500, 2000 };

                    switch (rarity)
                    {
                        case "Uncommon":
                            return characterPrices[0];

                        case "Rare":
                            return characterPrices[1];

                        case "Epic":
                            return characterPrices[2];

                        case "Legendary":
                            return characterPrices[3];
                    }
                    break;

                case "AthenaBackpack":
                    return 100;

                case "AthenaPickaxe":
                    // Uncommon, Rare, Epic
                    int[] pickaxePrices = { 500, 800, 1200 };

                    switch (rarity)
                    {
                        case "Uncommon":
                            return pickaxePrices[0];

                        case "Rare":
                            return pickaxePrices[1];

                        case "Epic":
                            return pickaxePrices[2];
                    }
                    break;

                case "AthenaGlider":
                    // Uncommon, Rare, Epic, Legendary
                    int[] gliderPrices = { 500, 800, 1200, 1500 };

                    switch (rarity)
                    {
                        case "Uncommon":
                            return gliderPrices[0];

                        case "Rare":
                            return gliderPrices[1];

                        case "Epic":
                            return gliderPrices[2];

                        case "Legendary":
                            return gliderPrices[3];
                    }
                    break;

                case "AthenaItemWrap":
                    // Uncommon, Rare
                    int[] wrapPrices = { 300, 500 };

                    switch (rarity)
                    {
                        case "Uncommon":
                            return wrapPrices[0];

                        case "Rare":
                            return wrapPrices[1];

                        case "Epic":
                            return wrapPrices[1];
                    }
                    break;

                case "AthenaSkyDiveContrail":
                case "AthenaDance":
                    // Uncommon, Rare, Epic
                    int[] contrailAndDancePrices = { 200, 500, 800 };

                    switch (rarity)
                    {
                        case "Uncommon":
                            return contrailAndDancePrices[0];

                        case "Rare":
                            return contrailAndDancePrices[1];

                        case "Epic":
                            return contrailAndDancePrices[2];
                    }
                    break;

                default:
                    Logger.Error($"Unknown item type: {item.Type.BackendValue}");
                    break;
            }

            return null;
        }
    }
}
