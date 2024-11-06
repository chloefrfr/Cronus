using System.Collections.Generic;
using Newtonsoft.Json;
using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.RegexMatching;
using ShopGenerator.Storefront.Utilities;

namespace ShopGenerator.Storefront.Services
{
    public class ShopGenerator : IShopGenerator
    {
        private readonly IAPIService _apiService;
        private readonly Dictionary<string, JSONResponse> _items = new();
        private readonly Dictionary<string, StoreSet> _sets = new();
        

        public ShopGenerator(IAPIService apiService)
        {
            _apiService = apiService;
            _items = new Dictionary<string, JSONResponse>();
            _sets = new Dictionary<string, StoreSet>();
        }

        public async Task GenerateShopAsync()
        {
            var cosmeticsData = await _apiService.GetCosmeticsAsync();

            HandleCosmeticData(cosmeticsData);

            var displayAssets = await LoadDisplayAssetsAsync();
            AssignDisplayAssets(displayAssets);
        }

        private void HandleCosmeticData(IEnumerable<JSONResponse> cosmetics)
        {
            foreach (var item in cosmetics)
            {
                if (IsEligibleForShop(item))
                {
                    _items[item.Id] = item;
                    if (item.Set != null)
                    {
                        if (!_sets.ContainsKey(item.Set.BackendValue))
                        {
                            _sets[item.Set.BackendValue] = new StoreSet
                            {
                                Value = item.Set.Value,
                                Text = item.Set.Text,
                                Definition = new List<JSONResponse>()
                            };
                        }
                        _sets[item.Set.BackendValue].Definition.Add(item);
                    }
                }
            }
        }

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

        private bool IsEligibleForShop(JSONResponse item)
        {
            Config config = Config.GetConfig();
            return item.Introduction != null &&
                item.Introduction.BackendValue <= int.Parse(config.CurrentVersion.Split(".")[0]) &&
                item.ShopHistory != null && item.ShopHistory.Count > 0;
        }

        private void AssignDisplayAssets(Dictionary<string, string> displayAssets)
        {
            foreach (var asset in displayAssets)
            {
                var assetParts = asset.Key.Split("_").Skip(1).ToArray();
                var itemKey = string.Join("_", assetParts);

                if (_items.TryGetValue(itemKey, out var item))
                {
                    item.NewDisplayAssetPath = asset.Value;
                } else
                {
                    if (assetParts[0].Contains("CID"))
                    {
                        var match = RegexHelper.MatchRegex(itemKey);
                        if (match != null)
                        {
                            item = _items.Values.FirstOrDefault(i => i.Type.BackendValue.Contains("AthenaCharacter"));
                        }
                    }
                }
            }
        }
    }
}
