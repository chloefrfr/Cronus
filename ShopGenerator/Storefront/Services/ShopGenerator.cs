using System.Collections.Generic;
using ShopGenerator.Storefront.Models;
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

            // TODO: Add DisplayAssets
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

        private bool IsEligibleForShop(JSONResponse item)
        {
            Config config = Config.GetConfig();
            return item.Introduction != null &&
                item.Introduction.BackendValue <= int.Parse(config.CurrentVersion) &&
                item.ShopHistory != null && item.ShopHistory.Count > 0;
        }
    }
}
