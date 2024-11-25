using ShopGenerator.Storefront.Services;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopGenerator
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // testing
            var httpClient = new HttpClient { DefaultRequestVersion = HttpVersion.Version20 };
            var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
            var apiService = new APIService(httpClient, memoryCache);
            var shopGenerator = new ShopGenerator.Storefront.Services.ShopGenerator(apiService);
            await shopGenerator.GenerateShopAsync();
            Console.WriteLine(JsonSerializer.Serialize(shopGenerator.shop, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
