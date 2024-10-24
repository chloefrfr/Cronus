using Larry.Source.Classes.Profiles;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.QueryBuilder;
using Larry.Source.Mappings;
using System.Diagnostics;
using Newtonsoft.Json;
using Larry.Source.Interfaces;
using Larry.Source.Classes.MCP;
using System.Text.Json;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using System.Collections;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using CUE4Parse.Utils;
using CUE4Parse.FileProvider;
using K4os.Compression.LZ4.Internal;
using System.Collections.Concurrent;

namespace Larry.Source.Utilities.Managers
{
    /// <summary>
    /// Manages user profiles, including creating and retrieving profile data.
    /// </summary>
    public class ProfileManager
    {
        /// <summary>
        /// Asynchronously creates a profile based on the specified type and account ID.
        /// </summary>
        /// <param name="type">The type of profile to create.</param>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A task that represents the asynchronous operation, containing the created profile.</returns>
        public static async Task<Profiles> CreateProfileAsync(string type, string accountId)
        {
            var config = Config.GetConfig();
            var profileRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);

            var newProfile = new Profiles
            {
                AccountId = accountId,
                ProfileId = type,
                Revision = 0
            };


            if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(type))
            {
                Logger.Error($"Invalid accountId or type: {accountId}, {type}");
                return null;
            }

            try
            {
                await profileRepository.SaveAsync(newProfile);
                switch (type)
                {
                    case ProfileIds.Athena:
                        await CreateAthenaProfileAsync(accountId, itemsRepository, newProfile);
                        break;

                    case ProfileIds.CommonCore:
                        await CreateCommonCoreProfileAsync(accountId, itemsRepository, newProfile);
                        break;

                    default:
                        Logger.Error($"Failed to find profileId: {type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create profile for account {accountId} with type {type}: {ex.Message}");
                return new Profiles();
            }

            return newProfile;
        }

        /// <summary>
        /// Asynchronously creates an Athena profile and saves its items and stats.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="itemsRepository">The repository for managing item data.</param>
        private static async Task CreateAthenaProfileAsync(string accountId, Repository<Items> itemsRepository, Larry.Source.Database.Entities.Profiles profile)
        {
            var athenaItems = CreateAthenaItems(accountId);
            Console.WriteLine(JsonConvert.SerializeObject(profile));
            var athenaProfile = new AthenaProfile(accountId, athenaItems, profile);
            var constructedAthenaProfile = athenaProfile.CreateProfile(accountId, athenaItems, profile);

            athenaProfile.Profile.version = $"Larry/{accountId}/${ProfileIds.Athena}/{DateTime.UtcNow:O}";

            await SaveProfileAttributesAsync(constructedAthenaProfile, itemsRepository, ProfileIds.Athena, accountId);
        }

