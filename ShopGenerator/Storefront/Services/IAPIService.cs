using ShopGenerator.Storefront.Models;

namespace ShopGenerator.Storefront.Services
{
    public interface IAPIService
    {
        public Task<List<JSONResponse>> GetCosmeticsAsync();
    }
}
