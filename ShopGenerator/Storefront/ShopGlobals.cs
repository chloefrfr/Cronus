namespace ShopGenerator.Storefront
{
    public class ShopGlobals
    {
        public static string ApiBaseUrl { get; } = "https://fortnite-api.com/v2";
        public static string CosmeticEndpoint { get; } = $"{ApiBaseUrl}/cosmetics/br?responseFlags=4";
    }
}
