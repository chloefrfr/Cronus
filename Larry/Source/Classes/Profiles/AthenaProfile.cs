﻿using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Classes.Profiles.ProfileManagement;
using Larry.Source.Database.Entities;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Larry.Source.Classes.Profiles
{
    public class AthenaProfile : ProfileBase
    {
        /// <summary>
        /// Gets the profile instance.
        /// </summary>
        public IProfile Profile { get; private set; }

        private PresetsManager _presetsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaProfile"/> class.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="items">The list of items.</param>
        /// <param name="profile">The profile</param>
        public AthenaProfile(string accountId, List<Items> items, Larry.Source.Database.Entities.Profiles profile) : base(null)
        {
            try
            {
                Profile = CreateProfile(accountId, items, profile);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing AthenaProfile: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates an Athena profile based on the provided account ID and items.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="items">The list of items.</param>
        /// <param name="profile">The profile</param>
        /// <returns>The created profile.</returns>
        public IProfile CreateProfile(string accountId, List<Items> items, Larry.Source.Database.Entities.Profiles profile)
        {
            var defaultItems = new Dictionary<string, ItemDefinition>();
            var initialStats = new StatsAttributes
            {
                attributes = new StatsData
                {
                    use_random_loadout = false,
                    past_seasons = new List<PastSeasons>(),
                    season_match_boost = 0,
                    loadouts = new List<string>(),
                    mfa_reward_claimed = false,
                    rested_xp_overflow = 0,
                    current_mtx_platform = "Epic",
                    last_xp_interaction = DateTime.UtcNow.ToString("o"),
                    quest_manager = new QuestManager
                    {
                        dailyLoginInterval = DateTime.MinValue.ToString("o"),
                        dailyQuestRerolls = 1,
                        questPoolStats = new QuestPoolStats()
                    },
                    book_level = 1,
                    season_num = 1,
                    book_xp = 0,
                    creative_dynamic_xp = new Dictionary<string, int>(),
                    season = new Season { numWins = 0, numHighBracket = 0, numLowBracket = 0 },
                    lifetime_wins = 0,
                    book_purchased = false,
                    rested_xp_exchange = 1,
                    level = 1,
                    rested_xp = 2500,
                    rested_xp_mult = 4,
                    accountLevel = 1,
                    rested_xp_cumulative = 52500,
                    xp = 0,
                    active_loadout_index = 0,
                    favorite_character = string.Empty,
                    favorite_pickaxe = string.Empty,
                    favorite_glider = string.Empty,
                    favorite_dance = new List<string>(),
                    favorite_itemwraps = new List<string>(),
                    favorite_loadingscreen = string.Empty,
                    favorite_backpack = string.Empty,
                    favorite_skydivecontrail = string.Empty,
                    favorite_musicpack = string.Empty
                }
            };

            _presetsManager = new PresetsManager(initialStats, new Items(), "larry_loadout1");
            _presetsManager.AddLoadout(accountId);

            foreach (var item in items)
            {
                GenerateItem(item, defaultItems, initialStats, accountId);
            }

            //_presetsManager.AddLoadout(accountId);


            return BuildProfileSkeleton(accountId, defaultItems, initialStats, profile);
        }

        /// <summary>
        /// Generates an individual item to update the profile's item definitions and stats.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <param name="defaultItems">The dictionary of default items.</param>
        /// <param name="initialStats">The initial stats for the profile.</param>
        private void GenerateItem(Items item, Dictionary<string, ItemDefinition> defaultItems, StatsAttributes initialStats, string accountId)
        {
            if (item == null || item.Value == null)
                return;

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<dynamic>(item.Value) ?? new JObject();

                if (item.IsStat == true)
                {
                    if (deserializedValue == null)
                        return;

                    try
                    {
                        UpdateStats(item, deserializedValue, initialStats, accountId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to assign the value '{deserializedValue}' to '{item.TemplateId}'");
                    }
                }
                else
                {
                    var itemValue = JsonConvert.DeserializeObject<ItemValue>(item.Value) ?? new ItemValue();
                    UpdateItemDefinitions(item, itemValue, defaultItems);
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Unexpected character encountered while parsing value: A"))
                {
                    Logger.Error($"Error generating item '{item.TemplateId}': {ex.Message}");
                }
            }
        }

        delegate void StatAttributeUpdater(object deserializedValue, StatsAttributes statAttribute);

        /// <summary>
        /// Updates the stats based on the item data.
        /// </summary>
        /// <param name="item">The item whose stats will be updated.</param>
        /// <param name="deserializedValue">The deserialized item data.</param>
        /// <param name="initialStats">The initial stats for the profile.</param>
        private async void UpdateStats(Items item, dynamic deserializedValue, StatsAttributes initialStats, string accountId)
        {
            var updates = new Dictionary<string, StatAttributeUpdater>
            {
                { "use_random_loadout", (value, attr) => attr.attributes.use_random_loadout = (bool)value },
                { "past_seasons", (value, attr) =>
                    attr.attributes.past_seasons = value is JToken token ? token.ToObject<List<PastSeasons>>() ?? new List<PastSeasons>() : new List<PastSeasons>()
                },
                { "season_match_boost", (value, attr) => attr.attributes.season_match_boost = Convert.ToInt32(value) },
                { "loadouts", (value, attr) =>
                    attr.attributes.loadouts = value is JToken loadoutsToken ? loadoutsToken.ToObject<List<string>>() ?? new List<string>() : new List<string>()
                },
                { "mfa_reward_claimed", (value, attr) => attr.attributes.mfa_reward_claimed = (bool)value },
                { "rested_xp_overflow", (value, attr) => attr.attributes.rested_xp_overflow = Convert.ToInt32(value) },
                { "current_mtx_platform", (value, attr) => attr.attributes.current_mtx_platform = value as string },
                { "last_xp_interaction", (value, attr) => attr.attributes.last_xp_interaction = value as string },
                { "book_level", (value, attr) => attr.attributes.book_level = Convert.ToInt32(value) },
                { "season_num", (value, attr) => attr.attributes.season_num = Convert.ToInt32(value) },
                { "book_xp", (value, attr) => attr.attributes.book_xp = Convert.ToInt32(value) },
                { "season", (value, attr) =>
                    attr.attributes.season = value is JToken seasonToken ? seasonToken.ToObject<Season>() ?? new Season() : new Season()
                },
                { "lifetime_wins", (value, attr) => attr.attributes.lifetime_wins = Convert.ToInt32(value) },
                { "book_purchased", (value, attr) => attr.attributes.book_purchased = (bool)value },
                { "rested_xp_exchange", (value, attr) => attr.attributes.rested_xp_exchange = Convert.ToInt32(value) },
                { "level", (value, attr) => attr.attributes.level = Convert.ToInt32(value) },
                { "rested_xp", (value, attr) => attr.attributes.rested_xp = Convert.ToInt32(value) },
                { "rested_xp_mult", (value, attr) => attr.attributes.rested_xp_mult = Convert.ToInt32(value) },
                { "accountLevel", (value, attr) => attr.attributes.accountLevel = Convert.ToInt32(value) },
                { "rested_xp_cumulative", (value, attr) => attr.attributes.rested_xp_cumulative = Convert.ToInt32(value) },
                { "xp", (value, attr) => attr.attributes.xp = Convert.ToInt32(value) },
                { "active_loadout_index", (value, attr) => attr.attributes.active_loadout_index = Convert.ToInt32(value) },
                { "favorite_character", (value, attr) => attr.attributes.favorite_character = value?.ToString() },
                { "favorite_pickaxe", (value, attr) => attr.attributes.favorite_pickaxe = value?.ToString() },
                { "favorite_glider", (value, attr) => attr.attributes.favorite_glider = value?.ToString() },
                { "favorite_backpack", (value, attr) => attr.attributes.favorite_backpack = value?.ToString() },
                { "favorite_loadingscreen", (value, attr) => attr.attributes.favorite_loadingscreen = value?.ToString() },
                { "favorite_skydivecontrail", (value, attr) => attr.attributes.favorite_skydivecontrail = value?.ToString() },
                { "favorite_musicpack", (value, attr) => attr.attributes.favorite_musicpack = value?.ToString() },
                { "favorite_itemwraps", (value, attr) =>
                    attr.attributes.favorite_itemwraps = value is JToken favoriteDanceToken ? favoriteDanceToken.ToObject<List<string>>() ?? new List<string>() : new List<string>()
                },
                { "favorite_dance", (value, attr) =>
                    attr.attributes.favorite_dance = value is JToken favoriteDanceToken ? favoriteDanceToken.ToObject<List<string>>() ?? new List<string>() : new List<string>()
                },
                { "allowed_to_receive_gifts", (value, attr) => attr.attributes.allowed_to_receive_gifts = (bool)value },
                { "mfa_enabled", (value, attr) => attr.attributes.mfa_enabled = (bool)value },
                { "allowed_to_send_gifts", (value, attr) => attr.attributes.allowed_to_send_gifts = (bool)value }
            };

            if (updates.TryGetValue(item.TemplateId, out var updateAction))
            {
                updateAction(deserializedValue, initialStats);
            }
            else
            {
                Logger.Warning($"Missing stat in profile '{item.ProfileId}': {item.TemplateId}");
            }

            Config config = Config.GetConfig();
            Repository<Items> itemsRepository = new Repository<Items>(config.ConnectionUrl);

            var allStats = await itemsRepository.GetAllItemsByAccountIdAsync(accountId, "athena");
            var attributes = initialStats.attributes;

            var attributeMapping = new Dictionary<string, Action<string>>
            {
                { "favorite_character", value => attributes.favorite_character = value },
                { "favorite_pickaxe", value => attributes.favorite_pickaxe = value },
                { "favorite_glider", value => attributes.favorite_glider = value },
                { "favorite_skydivecontrail", value => attributes.favorite_skydivecontrail = value },
                { "favorite_backpack", value => attributes.favorite_backpack = value },
                { "favorite_loadingscreen", value => attributes.favorite_loadingscreen = value },
                { "favorite_musicpack", value => attributes.favorite_musicpack = value },
                { "favorite_dance", value =>
                {
                    if (attributes.favorite_dance is not List<string> favoriteDances)
                    {
                        favoriteDances = new List<string>();
                        attributes.favorite_dance = favoriteDances;
                    }
                    UpdateFavoriteItems(value, ref favoriteDances);
                } },
                { "favorite_itemwraps", value =>
                {
                    if (attributes.favorite_itemwraps is not List<string> favoriteWraps)
                    {
                        favoriteWraps = new List<string>();
                        attributes.favorite_itemwraps = favoriteWraps;
                    }
                    UpdateFavoriteItems(value, ref favoriteWraps);
                } },
            };

            foreach (var stat in allStats)
            {
                if (stat.IsStat == true && attributeMapping.TryGetValue(stat.TemplateId, out var action))
                {
                    action(stat.Value);
                }
            }
        }

        /// <summary>
        /// Updates a list of favorite items based on a comma-separated string.
        /// </summary>
        /// <param name="value">The comma-separated string containing favorite items.</param>
        /// <param name="favorites">The list to be updated.</param>
        private void UpdateFavoriteItems(string value, ref List<string> favorites)
        {
            favorites ??= new List<string>();
            favorites.Clear();

            var items = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            favorites.AddRange(items);
        }

        /// <summary>
        /// Updates item definitions based on the provided item data and attributes.
        /// </summary>
        /// <param name="item">The item to update or add.</param>
        /// <param name="deserializedValue">The deserialized item data.</param>
        /// <param name="defaultItems">The dictionary of item definitions to update.</param>
        private void UpdateItemDefinitions(Items item, dynamic deserializedValue, Dictionary<string, ItemDefinition> defaultItems)
        {
            if (defaultItems == null)
            {
                Logger.Error("defaultItems is null.");
                return;
            }

            if (deserializedValue == null)
            {
                Logger.Error("deserializedValue is null.");
                return;
            }

            if (item.TemplateId.Contains("larry_loadout1"))
            {
                var presetsDict = new Dictionary<string, object>
                {
                    { "banner_color_template", deserializedValue.banner_color_template ?? "" },
                    { "banner_icon_template", deserializedValue.banner_icon_template ?? "" },
                    { "item_seen", deserializedValue.item_seen ?? false },
                    { "locker_name", deserializedValue.locker_name ?? "" },
                    { "locker_slots_data", deserializedValue.locker_slots_data ?? new LockerSlotData() }
                };

                if (defaultItems.ContainsKey(item.TemplateId))
                {
                    defaultItems[item.TemplateId].quantity = item.Quantity;
                    defaultItems[item.TemplateId].attributes = MapToItemValueForPresets(presetsDict); 
                }
                else
                {
                    defaultItems[item.TemplateId] = new ItemDefinition
                    {
                        templateId = item.TemplateId,
                        quantity = item.Quantity,
                        attributes = MapToItemValueForPresets(presetsDict) 
                    };
                }
            }
            else
            {
                var attributesDict = new Dictionary<string, object>
                {
                    { "favorite", deserializedValue.favorite ?? false },
                    { "item_seen", deserializedValue.item_seen ?? false },
                    { "xp", deserializedValue.xp ?? 0 },
                    { "variants", deserializedValue.variants ?? new List<object>() }
                };

                if (defaultItems.ContainsKey(item.TemplateId))
                {
                    defaultItems[item.TemplateId].quantity = item.Quantity;
                    defaultItems[item.TemplateId].attributes = MapToItemValue(attributesDict); 
                }
                else
                {
                    defaultItems[item.TemplateId] = new ItemDefinition
                    {
                        templateId = item.TemplateId,
                        quantity = item.Quantity,
                        attributes = MapToItemValue(attributesDict)  
                    };
                }
            }
        }

        /// <summary>
        /// Maps a dictionary of preset attributes to an <see cref="ItemValue"/> for presets.
        /// </summary>
        /// <param name="presets">The presets dictionary.</param>
        /// <returns>The mapped <see cref="ItemValue"/> for presets.</returns>
        private ItemValue MapToItemValueForPresets(Dictionary<string, object> presets)
        {
            return new ItemValue
            {
                banner_color_template = GetValue<string>(presets, "banner_color_template"),
                banner_icon_template = GetValue<string>(presets, "banner_icon_template"),
                item_seen = GetValue<bool>(presets, "item_seen"),
                locker_name = GetValue<string>(presets, "locker_name"),
                locker_slots_data = GetValue<LockerSlotData>(presets, "locker_slots_data")
            };
        }


        /// <summary>
        /// Cleans null attributes from the initial stats.
        /// </summary>
        /// <param name="initialStats">The initial statistics to clean.</param>
        /// <returns>A <see cref="StatsAttributes"/> instance with cleaned attributes.</returns>
        private StatsAttributes CleanNullAttributes(StatsAttributes initialStats)
        {
            var cleanedAttributes = new StatsAttributes
            {
                attributes = new Dictionary<string, object>()
            };

            foreach (var prop in initialStats.attributes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(initialStats.attributes);
                if (value != null && !(value is IList<object> list && list.Count == 0))
                {
                    cleanedAttributes.attributes[prop.Name] = value;
                }
            }

            return cleanedAttributes;
        }


        /// <summary>
        /// Maps a dictionary of attributes to an <see cref="ItemValue"/>.
        /// </summary>
        /// <param name="attributes">The attributes dictionary.</param>
        /// <returns>The mapped <see cref="ItemValue"/>.</returns>
        private ItemValue MapToItemValue(Dictionary<string, object> attributes)
        {
            return new ItemValue
            {
                favorite = GetValue<bool>(attributes, "favorite"),
                item_seen = GetValue<bool>(attributes, "item_seen"),
                xp = GetValue<int>(attributes, "xp"),
                variants = GetVariants(attributes)
            };
        }

        /// <summary>
        /// Safely Gets a value from the attributes dictionary, providing a default if the key does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="attributes">The attributes dictionary.</param>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value associated with the key or the default value of the type.</returns>
        private T GetValue<T>(Dictionary<string, object> attributes, string key)
        {
            return attributes.TryGetValue(key, out var value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
        }

        /// <summary>
        ///Gets a list of variants from the attributes dictionary.
        /// </summary>
        /// <param name="attributes">The attributes dictionary.</param>
        /// <returns>A list of <see cref="Variants"/>.</returns>
        private List<Variants> GetVariants(Dictionary<string, object> attributes)
        {
            return attributes.TryGetValue("variants", out var variants) && variants is IEnumerable<object> varList
                ? varList.Select(v => new Variants
                {
                    channel = (v as dynamic)?.channel ?? string.Empty,
                    owned = (v as dynamic)?.owned ?? new List<string>(),
                    active = (v as dynamic)?.active ?? string.Empty
                }).ToList()
                : new List<Variants>();
        }

        /// <summary>
        /// Builds a profile skeleton with the given data.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="defaultItems">The dictionary of default items.</param>
        /// <param name="initialStats">The initial stats for the profile.</param>
        /// <param name="profile">The profile</param>
        /// <returns>The built profile.</returns>
        private IProfile BuildProfileSkeleton(string accountId, Dictionary<string, ItemDefinition> defaultItems, StatsAttributes initialStats, Larry.Source.Database.Entities.Profiles profile)
        {
            // Fixes non-declare attributes from showing
            var cleanedStats = CleanNullAttributes(initialStats);
            return new MCPProfile
            {
                created = DateTime.UtcNow.ToString("o"),
                updated = DateTime.UtcNow.ToString("o"),
                rvn = profile.Revision,
                wipeNumber = 1,
                accountId = accountId,
                profileId = "athena",
                version = "no_version",
                stats = cleanedStats,
                items = defaultItems,
                commandRevision = profile.Revision,
            };
        }

        /// <summary>
        /// Gets the profile associated with this instance.
        /// </summary>
        /// <returns>An instance of <see cref="IProfile"/> representing the profile.</returns>
        public override IProfile GetProfile()
        {
            return Profile;
        }
    }
}