﻿using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Interfaces;
using Larry.Source.Database.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Larry.Source.Utilities;
using System.Reflection;
using Larry.Source.Classes.Profiles.Builders;

namespace Larry.Source.Classes.Profiles
{
    public class CommonCoreProfile : ProfileBase
    {
        /// <summary>
        /// Gets the profile instance.
        /// </summary>
        public IProfile Profile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonCoreProfile"/> class.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="items">The list of items.</param>
        /// <param name="profile">The profile</param>
        public CommonCoreProfile(string accountId, List<Items> items, Larry.Source.Database.Entities.Profiles profile) : base(null)
        {
            try
            {
                Profile = CreateProfile(accountId, items, profile);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing CommonCoreProfile: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates an CommonCore profile based on the provided account ID and items.
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
                    survey_data = new object(),
                    personal_offers = new object(),
                    intro_game_played = true,
                    import_friends_claimed = new object(),
                    mtx_purchase_history = new MtxPurchaseHistory
                    {
                        purchases = new List<object>(),
                        refundCredits = 3,
                        refundsUsed = 0,
                    },
                    undo_cooldowns = new List<object>(),
                    mtx_affiliate_set_time = "None",
                    inventory_limit_bonus = 0,
                    current_mtx_platform = "EpicPC",
                    mtx_affiliate = "None",
                    forced_intro_played = "Coconut",
                    weekly_purchases = new object(),
                    daily_purchases = new object(),
                    ban_history = new BanHistory
                    {
                        banCount = new Dictionary<string, int>(),
                        banTier = new Dictionary<string, int>(),
                    },
                    in_app_purchases = new InAppPurchases
                    {
                        receipts = new List<string>(),
                        fulfillmentCounts = new Dictionary<string, int>(),
                        ignoredReceipts = new List<string>(),
                        refreshTimers = new Dictionary<string, RefreshTimer>
                        {
                            { "EpicPurchasingService", new RefreshTimer
                            {
                                NextEntitlementRefresh = DateTime.UtcNow.ToString("o")
                            } }
                        },
                        version = 1
                    },
                    permissions = new List<object>(),
                    undo_timeout = "min",
                    monthly_purchases = new object(),
                    allowed_to_receive_gifts = true,
                    mfa_enabled = false,
                    allowed_to_send_gifts = true,
                    gift_history = new object(),
                    banner_icon = "",
                    banner_color = "",
                    homebase_name = "",
                    gifts = new List<Gifts>(),
                    bans = new BanStatus
                    {
                        bRequiresUserAck = false,
                        bBanHasStarted = false,
                        banReasons = new List<string>(),
                        banStartTimeUtc = null,
                        banDurationDays = null,
                        additionalInfo = "",
                        exploitProgramName = "",
                        competitiveBanReason = "None"
                    }
                }
            };

            foreach (var item in items)
            {
                GenerateItem(item, defaultItems, initialStats);
            }

            return BuildProfileSkeleton(accountId, defaultItems, initialStats, profile);
        }