        /// <summary>
        /// Creates a list of default items for the Athena profile.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A list of items for the Athena profile.</returns>
        private static List<Items> CreateAthenaItems(string accountId)
        {
            return new List<Items>
            {
                CreateItem(ProfileIds.Athena, accountId, "AthenaPickaxe:DefaultPickaxe"),
                CreateItem(ProfileIds.Athena, accountId, "AthenaGlider:DefaultGlider"),
                CreateItem(ProfileIds.Athena, accountId, "AthenaDance:EID_DanceMoves"),
                CreateItem(ProfileIds.Athena, accountId, "AthenaCharacter:CID_001_Athena_Commando_F_Default"),
                CreateStatItem(ProfileIds.Athena, accountId, "use_random_loadout", false),
                CreateStatItem(ProfileIds.Athena, accountId, "past_seasons", new List<PastSeasons>()),
                CreateStatItem(ProfileIds.Athena, accountId, "season_match_boost", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "loadouts", new List<string>()),
                CreateStatItem(ProfileIds.Athena, accountId, "mfa_reward_claimed", false),
                CreateStatItem(ProfileIds.Athena, accountId, "rested_xp_overflow", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "current_mtx_platform", "Epic"),
                CreateStatItem(ProfileIds.Athena, accountId, "last_xp_interaction", DateTime.UtcNow.ToString("o")),
                CreateStatItem(ProfileIds.Athena, accountId, "quest_manager", new QuestManager
                {
                    dailyLoginInterval = DateTime.MinValue.ToString("o"),
                    dailyQuestRerolls = 1,
                    questPoolStats = new QuestPoolStats()
                }),
                CreateStatItem(ProfileIds.Athena, accountId, "book_level", 1),
                CreateStatItem(ProfileIds.Athena, accountId, "season_num", 1),
                CreateStatItem(ProfileIds.Athena, accountId, "book_xp", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "creative_dynamic_xp", new Dictionary<string, int>()),
                CreateStatItem(ProfileIds.Athena, accountId, "season", new Season { numWins = 0, numHighBracket = 0, numLowBracket = 0 }),
                CreateStatItem(ProfileIds.Athena, accountId, "lifetime_wins", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "book_purchased", false),
                CreateStatItem(ProfileIds.Athena, accountId, "rested_xp_exchange", 1),
                CreateStatItem(ProfileIds.Athena, accountId, "level", 1),
                CreateStatItem(ProfileIds.Athena, accountId, "rested_xp", 2500),
                CreateStatItem(ProfileIds.Athena, accountId, "rested_xp_mult", 4),
                CreateStatItem(ProfileIds.Athena, accountId, "accountLevel", 1),
                CreateStatItem(ProfileIds.Athena, accountId, "rested_xp_cumulative", 52500),
                CreateStatItem(ProfileIds.Athena, accountId, "xp", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "active_loadout_index", 0),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_character", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_pickaxe", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_glider", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_backpack", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_skydivecontrail", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_loadingscreen", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_musicpack", ""),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_dance", new List<string>()),
                CreateStatItem(ProfileIds.Athena, accountId, "favorite_itemwraps", new List<string>())

            };
        }

        /// <summary>
        /// Asynchronously creates a Common Core profile and saves its items and stats.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="itemsRepository">The repository for managing item data.</param>
        private static async Task CreateCommonCoreProfileAsync(string accountId, Repository<Items> itemsRepository, Larry.Source.Database.Entities.Profiles profile)
        {
            var commonCoreItems = CreateCommonCoreItems(accountId);
            var commonCoreProfile = new CommonCoreProfile(accountId, commonCoreItems, profile);
            var constructedCommonCoreProfile = commonCoreProfile.CreateProfile(accountId, commonCoreItems, profile);

            constructedCommonCoreProfile.version = $"Larry/{accountId}/${ProfileIds.CommonCore}/{DateTime.UtcNow:O}";

            await SaveProfileAttributesAsync(constructedCommonCoreProfile, itemsRepository, ProfileIds.CommonCore, accountId);
        }

        /// <summary>
        /// Creates a list of default items for the CommonCore profile.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A list of items for the CommonCore profile.</returns>
        private static List<Items> CreateCommonCoreItems(string accountId)
        {
            return new List<Items>
            {
                CreateCCItem(ProfileIds.CommonCore, accountId, "Currency:MtxPurchased")
            };
        }

        /// <summary>
        /// Asynchronously saves the attributes of the given profile and its items.
        /// </summary>
        /// <param name="constructedProfile">The profile containing items and stats to save.</param>
        /// <param name="itemsRepository">The repository for managing item data.</param>
        /// <param name="profileId">The ID of the profile.</param>
        /// <param name="accountId">The account ID associated with the profile.</param>
        private static async Task SaveProfileAttributesAsync(dynamic constructedProfile, Repository<Items> itemsRepository, string profileId, string accountId)
        {
            foreach (var itemData in constructedProfile.items)
            {
                await SaveItemAttributesAsync(profileId, accountId, itemData.Value, itemsRepository, false);
            }

            await SaveStatAttributesAsync(profileId, accountId, constructedProfile.stats.attributes, itemsRepository);
        }

        /// <summary>
        /// Creates a new item with the specified profile ID and template ID.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        private static Items CreateItem(string profileId, string accountId, string templateId)
        {
            return new Items
            {
                AccountId = accountId,
                ProfileId = profileId,
                TemplateId = templateId,
                Value = System.Text.Json.JsonSerializer.Serialize(new ItemValue
                {
                    xp = 0,
                    level = 1,
                    variants = new List<Variants>(),
                    item_seen = false,
                }),
                Quantity = 1,
                IsStat = false
            };
        }

        /// <summary>
        /// Creates a new item with the specified profile ID and template ID.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        private static Items CreateCCItem(string profileId, string accountId, string templateId)
        {
            var itemValue = new ItemValue();

            if (templateId == "Currency:MtxPurchased")
            {
                itemValue.platform = "EpicPC";
            }
            else
            {
                itemValue.level = 1;
            }

            return new Items
            {
                AccountId = accountId,
                ProfileId = profileId,
                TemplateId = templateId,
                Value = System.Text.Json.JsonSerializer.Serialize(itemValue),
                Quantity = templateId == "Currency:MtxPurchased" ? 0 : 1,
                IsStat = false
            };
        }

        private static Items CreateStatItem(string profileId, string accountId, string templateId, dynamic value)
        {
            return new Items
            {
                AccountId = accountId,
                ProfileId = profileId,
                TemplateId = templateId,
                Value = System.Text.Json.JsonSerializer.Serialize(value),
                Quantity = 1,
                IsStat = true
            };
        }

        /// <summary>
        /// Saves the attributes of an item asynchronously.
        /// </summary>
        /// <param name="item">The item whose attributes are to be saved.</param>
        /// <param name="itemsRepository">The repository for items.</param>
        private static async Task SaveItemAttributesAsync(string profileId, string accountId, ItemDefinition item, Repository<Items> itemsRepository, bool isStat)
        {
            //Console.WriteLine(ie)
            if (item.attributes == null)
            {
                return;
            }

            dynamic relevantAttributes = new System.Dynamic.ExpandoObject();

            var attributesType = item.attributes.GetType();
            foreach (var property in attributesType.GetProperties())
            {
                var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
                var jsonPropertyName = jsonProperty != null ? jsonProperty.PropertyName : property.Name;

                switch (jsonPropertyName)
                {
                    case "favorite":
                        ((IDictionary<string, object>)relevantAttributes)["favorite"] = (bool)property.GetValue(item.attributes);
                        break;
                    case "item_seen":
                        ((IDictionary<string, object>)relevantAttributes)["item_seen"] = (bool)property.GetValue(item.attributes);
                        break;
                    case "xp":
                        ((IDictionary<string, object>)relevantAttributes)["xp"] = (int)property.GetValue(item.attributes);
                        break;
                    case "variants":
                        ((IDictionary<string, object>)relevantAttributes)["variants"] = (List<Variants>)property.GetValue(item.attributes);
                        break;
                }
            }

            var newItem = new Items
            {
                ProfileId = profileId,
                AccountId = accountId,
                TemplateId = item.templateId,
                Value = System.Text.Json.JsonSerializer.Serialize(relevantAttributes),
                Quantity = item.quantity,
                IsStat = isStat
            };

            await itemsRepository.SaveAsync(newItem);
        }

        /// <summary>
        /// Asynchronously saves stat attributes for a specified user profile and account.
        /// </summary>
        /// <param name="profileId">The unique identifier for the user profile.</param>
        /// <param name="accountId">The unique identifier for the user account.</param>
        /// <param name="stats">An object containing the stat attributes to be saved.</param>
        /// <param name="itemsRepository">The repository instance used to save item data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the saving process.</exception>
        private static async Task SaveStatAttributesAsync(string profileId, string accountId, dynamic stats, Repository<Items> itemsRepository)
        {
            try
            {
                // Idk what happened but the type of the stats changed three different times within the span of 4 minutes
                foreach (var stat in stats)
                {
                    var newItem = new Items
                    {
                        ProfileId = profileId,
                        AccountId = accountId,
                        TemplateId = stat.Key,
                        Value = System.Text.Json.JsonSerializer.Serialize(stat.Value),
                        Quantity = 1,
                        IsStat = true
                    };

                    await itemsRepository.SaveAsync(newItem);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save attributes: {ex.Message}");
            }
        }

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
                { "wraps", "AthenaItemWrap" },
                { "gliders", "AthenaGlider" },
                { "contrails", "AthenaSkyDiveContrail" },
                { "petcarriers", "AthenaPetCarrier" },
                { "battlebuses", "AthenaBattleBus" },
                { "victoryposes", "AthenaVictoryPose" },
                { "consumableemotes", "AthenaConsumableEmote" }
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

                    if (isAlreadyInDB != null)
                    {
                        Logger.Warning("???");
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
                            variants = new List<Variants>(),
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
                    Logger.Error($"Unknown cosmetic type: {cosmeticPath}");
                }
            }));
        }

        /// <summary>
        /// Asynchronously gets a user profile based on the provided account ID.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="IProfile"/> object if found, or null otherwise.</returns>
        /// <summary>
        /// Asynchronously gets a user profile based on the provided account ID.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="Profile"/> object if found, or null otherwise.</returns>
        public static async Task<IProfile?> GetProfileAsync(string accountId, string profileId)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                Config config = Config.GetConfig();
                Repository<Items> itemsRepository = new Repository<Items>(config.ConnectionUrl);
                Repository<Profiles> profilesRepository = new Repository<Profiles>(config.ConnectionUrl);

                Profiles primaryProfile = await profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);
                List<Items> profileItems = await itemsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);


                IProfile constructedItems = null;

                switch (profileId)
                {
                    case "athena":
                        var athenaBuilder = new AthenaProfile(accountId, profileItems, primaryProfile);
                        constructedItems = athenaBuilder.GetProfile();
                        break;

                    case "common_core":
                        var commonCoreBuilder = new CommonCoreProfile(accountId, profileItems, primaryProfile);
                        constructedItems = commonCoreBuilder.GetProfile();
                        break;

                    default:
                        Logger.Error($"Missing profile: {primaryProfile.ProfileId}");
                        break;
                }

                if (constructedItems == null)
                {
                    Logger.Error($"constructedItems is null, ProfileId: {primaryProfile.ProfileId}.");
                    return null;
                }

                return constructedItems;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while getting profile for accountId {accountId}: {ex.Message}");
                return null;
            }
            finally
            {
                if (timer.IsRunning)
                {
                    timer.Stop();
                }
            }
        }
    }
}