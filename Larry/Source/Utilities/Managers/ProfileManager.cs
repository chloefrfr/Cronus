using Larry.Source.Classes.Profiles;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
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
using Microsoft.IdentityModel.Tokens;
using Larry.Source.Utilities.Managers.Helpers;

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

            var loadoutsRepository = new Repository<Loadouts>(config.ConnectionUrl);

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
                        var loadouts = await loadoutsRepository.GetAllItemsByAccountIdAsync(accountId, "athena");
                        await CreateAthenaProfileAsync(accountId, itemsRepository, loadouts, newProfile);
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
        private static async Task CreateAthenaProfileAsync(string accountId, Repository<Items> itemsRepository, List<Loadouts> loadouts, Larry.Source.Database.Entities.Profiles profile)
        {
            var athenaItems = CreateAthenaItems(accountId);
            var athenaProfile = new AthenaProfile(accountId, athenaItems, loadouts, profile);
            var constructedAthenaProfile = athenaProfile.CreateProfile(accountId, athenaItems, loadouts, profile);

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
            var items = new List<Items>
            {
                ProfileCreation.CreateItem(ProfileIds.Athena, accountId, "AthenaPickaxe:DefaultPickaxe"),
                ProfileCreation.CreateItem(ProfileIds.Athena, accountId, "AthenaGlider:DefaultGlider"),
                ProfileCreation.CreateItem(ProfileIds.Athena, accountId, "AthenaDance:EID_DanceMoves"),
                ProfileCreation.CreateItem(ProfileIds.Athena, accountId, "AthenaCharacter:CID_001_Athena_Commando_F_Default"),
            };

            var statItems = new (string key, object value)[]
            {
                ("use_random_loadout", false),
                ("past_seasons", new List<PastSeasons>()),
                ("season_match_boost", 0),
                ("loadouts", new List<string>()),
                ("mfa_reward_claimed", false),
                ("rested_xp_overflow", 0),
                ("current_mtx_platform", "Epic"),
                ("last_xp_interaction", DateTime.UtcNow.ToString("o")),
                ("quest_manager", new QuestManager
                {
                    dailyLoginInterval = DateTime.MinValue.ToString("o"),
                    dailyQuestRerolls = 1,
                    questPoolStats = new QuestPoolStats()
                }),
                ("book_level", 1),
                ("season_num", 1),
                ("book_xp", 0),
                ("creative_dynamic_xp", new Dictionary<string, int>()),
                ("season", new Season { numWins = 0, numHighBracket = 0, numLowBracket = 0 }),
                ("lifetime_wins", 0),
                ("book_purchased", false),
                ("rested_xp_exchange", 1),
                ("level", 1),
                ("rested_xp", 2500),
                ("rested_xp_mult", 4),
                ("accountLevel", 1),
                ("rested_xp_cumulative", 52500),
                ("xp", 0),
                ("active_loadout_index", 0),
                ("favorite_character", "AthenaCharacter:CID_001_Athena_Commando_F_Default"),
                ("favorite_pickaxe", "AthenaPickaxe:DefaultPickaxe"),
                ("favorite_glider", "AthenaGlider:DefaultGlider"),
                ("favorite_backpack", ""),
                ("favorite_skydivecontrail", ""),
                ("favorite_loadingscreen", ""),
                ("favorite_musicpack", ""),
                ("favorite_dance", new List<string>()),
                ("favorite_itemwraps", new List<string>())
            };

            foreach (var (key, value) in statItems)
            {
                items.Add(ProfileCreation.CreateStatItem(ProfileIds.Athena, accountId, key, value));
            }

            return items;
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
               ProfileCreation.CreateCCItem(ProfileIds.CommonCore, accountId, "Currency:MtxPurchased")
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
        /// Saves the attributes of an item asynchronously.
        /// </summary>
        /// <param name="item">The item whose attributes are to be saved.</param>
        /// <param name="itemsRepository">The repository for items.</param>
        private static async Task SaveItemAttributesAsync(string profileId, string accountId, ItemDefinition item, Repository<Items> itemsRepository, bool isStat)
        {
            if (item.attributes == null)
            {
                return;
            }

            var relevantAttributes = new Dictionary<string, object>();
            var missingProperties = new List<string>();

            var attributesType = item.attributes.GetType();
            foreach (var property in attributesType.GetProperties())
            {
                var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
                var jsonPropertyName = jsonProperty?.PropertyName ?? property.Name;
                var propertyValue = property.GetValue(item.attributes);

                if (propertyValue == null)
                {
                    continue;
                }

                switch (jsonPropertyName)
                {
                    case "favorite":
                    case "item_seen":
                        relevantAttributes[jsonPropertyName] = (bool)propertyValue;
                        break;
                    case "xp":
                    case "level":
                    case "use_count":
                        relevantAttributes[jsonPropertyName] = (int)propertyValue;
                        break;
                    case "platform":
                    case "banner_color_template":
                    case "banner_icon_template":
                    case "locker_name":
                        relevantAttributes[jsonPropertyName] = propertyValue?.ToString() ?? "";
                        break;
                    case "variants":
                        relevantAttributes[jsonPropertyName] = (List<Variants>)propertyValue;
                        break;
                    case "locker_slots_data":
                        relevantAttributes[jsonPropertyName] = propertyValue;
                        break;
                    default:
                        missingProperties.Add(jsonPropertyName);
                        break;
                }
            }

            if (missingProperties.Any())
            {
                Logger.Warning($"Missing properties: {string.Join(", ", missingProperties)}");
            }

            if (!relevantAttributes.Any())
            {
                return;
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
                var saveTasks = new List<Task>();

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
                    saveTasks.Add(itemsRepository.SaveAsync(newItem));
                }

                await Task.WhenAll(saveTasks);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save attributes: {ex.Message}");
            }
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
                var connectionUrl = config.ConnectionUrl;

                var itemsRepository = new Repository<Items>(connectionUrl);
                var loadoutsRepository = new Repository<Loadouts>(connectionUrl);
                var profilesRepository = new Repository<Profiles>(connectionUrl);

                var primaryProfileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);
                var profileItemsTask = itemsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);
                var profileLoadoutsTask = loadoutsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);

                await Task.WhenAll(primaryProfileTask, profileItemsTask, profileLoadoutsTask);

                var primaryProfile = await primaryProfileTask;
                var profileItems = await profileItemsTask;
                var profileLoadouts = await profileLoadoutsTask;

                if (primaryProfile == null)
                {
                    Logger.Error($"Profile not found: ProfileId: {profileId}, AccountId: {accountId}");
                    return null;
                }

                IProfile constructedItems = profileId switch
                {
                    "athena" => new AthenaProfile(accountId, profileItems, profileLoadouts, primaryProfile).GetProfile(),
                    "common_core" => new CommonCoreProfile(accountId, profileItems, primaryProfile).GetProfile(),
                    _ => null
                };

                if (constructedItems == null)
                {
                    Logger.Error($"Missing or invalid profile: {profileId}");
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
                timer.Stop();
            }
        }
    }
}