        /// <summary>
        /// Generates an individual item to update the profile's item definitions and stats.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <param name="defaultItems">The dictionary of default items.</param>
        /// <param name="initialStats">The initial stats for the profile.</param>
        private void GenerateItem(Items item, Dictionary<string, ItemDefinition> defaultItems, StatsAttributes initialStats)
        {
            if (item == null || item.Value == null)
                return;

            var ignoredCommonCoreStats = new HashSet<string>
            {
                "current_season",
                "max_island_plots",
                "publish_allowed",
                "use_random_loadout",
                "season_match_boost",
                "mfa_reward_claimed",
                "rested_xp_overflow",
                "matches_played",
                "packs_granted",
                "book_level",
                "season_num",
                "book_xp",
                "lifetime_wins",
                "book_purchased",
                "rested_xp_exchange",
                "level",
                "rested_xp",
                "rested_xp_mult",
                "accountLevel",
                "rested_xp_cumulative",
                "xp",
                "battlestars",
                "battlestars_season_total",
                "season_friend_match_boost",
                "active_loadout_index"
            };


            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<dynamic>(item.Value) ?? new JObject();

                if (item.IsStat == true)
                {
                    if (deserializedValue == null)
                        return;

                    try
                    {
                        var updates = new Dictionary<string, StatAttributeUpdater>
                        {
                                { "survey_data", (value, attr) => attr.attributes.survey_data = value ?? new object() },
                                { "personal_offers", (value, attr) => attr.attributes.personal_offers = value ?? new object() },
                                { "intro_game_played", (value, attr) => attr.attributes.intro_game_played = (bool)value },
                                { "import_friends_claimed", (value, attr) => attr.attributes.import_friends_claimed = value ?? new object() },
                                { "mtx_purchase_history", (value, attr) => attr.attributes.mtx_purchase_history = new MtxPurchaseHistory
                                    {
                                        purchases = new List<object>(),
                                        refundCredits = 3,
                                        refundsUsed = 0,
                                    }
                                },
                                { "undo_cooldowns", (value, attr) => attr.attributes.undo_cooldowns = new List<object>() },
                                { "mtx_affiliate_set_time", (value, attr) => attr.attributes.mtx_affiliate_set_time = "None" },
                                { "inventory_limit_bonus", (value, attr) => attr.attributes.inventory_limit_bonus = 0 },
                                { "current_mtx_platform", (value, attr) => attr.attributes.current_mtx_platform = "EpicPC" },
                                { "mtx_affiliate", (value, attr) => attr.attributes.mtx_affiliate = "None" },
                                { "forced_intro_played", (value, attr) => attr.attributes.forced_intro_played = "Coconut" },
                                { "weekly_purchases", (value, attr) => attr.attributes.weekly_purchases = new object() },
                                { "daily_purchases", (value, attr) => attr.attributes.daily_purchases = new object() },
                                { "ban_history", (value, attr) => attr.attributes.ban_history = new BanHistory
                                    {
                                        banCount = new Dictionary<string, int>(),
                                        banTier = new Dictionary<string, int>(),
                                    }
                                },
                                { "in_app_purchases", (value, attr) => attr.attributes.in_app_purchases = new InAppPurchases
                                {
                                    receipts = new List<string>(),
                                    fulfillmentCounts = new Dictionary<string, int>(),
                                    ignoredReceipts = new List<string>(),
                                    refreshTimers = new Dictionary<string, RefreshTimer>
                                    {
                                        { "EpicPurchasingService", new RefreshTimer
                                            {
                                                NextEntitlementRefresh = DateTime.UtcNow.ToString("o")
                                            }
                                        }
                                    },
                                    version = 1
                                }
                            },
                            { "permissions", (value, attr) => attr.attributes.permissions = new List<object>() },
                            { "undo_timeout", (value, attr) => attr.attributes.undo_timeout = "min" },
                            { "monthly_purchases", (value, attr) => attr.attributes.monthly_purchases = new object() },
                            { "allowed_to_receive_gifts", (value, attr) => attr.attributes.allowed_to_receive_gifts = (bool)value },
                            { "mfa_enabled", (value, attr) => attr.attributes.mfa_enabled = (bool)value },
                            { "allowed_to_send_gifts", (value, attr) => attr.attributes.allowed_to_send_gifts = (bool)value },
                            { "gift_history", (value, attr) => attr.attributes.gift_history = new object() },
                            { "banner_icon", (value, attr) => attr.attributes.banner_icon = "" },
                            { "banner_color", (value, attr) => attr.attributes.banner_color = "" },
                            { "homebase_name", (value, attr) => attr.attributes.homebase_name = "" },
                            { "gifts", (value, attr) => attr.attributes.gifts = new List<Gifts>() },
                            { "bans", (value, attr) => attr.attributes.bans = new BanStatus
                                {
                                    bRequiresUserAck = false,
                                    bBanHasStarted = false,
                                    banReasons = new List<string>(),
                                    banStartTimeUtc = null,
                                    banDurationDays = null,
                                    additionalInfo = "",
                                    exploitProgramName = "",
                                    competitiveBanReason = "None"
                                }
                            },
                        };

                        if (updates.TryGetValue(item.TemplateId, out var updateAction))
                        {
                            updateAction(deserializedValue, initialStats);
                        }
                        else if (!ignoredCommonCoreStats.Contains(item.TemplateId))
                        {
                            Logger.Warning($"Missing stat in profile '{item.ProfileId}': {item.TemplateId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to assign the value '{deserializedValue}' to '{item.TemplateId}'");
                    }
                }
                else
                {
                    var itemValue = JsonConvert.DeserializeObject<ItemValue>(item.Value) ?? new ItemValue();
                    GenerateNonStatItem(item, deserializedValue, defaultItems);
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
        private void GenerateNonStatItem(Items item, dynamic deserializedValue, Dictionary<string, ItemDefinition> defaultItems)
        {
            try
            {
                if (deserializedValue == null)
                {
                    Logger.Warning($"Item value for '{item.TemplateId}' is null.");
                    return;
                }

                if (item.TemplateId.Contains("Currency:MtxPurchased"))
                {
                    HandleMtxPurchasedItem(item, deserializedValue, defaultItems);
                    return;
                }
                else
                {
                    var attributesDict = new Dictionary<string, object>
                    {
                        { "use_count", deserializedValue.use_count as int? ?? 0 },
                        { "level", deserializedValue.level as int? ?? 1 },
                    };

                    UpdateOrCreateItem(defaultItems, item.TemplateId, item.Quantity, MapToItemValue(attributesDict));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating non-stat item '{item.TemplateId}': {ex}");
            }
        }

        /// <summary>
        /// Handles the processing of the "Currency:MtxPurchased" item.
        /// Updates or creates an entry in the <paramref name="defaultItems"/> dictionary for the given item.
        /// </summary>
        /// <param name="item">The item being processed.</param>
        /// <param name="deserializedValue">The deserialized dynamic value containing additional item details.</param>
        /// <param name="defaultItems">The dictionary to store or update item definitions.</param>
        private void HandleMtxPurchasedItem(Items item, dynamic deserializedValue, Dictionary<string, ItemDefinition> defaultItems)
        {
            const string key = "Currency:MtxPurchased";

            if (!defaultItems.TryGetValue(key, out var existingItem))
            {
                existingItem = new ItemDefinition
                {
                    templateId = key
                };
                defaultItems[key] = existingItem;
            }

            existingItem.quantity = item.Quantity;
            existingItem.attributes = new ItemValue
            {
                platform = deserializedValue.platform?.ToString() ?? "EpicPC"
            };
        }

        /// <summary>
        /// Updates an existing item in the dictionary or creates a new entry if it does not exist.
        /// </summary>
        /// <param name="items">The dictionary containing item definitions.</param>
        /// <param name="templateId">The unique template ID of the item.</param>
        /// <param name="quantity">The quantity of the item.</param>
        /// <param name="attributes">The item attributes to be added or updated.</param>
        private void UpdateOrCreateItem(Dictionary<string, ItemDefinition> items, string templateId, int quantity, ItemValue attributes)
        {
            if (items.TryGetValue(templateId, out var existingItem))
            {
                existingItem.quantity = quantity;
                existingItem.attributes = attributes;
            }
            else
            {
                items[templateId] = new ItemDefinition
                {
                    templateId = templateId,
                    quantity = quantity,
                    attributes = attributes
                };
            }
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
                level = 1,
                use_count = 0,
            };
        }


        /// <summary>
        /// Safely gets a value from the attributes dictionary, providing a default if the key does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the value to get.</typeparam>
        /// <param name="attributes">The attributes dictionary.</param>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value associated with the key or the default value of the type.</returns>
        private T GetValue<T>(Dictionary<string, object> attributes, string key)
        {
            return attributes.TryGetValue(key, out var value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
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

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var serializedItems = JsonConvert.SerializeObject(defaultItems, settings);
            var cleanedItems = JsonConvert.DeserializeObject<Dictionary<string, ItemDefinition>>(serializedItems);

            return new MCPProfile
            {
                created = DateTime.UtcNow.ToString("o"),
                updated = DateTime.UtcNow.ToString("o"),
                rvn = profile.Revision,
                wipeNumber = 1,
                accountId = accountId,
                profileId = "common_core",
                version = "no_version",
                stats = cleanedStats,
                items = cleanedItems,
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
