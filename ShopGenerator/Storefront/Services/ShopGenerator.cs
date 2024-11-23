using System.Collections.Generic;
using Newtonsoft.Json;
using ShopGenerator.Storefront.Enums;
using ShopGenerator.Storefront.Generation;
using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.RegexMatching;
using ShopGenerator.Storefront.Utilities;

namespace ShopGenerator.Storefront.Services
{
    public class ShopGenerator : IShopGenerator
    {
        private readonly IAPIService _apiService;


        public ShopGenerator(IAPIService apiService)
        {
            _apiService = apiService;
            Constants._items = new Dictionary<string, JSONResponse>();
            Constants._sets = new Dictionary<string, StoreSet>();
            Constants._cosmeticTypes = new Dictionary<string, CosmeticTypes>();
        }


        /// <summary>
        /// Asynchronously generates the shop by loading and generating cosmetic data, 
        /// assigning display assets, and organizing storefront sections.
        /// </summary>
        public async Task GenerateShopAsync()
        {
            var cosmeticsData = await _apiService.GetCosmeticsAsync();

            HandleCosmeticData(cosmeticsData);

            var displayAssets = await LoadDisplayAssetsAsync();
            AssignDisplayAssets(displayAssets);

            foreach (var item in Constants._items.Values
            .Where(item => item.Type.BackendValue.Contains("AthenaBackpack") && !string.IsNullOrEmpty(item.ItemPreviewHeroPath)))
            {
                var cosmeticId = item.ItemPreviewHeroPath.Split("/").LastOrDefault();
                if (string.IsNullOrEmpty(cosmeticId)) continue;

                if (Constants._items.TryGetValue(cosmeticId, out var cosmetic))
                {
                    cosmetic.Backpack = item;
                }
            }

            var dailySection = Constants.InitializeStorefront("BRDailyStorefront");
            var weeklySection = Constants.InitializeStorefront("BRWeeklyStorefront");

            var generation = new ItemGeneration();

            await generation.FillDailyStorefront(dailySection);
            await generation.FillWeeklyStorefront(weeklySection);
        }

        /// <summary>
        /// Generates and organizes cosmetic data into items and sets for the shop.
        /// </summary>
        /// <param name="cosmetics">The list of cosmetic data found from the API.</param>
        private void HandleCosmeticData(IEnumerable<JSONResponse> cosmetics)
        {
            foreach (var item in cosmetics)
            {
                if (IsEligibleForShop(item))
                {
                    var itemType = item.Type is CosmeticType cosmeticType ? cosmeticType.BackendValue : null;

                    if (itemType != null && Constants._cosmeticTypes.ContainsKey(itemType.ToString()))
                    {
                        item.Type.BackendValue = Constants._cosmeticTypes[itemType.ToString()].ToString();
                    }

                    if (itemType == null) continue;

                    if (item.Set != null)
                    {
                        if (!Constants._sets.ContainsKey(item.Set.BackendValue))
                        {
                            Constants._sets[item.Set.BackendValue] = new StoreSet
                            {
                                Value = item.Set.Value,
                                Text = item.Set.Text,
                                Definition = new List<JSONResponse>()
                            };
                        }
                        Constants._sets[item.Set.BackendValue].Definition.Add(item);
                    }

                    Constants._items[item.Id] = item;
                }
            }
        }

        /// <summary>
        /// Asynchronously loads display assets from the json file.
        /// </summary>
        /// <returns>A dictionary containing the display asset paths.</returns>
        private async Task<Dictionary<string, string>> LoadDisplayAssetsAsync()
        {
            var assets = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Memory", "displayAssets.json");

            using (var reader = new StreamReader(assets))
            {
                string json = await reader.ReadToEndAsync();
                var displayAssets = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return displayAssets;
            }
        }

        /// <summary>
        /// Determines if a cosmetic item is eligible to be added to the shop.
        /// </summary>
        /// <param name="item">The cosmetic item to check.</param>
        /// <returns><c>true</c> if the item is eligible; otherwise, <c>false</c>.</returns>
        private bool IsEligibleForShop(JSONResponse item)
        {
            Config config = Config.GetConfig();
            return item.Introduction != null &&
                item.Introduction.BackendValue <= int.Parse(config.CurrentVersion.Split(".")[0]) &&
                item.ShopHistory != null && item.ShopHistory.Count > 0;
        }

        /// <summary>
        /// Assigns display asset paths to corresponding cosmetic items.
        /// </summary>
        /// <param name="displayAssets">A dictionary containing display asset paths.</param>
        private void AssignDisplayAssets(Dictionary<string, string> displayAssets)
        {
            foreach (var asset in displayAssets)
            {
                var assetParts = asset.Key.Split("_").Skip(1).ToArray();
                var itemKey = string.Join("_", assetParts);

                if (Constants._items.TryGetValue(itemKey, out var item))
                {
                    item.NewDisplayAssetPath = asset.Value;
                } else
                {
                    if (assetParts[0].Contains("CID"))
                    {
                        var match = RegexHelper.MatchRegex(itemKey);
                        if (match != null)
                        {
                            item = Constants._items.Values.FirstOrDefault(i => i.Type.BackendValue.Contains("AthenaCharacter"));
                        }
                    }
                }
            }
        }
    }
}
