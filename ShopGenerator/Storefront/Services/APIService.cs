using System.Net;
using System.Text.Json;
using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;

namespace ShopGenerator.Storefront.Services
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string CosmeticsCacheKey = "CosmeticsData";

        public APIService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestVersion = HttpVersion.Version20; 
            _cache = cache;

            _ = PreloadCosmeticsDataAsync();
        }

        /// <summary>
        /// Asynchronously gets cosmetics data from the API.
        /// Uses caching to avoid redundant API calls.
        /// </summary>
        /// <returns>A list of cosmetics data represented by <see cref="JSONResponse"/>.</returns>
        public async Task<List<JSONResponse>> GetCosmeticsAsync()
        {
            if (_cache.TryGetValue(CosmeticsCacheKey, out List<JSONResponse> cachedData))
            {
                return cachedData;
            }

            try
            {
                using var response = await _httpClient.GetAsync(ShopGlobals.CosmeticEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to get data: {response.ReasonPhrase}");
                    return new List<JSONResponse>();
                }

                var apiResponse = await JsonSerializer.DeserializeAsync<APIResponse>(
                    await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                    {
                        DefaultBufferSize = 1024,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                var cosmeticsData = apiResponse?.Data ?? new List<JSONResponse>();

                _cache.Set(CosmeticsCacheKey, cosmeticsData, TimeSpan.FromMinutes(10)); 

                return cosmeticsData;
            }
            catch (HttpRequestException httpEx)
            {
                Logger.Error($"HTTP request error: {httpEx.Message}");
                throw;
            }
        }

        /// <summary>
        /// Preloads cosmetics data into the cache to improve initial load performance.
        /// Logs a warning if preloading fails.
        /// </summary>
        private async Task PreloadCosmeticsDataAsync()
        {
            try
            {
                await GetCosmeticsAsync();
            }
            catch (Exception ex)
            {
                Logger.Warning($"Preload of cosmetics data failed: {ex.Message}");
            }
        }
    }
}
