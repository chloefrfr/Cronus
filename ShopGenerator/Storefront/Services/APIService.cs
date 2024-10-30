using System.Net;
using System.Text.Json;
using ShopGenerator.Storefront.Models;
using ShopGenerator.Storefront.Utilities;

namespace ShopGenerator.Storefront.Services
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _httpClient;

        public APIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JSONResponse>> GetCosmeticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ShopGlobals.CosmeticEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get data: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<APIResponse>(content);
                return apiResponse?.Data ?? new List<JSONResponse>();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error retrieving data: {ex.Message}");
                throw;
            }
        }

    }
}
