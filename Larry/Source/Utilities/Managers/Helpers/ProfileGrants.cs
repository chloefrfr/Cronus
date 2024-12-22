using CUE4Parse.Utils;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using System.Collections.Concurrent;

namespace Larry.Source.Utilities.Managers.Helpers
{
    public static class ProfileGrants
    {
        /// <summary>
        /// Grants all cosmetics to the specified account by loading all cosmetic files, 
        /// determining their type, and saving them into the database if not already present.
        /// </summary>
        /// <param name="accountId">The account ID to which cosmetics should be granted.</param>
        public static async Task GrantAll(string accountId)
        {
            Config config = Config.GetConfig();
            List<string> cosmeticFiles = await Program._fileProviderManager.LoadAllCosmeticsAsync();
            Repository<Items> itemRepository = new Repository<Items>(config.ConnectionUrl);
            var itemsToSave = new ConcurrentBag<Items>();

            var cosmeticTypeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "characters", "AthenaCharacter" },
                { "backpacks", "AthenaBackpack" },
                { "pickaxes", "AthenaPickaxe" },
                { "dances", "AthenaDance" },
                { "musicpacks", "AthenaMusicPack" },
                { "pets", "AthenaBackpack" },
                { "sprays", "AthenaDance" },
                { "toys", "AthenaDance" },
                { "loadingscreens", "AthenaLoadingScreen" },
                { "gliders", "AthenaGlider" },
                { "contrails", "AthenaSkyDiveContrail" },
                { "petcarriers", "AthenaPetCarrier" },
                { "battlebuses", "AthenaBattleBus" },
                { "victoryposes", "AthenaVictoryPose" },
                { "consumableemotes", "AthenaConsumableEmote" },
                { "wraps", "AthenaItemWrap" },
                { "itemwraps", "AthenaItemWrap" }
            };

            string GetCosmeticTypeKey(string cosmeticPath)
            {
                var pathSegments = cosmeticPath.ToLower().Split('/');
                return pathSegments.Length > 5 ? pathSegments[5] : string.Empty;
            }

            await Task.WhenAll(cosmeticFiles.Select(async cosmeticPath =>
            {
                var cosmeticName = cosmeticPath.SubstringAfterLast("/").SubstringBefore(".");
                var cosmeticTypeKey = GetCosmeticTypeKey(cosmeticPath);

                if (cosmeticTypeMapping.TryGetValue(cosmeticTypeKey, out var cosmeticType))
                {
                    var templateId = $"{cosmeticType}:{cosmeticName}";
                    var isAlreadyInDB = await itemRepository.FindByTemplateIdAsync(templateId);
                    var variant = await Program._fileProviderManager.GetVariantsAsync(cosmeticPath.SubstringBefore("."));

                    if (isAlreadyInDB != null)
                    {
                        return;
                    }


                    var newItem = new Items
                    {
                        AccountId = accountId,
                        ProfileId = "athena",
                        TemplateId = templateId,
                        Value = System.Text.Json.JsonSerializer.Serialize(new ItemValue
                        {
                            item_seen = false,
                            variants = variant,
                            xp = 0,
                            favorite = false
                        }),
                        Quantity = 1,
                        IsStat = false
                    };

                    await itemRepository.SaveAsync(newItem);
                    itemsToSave.Add(newItem);
                }
                else
                {
                    Logger.Error($"Unknown cosmetic type: {cosmeticName}");
                }
            }));
        }
    }
}
